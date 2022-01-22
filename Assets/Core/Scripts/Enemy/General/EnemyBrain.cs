using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using System;

public enum AnimType
{
    Idle,
    Walk,
    Attack,
    Death
}

public class EnemyBrain : MonoBehaviour
{
    private int layer = 1 << 0;
    private bool playingAnim;
    private bool isDying;
    private bool isSpellAffected;
    private bool isFrozen;
    private int currentAminHash;
    private AnimType animType;
    private AnimType lastAnimType;
    [SerializeField] private EnemyTypes enemyType;
    private Vector3 startPos;
    private Enemy enemy;
    private AOETargeted spellCasted;
    private SpellTypes spellType;
    private StateMachine stateMachine;
    private EnemyTrigger enemyTrigger;
    private NavMeshAgent navAgent;
    private VisualEffect vfxGraph;
    private BloodEffectOnHit bloodOnHit;
    private BloodEffectOnDeath bloodOnDeath;
    private TriggerHeadshot headshot;
    private Target enemyTarget;
    private Animator animator;
    private WaitForSeconds attackAnimWait;
    private WaitForSeconds deathAnimWait;
    private WaitForSeconds postDeathWait = new WaitForSeconds(2f);
    private TempShieldTrigger tempShield;
    public Vector3 StartPos { get => startPos; }
    public Target EnemyTarget { get => enemyTarget; }
    public NavMeshAgent navMeshAgent { get => navAgent; }  
    public SpellTypes SpellType { get => spellType; }
    public bool IsSpellAffected { get => isSpellAffected; }
    public bool IsFrozen { get => isFrozen; }
    [HideInInspector] public bool IsSetupDone = false;
    private int idleHash = Animator.StringToHash("EnemyIdle");
    private int moveHash = Animator.StringToHash("EnemyMove");
    private int attackHash = Animator.StringToHash("EnemyAttack");
    private int deathHash = Animator.StringToHash("EnemyDeath");    

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
        AOETargeted.OnAOESpellCast -= AOETargeted_OnAOESpellCast;
    }

    private void Update()
    {
        if (IsSetupDone)
        {
            stateMachine.Tick();
            PlayAnim();
            //StartCoroutine(PlayAnim());
        }      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AOESpell"))
        {
            if (!isSpellAffected && spellCasted != null)
            {
                isSpellAffected = true;
                if (spellType == SpellTypes.FreezeBlast)
                {
                    isFrozen = true;
                }
                StartCoroutine(spellCasted.AddSpellEffect(this, () =>
                {
                    isSpellAffected = false;
                    if (isFrozen) { isFrozen = false; }                        
                }));
            }
            
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
        AOETargeted.OnAOESpellCast += AOETargeted_OnAOESpellCast;
        enemyTrigger = GetComponentInChildren<EnemyTrigger>();
        enemyTrigger.TriggerSetup(enemy);
        tempShield = GetComponentInChildren<TempShieldTrigger>();
        tempShield.SetShield(enemy.ThisEnemySO.ShieldSize, enemy.ThisEnemySO.ShieldDuration);
        navAgent = GetComponent<NavMeshAgent>();
        vfxGraph = GetComponentInChildren<VisualEffect>();
        bloodOnHit = GetComponentInChildren<BloodEffectOnHit>();
        bloodOnDeath = GetComponentInChildren<BloodEffectOnDeath>();
        headshot = GetComponentInChildren<TriggerHeadshot>();
        animator = GetComponentInChildren<Animator>();
        playingAnim = false;
        isDying = false;
        isSpellAffected = false;
        isFrozen = false;
        stateMachine = new StateMachine(this);
        navAgent.speed = enemy.ThisEnemySO.MoveSpeed;
        stateMachine.OnStateChange += StateMachine_OnStateChange;
        startPos = transform.position;
        enemyTarget = GetComponent<Target>();
        enemyTarget.SetupMaxHP(enemy.ThisEnemySO.MaxHP);
        enemyTarget.SetEB(this);
        enemyTarget.OnDodge += EnemyTarget_OnDodge;
        enemyTarget.Resource.OnKilled += Resource_OnKilled;
        AssignWaiters();
        IsSetupDone = true;
    }

    //~~~~~~~~~~~~~~~~~~~ Animation ~~~~~~~~~~~~~~~~~~~

    private void AssignWaiters()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach(var clip in clips)
        {
            if (clip.name == "EnemyAttack")
            {
                attackAnimWait = new WaitForSeconds(clip.length);
            }else if (clip.name == "EnemyDeath")
            {
                deathAnimWait = new WaitForSeconds(clip.length);
            }
        }
    }

    public void ChangeAnimType(AnimType AT)
    {
        lastAnimType = animType;
        animType = AT;
    }

    public void ResetAnimType()
    {
        animType = lastAnimType;
    }

    private void PlayAnim()
    {
        //AnimStateCheck();
        if (!playingAnim && animType != AnimType.Death)
        {
            if (animType == AnimType.Idle)
            {
                ChangeAnimState(idleHash);
                playingAnim = false;
            }
            if (animType == AnimType.Walk)
            {
                ChangeAnimState(moveHash);
                playingAnim = false;
            }
            else if (animType == AnimType.Attack)
            {
                ChangeAnimState(attackHash);
                StartCoroutine(DelayAnim(attackAnimWait));
            }
        }
        else if (animType == AnimType.Death && !isDying)
        {
            isDying = true;
            ChangeAnimState(deathHash);
            bloodOnDeath.PlayBloodOnDeath();
            StartCoroutine(DelayAnim(deathAnimWait));
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

    private IEnumerator DelayAnim(WaitForSeconds wait)
    {     
        if (!isFrozen)
        {
            yield return wait;
        }
        playingAnim = false;
    }

    private IEnumerator AfterKilled()
    {
        yield return postDeathWait;
        gameObject.SetActive(false);
        AssetCollections.ReleaseAssetInstance(gameObject, "Enemy", false);
    }

    public Vector2 GetDirWRTPlayer(Camera cam)
    {
        if (navAgent != null)
        {
            Vector2 dir = (navAgent.destination - transform.position).normalized;
            float dot = Vector2.Dot(cam.transform.forward, dir);
            Vector2 result = Vector2.zero;
            if (dot <= -0.5f)
            {
                result = new Vector2(0, 1);
            }
            else if (dot >= 0.5f)
            {
                result = new Vector2(0, -1);
            }
            else
            {
                float subDot = Vector2.Dot(cam.transform.right, dir);
                if (subDot >= 0.5f)
                {
                    result = new Vector2(1, 0);
                }
                else if (subDot <= -0.5f)
                {
                    result = new Vector2(-1, 0);
                }
            }
            return result;
        }
        return Vector2.zero;
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
            if (e.IsHeadshot)
            {
                headshot.PlayHeadshotVFX();
            }
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
            else if (obj.GetType() == typeof(Stop))
            {
                if (animType != AnimType.Idle)
                    animType = AnimType.Idle;
            }
            Debug.Log(obj);
        }        
    }

    private void Enemy_OnEnemyAttack(object sender, OnEnemyAttackEventArg e)
    {
            
    }

    private void AOETargeted_OnAOESpellCast(object sender, OnAOESpellCastEventArg e)
    {
        spellCasted = e.spell;
        spellType = e.spellType;
    }
}
