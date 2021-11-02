using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using System;

public class EnemyBrain : MonoBehaviour
{
    private int layer = 1 << 0;
    private bool playingAnim;
    private bool isDying;
    private int currentAminHash;
    private AnimType animType;
    [SerializeField] private EnemyTypes enemyType;
    private Vector3 startPos;
    private Enemy enemy;
    private StateMachine stateMachine;
    private EnemyTrigger enemyTrigger;
    private NavMeshAgent navAgent;
    private VisualEffect vfxGraph;
    private BloodEffectOnHit bloodOnHit;
    private BloodEffectOnDeath bloodOnDeath;
    private Target enemyTarget;
    private Animator animator;
    private TempShieldTrigger tempShield;
    public Vector3 StartPos { get => startPos; }
    public NavMeshAgent navMeshAgent { get => navAgent; }  
    [HideInInspector] public bool IsSetupDone = false;
    private int idleHash = Animator.StringToHash("EnemyIdle");
    private int moveHash = Animator.StringToHash("EnemyMove");
    private int attackHash = Animator.StringToHash("EnemyAttack");
    private int deathHash = Animator.StringToHash("EnemyDeath");
    
    private enum AnimType
    {
        Idle,
        Walk,
        Attack,
        Death
    }

    private void OnEnable()
    {
        StartCoroutine(SetEnemy());
    }

    private void OnDisable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChange -= StateMachine_OnStateChange;
        }
        if (enemy != null)
        {
            enemy.OnEnemyAttack -= Enemy_OnEnemyAttack;
        }
        if (enemyTarget != null)
        {
            enemyTarget.OnDodge -= EnemyTarget_OnDodge;
            enemyTarget.Resource.OnKilled -= Resource_OnKilled;
        }
        Weapons.OnPlayerDamage -= Weapons_OnPlayerDamage;
    }

    private void Update()
    {
        if (IsSetupDone)
        {
            stateMachine.Tick();
            StartCoroutine(PlayAnim());
        }      
    }

    public Enemy GetThisEnemy()
    {
        return enemy;
    }

    public EnemyTypes GetEnemyType()
    {
        return enemyType;
    }

    private IEnumerator SetEnemy()
    {
        yield return new WaitForSeconds(1f);
        switch (enemyType)      
        {
            case EnemyTypes.Giant:
                enemy = new Giant(this, enemyType);
                break;
            case EnemyTypes.Fighter:
                enemy = new Fighter(this, enemyType);
                break;
        }
        enemy.OnEnemyAttack += Enemy_OnEnemyAttack;
        Weapons.OnPlayerDamage += Weapons_OnPlayerDamage;
        enemyTrigger = GetComponentInChildren<EnemyTrigger>();
        enemyTrigger.TriggerSetup(enemy);
        tempShield = GetComponentInChildren<TempShieldTrigger>();
        tempShield.SetShield(enemy.ThisEnemySO.ShieldSize, enemy.ThisEnemySO.ShieldDuration);
        navAgent = GetComponent<NavMeshAgent>();
        vfxGraph = GetComponentInChildren<VisualEffect>();
        bloodOnHit = GetComponentInChildren<BloodEffectOnHit>();
        bloodOnDeath = GetComponentInChildren<BloodEffectOnDeath>();
        animator = GetComponentInChildren<Animator>();
        playingAnim = false;
        isDying = false;
        stateMachine = new StateMachine(this);
        navAgent.speed = enemy.ThisEnemySO.MoveSpeed;
        stateMachine.OnStateChange += StateMachine_OnStateChange;
        startPos = transform.position;
        enemyTarget = GetComponent<Target>();
        enemyTarget.SetupMaxHP(enemy.ThisEnemySO.MaxHP);
        enemyTarget.SetEB(this);
        enemyTarget.OnDodge += EnemyTarget_OnDodge;
        enemyTarget.Resource.OnKilled += Resource_OnKilled;
        IsSetupDone = true;
    }

    //~~~~~~~~~~~~~~~~~~~ Animation ~~~~~~~~~~~~~~~~~~~

    private IEnumerator PlayAnim()
    {
        //AnimStateCheck();
        if (!playingAnim && animType != AnimType.Death)
        {
            if (animType == AnimType.Idle)
            {
                ChangeAnimState(idleHash);
                playingAnim = false;
                yield return null;

            }
            if (animType == AnimType.Walk)
            {
                ChangeAnimState(moveHash);
                playingAnim = false;
                yield return null;
            }
            else if (animType == AnimType.Attack)
            {
                ChangeAnimState(attackHash);
                StartCoroutine(ApplyDelay());
            }
        }
        else if (animType == AnimType.Death && !isDying)
        {
            isDying = true;
            ChangeAnimState(deathHash);
            bloodOnDeath.PlayBloodOnDeath();
            StartCoroutine(ApplyDelay());
            StartCoroutine(AfterKilled());
        }
    }

    private void ChangeAnimState(int animHash)
    {
        if (currentAminHash == animHash) return;
        playingAnim = true;
        animator.PlayInFixedTime(animHash);
        currentAminHash = animHash;
    }

    private IEnumerator ApplyDelay()
    {
        yield return new WaitForEndOfFrame();
        float animDelay = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(DelayAnim(animDelay));
    }

    private IEnumerator DelayAnim(float animDelay)
    {
        yield return new WaitForSeconds(animDelay);
        playingAnim = false;
    }

    private IEnumerator AfterKilled()
    {
        yield return new WaitForSeconds(2f);
        //yield return new WaitWhile(() => playingAnim);
        gameObject.SetActive(false);
        AssetCollections.ReleaseAssetInstance(gameObject, "Enemy", false);
    }

    //~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~

    private void EnemyTarget_OnDodge()
    {
        StartCoroutine(tempShield.TriggerShield(enemy.ThisEnemySO.ShieldDuration));
    }

    private void Resource_OnKilled(object sender, EventArgs e)
    {
        if (e != null)
        {
            navAgent.isStopped = true;
            animType = AnimType.Death;
        }        
    }

    private void Weapons_OnPlayerDamage(object sender, OnPlayerDamageEventArg e)
    {
        if (e.enemyBrain == this)
        {
            bloodOnHit.PlayBloodOnHit();
            vfxGraph.Play();
        }
    }

    private void StateMachine_OnStateChange(State obj)
    {
        if (animType != AnimType.Death)
        {
            if (obj.GetType() == typeof(Roam))
            {
                if (animType != AnimType.Walk)
                    animType = AnimType.Walk;
            }
            else if (obj.GetType() == typeof(Chase))
            {
                if (animType != AnimType.Walk)
                    animType = AnimType.Walk;
            }
            else if (obj.GetType() == typeof(Attack))
            {
                if (animType != AnimType.Attack)
                    animType = AnimType.Attack;
            }
            Debug.Log(obj);
        }        
    }

    private void Enemy_OnEnemyAttack(object sender, OnEnemyAttackEventArg e)
    {
            
    }
}
