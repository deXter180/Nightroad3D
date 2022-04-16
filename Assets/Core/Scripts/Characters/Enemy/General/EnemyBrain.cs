using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using System;

public class EnemyBrain : MonoBehaviour
{
    private string enemyID;
    private string enemyName;
    private int layer = 1 << 0;
    private bool isDying;
    private bool isSpellAffected;
    private bool isFrozen;
    private bool isMoving;
    private bool isAttacking;
    private Vector3 targetDir;
    private float angle;
    private int lastIndex;
    private bool readyForAnim = false;
    [SerializeField] private EnemyTypes enemyType;
    [SerializeField] private VisualEffect critHitVfx;
    [SerializeField] private VisualEffect bloodVfx;
    private Vector3 startPos;
    private Vector3 nextPos;
    private Enemy enemy;
    private AOETargeted spellCasted;
    private SpellTypes spellType;
    private StateMachine stateMachine;
    private EnemyTrigger enemyTrigger;
    private NavMeshAgent navAgent;    
    private BloodEffectOnHit bloodOnHit;
    private BloodEffectOnDeath bloodOnDeath;
    private Target enemyTarget;
    private Animator animator;
    private WaitForSeconds postDeathWait = new WaitForSeconds(2f);
    private TempShieldTrigger tempShield;
    public string EnemyID => enemyID; 
    public string EnemyName => enemyName;
    public Vector3 StartPos => startPos; 
    public Target EnemyTarget => enemyTarget; 
    public NavMeshAgent navMeshAgent => navAgent; 
    public SpellTypes SpellType => spellType; 
    public bool IsSpellAffected => isSpellAffected; 
    public bool IsFrozen  => isFrozen; 
    [HideInInspector] public bool IsSetupDone = false;
    private int angleHash = Animator.StringToHash("Angle");
    private int moveHash = Animator.StringToHash("IsMoving");
    private int attackHash = Animator.StringToHash("IsAttacking");
    private int deathHash = Animator.StringToHash("IsDead");    

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
            enemyTarget.OnCritShot -= EnemyTarget_OnCritShot;
        }
        Weapons.OnPlayerDamage -= Weapons_OnPlayerDamage;
        AOETargeted.OnAOESpellCast -= AOETargeted_OnAOESpellCast;
    }

    private void Update()
    {
        if (IsSetupDone)
        {
            stateMachine.Tick();
            if (navAgent.destination != nextPos)
            {
                nextPos = navAgent.destination;                           
            }
            CalculateAngle(new Vector3(nextPos.x, transform.position.y, nextPos.z));
            PlayingAnim();
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
        enemyName = enemy.ThisEnemySO.EnemyName;
        enemyTrigger = GetComponentInChildren<EnemyTrigger>();
        enemyTrigger.TriggerSetup(enemy);
        tempShield = GetComponentInChildren<TempShieldTrigger>();
        tempShield.SetShield(enemy.ThisEnemySO.ShieldSize, enemy.ThisEnemySO.ShieldDuration);
        navAgent = GetComponent<NavMeshAgent>();
        bloodOnHit = GetComponentInChildren<BloodEffectOnHit>();
        bloodOnDeath = GetComponentInChildren<BloodEffectOnDeath>();
        animator = GetComponentInChildren<Animator>();
        isDying = false;
        isMoving = false;
        isAttacking = false;
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
        enemyTarget.OnCritShot += EnemyTarget_OnCritShot;
        readyForAnim = true;
        IsSetupDone = true;
    }   

    private IEnumerator AfterKilled()
    {
        yield return postDeathWait;
        gameObject.SetActive(false);
        AssetCollections.ReleaseAssetInstance(gameObject, "Enemy", false);
    }

    //!!!!!!!!!!!!Call this where instantiating enemies!!!!!!!!!!!!!!!!

    public void SetupEnemyID(int count)
    {
        enemyID = $"{enemyType}{count}";
    }

    //~~~~~~~~~~~~~~~~~~~ Animation ~~~~~~~~~~~~~~~~~~~

    public void CalculateAngle(Vector3 targetPos)
    {
        targetDir = targetPos - transform.position;
        angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
        lastIndex = GetIndex(angle);
    }

    private int GetIndex(float _angl)
    {
        //front
        if (_angl <= -133 || _angl >= 133)
        {
            lastIndex = 0;
        }

        //Left
        else if (_angl >= -133 && _angl < -43)
        {
            lastIndex = 3;
        }

        //back
        else if (_angl >= -43 && _angl <= 43)
        {
            lastIndex = 2;
        }

        //Right
        else if (_angl > 43 && _angl <= 133)
        {
            lastIndex = 1;
        }

        return lastIndex;
    }

    private void PlayingAnim()
    {
        if (readyForAnim)
        {
            animator.SetFloat(angleHash, lastIndex);
            animator.SetBool(moveHash, isMoving);
            animator.SetBool(attackHash, isAttacking);
            animator.SetBool(deathHash, isDying);
        }       
    }

    //~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~

    private void EnemyTarget_OnDodge()
    {
        StartCoroutine(tempShield.TriggerShield(enemy.ThisEnemySO.ShieldDuration));
    }

    private void EnemyTarget_OnCritShot()
    {
        critHitVfx.Play();
    }

    private void Resource_OnKilled(object sender, EventArgs e)
    {
        if (e != null)
        {
            navAgent.isStopped = true;
            isDying = true;
            isMoving = false;
            isAttacking = false;
            StartCoroutine(AfterKilled());
        }        
    }

    private void Weapons_OnPlayerDamage(object sender, OnPlayerDamageEventArg e)
    {
        if (e.enemyBrain == this)
        {
            bloodOnHit.PlayBloodOnHit();
            bloodVfx.Play();
        }
    }

    private void StateMachine_OnStateChange(State obj)
    {
        if (obj.GetType() == typeof(Roam))
        {
            isMoving = true;
            isAttacking = false;
        }
        else if (obj.GetType() == typeof(Chase))
        {
            isMoving = true;
            isAttacking = false;
        }
        else if (obj.GetType() == typeof(Attack))
        {
            isMoving = false;
            isAttacking = true;
        }
        else if (obj.GetType() == typeof(Stop))
        {
            isAttacking = false;
            isMoving = false;
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
