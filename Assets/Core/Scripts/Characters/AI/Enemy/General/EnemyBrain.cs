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
    private Vector3 targetDir;
    private float angle;
    private float lighteningRestruckDelay = 5;
    private float elaplsedTime;
    private int lastIndex;
    private bool readyForAnim = false;
    [Tooltip("Enter unique ID for each Enemy")] [SerializeField] private string uniqueID;
    [SerializeField] private EnemyTypes enemyType;
    [SerializeField] private VisualEffect critHitVfx;
    [SerializeField] private VisualEffect bloodVfx;
    [Tooltip("Only for Ranged")] public Transform Firepoint;
    private Vector3 startPos;
    private Vector3 nextPos;
    private Enemy enemy;
    private EnemySO enemySO;
    private EquipMenuControl menuControl;
    private AOETargeted spellCasted;
    private SpellTypes spellType;
    private StateMachine stateMachine;
    private EnemyTrigger enemyTrigger;
    private NavMeshAgent navAgent;    
    private BloodEffectOnHit bloodOnHit;
    private BloodEffectOnDeath bloodOnDeath;
    private Target enemyTarget;
    private Animator animator;
    private Rigidbody RB;
    private TempShieldTrigger tempShield;
    public string EnemyID => enemyID; 
    public string EnemyName => enemyName;
    public Vector3 StartPos => startPos;
    public Transform EnemyTransform { get; private set; }
    public EnemySO ThisEnemySO => enemySO;
    public Target EnemyTarget => enemyTarget;
    public Animator EnemyAnimator => animator;
    public Rigidbody EnemyRigidbody => RB;
    public EquipMenuControl MenuControl => menuControl;
    public NavMeshAgent navMeshAgent => navAgent;
    public EnemyTrigger Trigger => enemyTrigger;
    public SpellTypes SpellType => spellType; 
    public bool IsHitByLightening { get; private set; }
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
            enemyTarget.Resource.OnHealthLoss -= Resource_OnHealthLoss;
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
            if (navAgent.isOnNavMesh)
            {
                if (navAgent.velocity.sqrMagnitude == 0)
                {
                    isMoving = false;
                }
                else
                {
                    isMoving = true;
                }
            }
            PlayingAnim();
            if (IsHitByLightening)
            {
                elaplsedTime += Time.deltaTime;
                if (elaplsedTime >= lighteningRestruckDelay)
                {
                    IsHitByLightening = false;
                    elaplsedTime = 0;
                }
            }              
        }         
    }

//#if UNITY_EDITOR

//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.cyan;
//        Gizmos.DrawSphere(transform.position, enemySO.AttackRange);
//    }

//#endif

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
        EnemyTransform = transform;
        RB = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();
        bloodOnHit = GetComponentInChildren<BloodEffectOnHit>();
        bloodOnDeath = GetComponentInChildren<BloodEffectOnDeath>();
        animator = GetComponentInChildren<Animator>();
        tempShield = GetComponentInChildren<TempShieldTrigger>();
        enemyTarget = GetComponent<Target>();
        enemyTrigger = GetComponentInChildren<EnemyTrigger>();
        yield return Helpers.GetWait(1);
        enemySO = GameController.GetEnemySOFromList(enemyType);
        if (enemySO.IsRanged)
        {
            if (enemySO.IsGroundUnit)
            {
                enemy = new RangedGround(this);
            }
            else
            {
                enemy = new RangedFly(this);
            }
        }
        else
        {
            if (enemySO.IsGroundUnit)
            {
                enemy = new MeleeGround(this);
            }
            else
            {
                enemy = new MeleeFly(this);
            }
        }
        enemy.OnEnemyAttack += Enemy_OnEnemyAttack;
        Weapons.OnPlayerDamage += Weapons_OnPlayerDamage;
        AOETargeted.OnAOESpellCast += AOETargeted_OnAOESpellCast;
        enemyName = enemySO.EnemyName;
        enemyTrigger.TriggerSetup();        
        tempShield.SetShield(enemySO.ShieldSize, enemySO.ShieldDuration);
        menuControl = EquipMenuControl.Instance;
        elaplsedTime = 0;
        IsHitByLightening = false;
        isDying = false;
        isMoving = false;
        isSpellAffected = false;
        isFrozen = false;
        stateMachine = new StateMachine(this);
        navAgent.speed = enemySO.MoveSpeed;
        stateMachine.OnStateChange += StateMachine_OnStateChange;
        startPos = transform.position;        
        enemyTarget.SetupMaxHP(enemySO.MaxHP);
        enemyTarget.SetEB(this);        
        enemyTarget.OnDodge += EnemyTarget_OnDodge;
        enemyTarget.Resource.OnHealthLoss += Resource_OnHealthLoss;
        enemyTarget.Resource.OnKilled += Resource_OnKilled;
        enemyTarget.OnCritShot += EnemyTarget_OnCritShot;
        readyForAnim = true;
        SetupEnemyID();
        IsSetupDone = true;
    }

    public void SetLighteningHit()
    {
        IsHitByLightening = true;
    }

    //!!!!!!!!!!!!Call this where instantiating enemies!!!!!!!!!!!!!!!!

    public void SetupEnemyID()
    {
        enemyID = $"{enemyType}{uniqueID}";
    }

    //~~~~~~~~~~~~~~~~~~~ Animation ~~~~~~~~~~~~~~~~~~~

    public void SetAnimIndex(int index)
    {
        lastIndex = index;
    }

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
            if (!enemy.IsAttacking)
            {
                lastIndex = 2;
            }
            else
            {
                lastIndex = 0;
            }
            
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
            animator.SetBool(attackHash, enemy.IsAttacking);
            animator.SetBool(deathHash, isDying);
        }       
    }

    //~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~

    private void EnemyTarget_OnDodge()
    {
        StartCoroutine(tempShield.TriggerShield(enemySO.ShieldDuration));
    }

    private void EnemyTarget_OnCritShot()
    {
        critHitVfx.Play();
    }

    private void Resource_OnHealthLoss(object sender, ResourceManagement.DamagedEventArgs e)
    {
        bloodOnHit.PlayBloodOnHit();
        bloodVfx.Play();
    }

    private void Resource_OnKilled(object sender, EventArgs e)
    {
        if (e != null)
        {
            bloodOnDeath.PlayBloodOnDeath();
            navAgent.isStopped = true;
            isDying = true;
            isMoving = false;
            StartCoroutine(AfterKilled());
            IEnumerator AfterKilled()
            {
                yield return Helpers.GetWait(2);
                gameObject.SetActive(false);
                AssetLoader.ReleaseAssetInstance(gameObject);
            }
        }        
    }

    private void Weapons_OnPlayerDamage(object sender, OnPlayerDamageEventArg e)
    {
        
    }

    private void StateMachine_OnStateChange(State obj)
    {
            
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
