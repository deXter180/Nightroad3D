using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System;

public class EnemyBrain : EnemyCore
{
    #region Variables

    private int layer = 1 << 0;
    private bool isInBattle = false;
    private bool isDead = false;
    private bool isCritHit = false;
    private bool isAttack1 = false;
    private bool isAttack2 = false;
    private bool isAttack3 = false;
    private bool isAttack4 = false;
    private bool isSpellAffected = false;
    private bool isFrozen = false;
    private bool isDamaged = false;
    private Vector3 targetDir;
    private float angle;
    private float lighteningRestruckDelay = 5;
    private float elaplsedTime;
    private float velocityX;
    private float velocityZ;
    private float attack1Index;
    private bool readyForAnim = false;
    private Vector3 startPos;
    private Collider bodyCol;
    private Rigidbody rb;
    private Enemy enemy;
    private EnemySO enemySO;
    private AOETargeted spellCasted;
    //private EnemyLOSManager enemyLOS;
    private SpellTypes spellType;
    private StateMachine stateMachine;
    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRBs;
    private CharacterJoint[] ragdollJoints;
    [HideInInspector] public bool IsSetupDone = false;

    #endregion

    #region SerializedVariables

    [Tooltip("Enter unique ID for each Enemy")] [SerializeField] private string uniqueID;
    [SerializeField] private EnemyTypes enemyType;
    [SerializeField] private VisualEffect critHitVfx;
    [Tooltip("Only for Ranged")] public Transform FirepointTransform;
    [SerializeField] private Transform armatureTransform;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Collider headCol;
    
    #endregion

    #region AnimHash

    private int inBattleHash = Animator.StringToHash("IsInBattle");
    private int velocityXHash = Animator.StringToHash("VelocityX");
    private int velocityZHash = Animator.StringToHash("VelocityZ");
    private int moveIndexHash = Animator.StringToHash("MoveIndex");
    private int attack1IndexHash = Animator.StringToHash("Attack1Index");
    private int attack1Hash = Animator.StringToHash("IsAttack1");
    private int attack2Hash = Animator.StringToHash("IsAttack2");
    private int attack3Hash = Animator.StringToHash("IsAttack3");
    private int attack4Hash = Animator.StringToHash("IsAttack4");
    private int critHash = Animator.StringToHash("IsCritHit");

    #endregion

    #region Properties

    public Vector3 StartPos => startPos;
    public EnemySO ThisEnemySO => enemySO;
    public StateMachine ThisStateMachine => stateMachine;
    public SpellTypes SpellType => spellType;
    public bool IsHitByLightening { get; private set; }
    public bool IsSpellAffected => isSpellAffected;
    public bool IsDead => isDead;
    public bool IsFrozen => isFrozen;
    public bool IsDamaged => isDamaged;
    public bool IsAttack1 => isAttack1;
    public bool IsAttack2 => isAttack2;
    public bool IsAttack3 => isAttack3;
    public bool IsAttack4 => isAttack4;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        bodyCol = bodyTransform.GetComponent<Collider>();
        rb = bodyTransform.GetComponent<Rigidbody>();
        ragdollColliders = armatureTransform.GetComponentsInChildren<Collider>();
        ragdollRBs = armatureTransform.GetComponentsInChildren<Rigidbody>();
        ragdollJoints = armatureTransform.GetComponentsInChildren<CharacterJoint>();
    }

    private void Start()
    {
        StartCoroutine(SetEnemy());
        EnableAnimator();
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
            if (isDamaged || isCritHit)
            {
                StartCoroutine(ResetDamaged());
            }
            stateMachine.Tick();
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
        
        IEnumerator ResetDamaged()
        {
            yield return Helpers.GetWait(0.4f);
            isDamaged = false;
            isCritHit = false;
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

    private IEnumerator SetEnemy()
    {
        EnemyType = enemyType;
        yield return Helpers.GetWait(0.5f);
        enemySO = AssetLoader.GetEnemySOFromList(enemyType);
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
        //enemyLOS = FindObjectOfType<EnemyLOSManager>();
        enemy.OnEnemyAttack += Enemy_OnEnemyAttack;
        Weapons.OnPlayerDamage += Weapons_OnPlayerDamage;
        AOETargeted.OnAOESpellCast += AOETargeted_OnAOESpellCast;
        enemyTrigger.TriggerSetup();        
        elaplsedTime = 0;
        IsHitByLightening = false;
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
        enemyName = enemySO.EnemyName;
        EnemyDodgeChance = enemySO.DodgeChance;
        SetupEnemyID(uniqueID);     
        IsSetupDone = true;
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~ Animation ~~~~~~~~~~~~~~~~~~~

    #region AnimationControl

    public void SetLighteningHit()
    {
        IsHitByLightening = true;
    }

    public void SetInBattle(bool istrue)
    {
        isInBattle = istrue;
    }

    public void SetAttack1(bool isTrue)
    {
        isAttack1 = isTrue;
    }

    public void SetAttack2(bool isTrue)
    {
        isAttack2 = isTrue;
    }

    public void SetAttack3(bool isTrue)
    {
        isAttack3 = isTrue;
    }

    public void SetAttack4(bool isTrue)
    {
        isAttack4 = isTrue;
    }

    public void SetAttack1Index()
    {
        attack1Index = UnityEngine.Random.Range(0f, 1f);
    }

    public void ResetAttack()
    {
        isAttack1 = false;
        isAttack2 = false;
        isAttack3 = false;
        isAttack4 = false;
    }

    public void CalculateAngle(Vector3 targetPos)
    {
        targetDir = targetPos - transform.position;
        targetDir.y = 0;
        var forward = (float)Math.Round(Vector3.Dot(targetDir.normalized, transform.forward), 2);
        var right = (float)Math.Round(Vector3.Dot(targetDir.normalized, transform.right), 2);
        velocityX = right;
        velocityZ = -forward;
    }

    private void PlayingAnim()
    {
        if (readyForAnim)
        {          
            var speedParameter = (float) Math.Round(navAgent.velocity.sqrMagnitude / navAgent.speed, 1);
            animator.SetFloat(moveIndexHash, speedParameter);
            animator.SetFloat(velocityXHash, velocityX);
            animator.SetFloat(velocityZHash, velocityZ);
            animator.SetBool(inBattleHash, isInBattle);
            animator.SetFloat(attack1IndexHash, attack1Index);
            animator.SetBool(attack1Hash, isAttack1);
            animator.SetBool(attack2Hash, isAttack2);
            animator.SetBool(attack3Hash, isAttack3);
            animator.SetBool(attack4Hash, isAttack4);
            animator.SetBool(critHash, isCritHit);
        }       
    }

    private void EnableRagdoll()
    {
        Destroy(bodyTransform.gameObject);
        Destroy(headCol.gameObject);
        animator.enabled = false;
        foreach (var joint in ragdollJoints)
        {
            joint.enableCollision = true;
        }
        foreach (var rb in ragdollRBs)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.freezeRotation = false;
            rb.detectCollisions = true;
            rb.useGravity = true;
        }
        foreach (var col in ragdollColliders)
        {
            col.enabled = true;
        }
    }

    private void EnableAnimator()
    {
        animator.enabled = true;
        rb.isKinematic = true;
        rb.detectCollisions = true;
        bodyCol.enabled = true;
        headCol.enabled = true;
        foreach (var joint in ragdollJoints)
        {
            joint.enableCollision = false;
        }
        foreach (var rb in ragdollRBs)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.freezeRotation = true;
            rb.detectCollisions = false;
            rb.useGravity = false;           
        }
        foreach (var col in ragdollColliders)
        {
            col.enabled = false;
        }
    }

    #endregion

    //~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void EnemyTarget_OnDodge()
    {
        isDamaged = false;
    }

    private void EnemyTarget_OnCritShot()
    {
        critHitVfx.Play();
        isCritHit = true;
    }

    private void Resource_OnHealthLoss(object sender, ResourceManagement.DamagedEventArgs e)
    {
        isDamaged = true;
    }

    private void Resource_OnKilled(object sender, EventArgs e)
    {
        if (e != null)
        {
            isDead = true;
            stateMachine.SetState(AIStates.Stop);
            animator.SetLayerWeight(1, 0);           
            isDamaged = false; 
            isCritHit = false;
            EnableRagdoll();
            //if (enemyLOS != null)
            //{
            //    enemyLOS.RemoveEnemy(this);
            //}
            StartCoroutine(AfterKilled());
            IEnumerator AfterKilled()
            {
                yield return Helpers.GetWait(5);
                gameObject.SetActive(false);
                AssetLoader.ReleaseAssetInstance(gameObject);
            }
        }        
    }

    private void Weapons_OnPlayerDamage(object sender, OnPlayerDamageEventArg e)
    {
        
    }

    private void StateMachine_OnStateChange(State currentState, State previousState)
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

    #endregion
}
