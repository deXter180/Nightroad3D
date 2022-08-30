using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCore : MonoBehaviour
{
    #region Variables

    protected string enemyID;
    protected string enemyName;
    protected EnemyTrigger enemyTrigger;
    protected NavMeshAgent navAgent;
    protected BloodEffectOnHit bloodOnHit;
    protected BloodEffectOnDeath bloodOnDeath;
    protected Target enemyTarget;
    protected Animator animator;
    protected Rigidbody RB;
    protected TempShieldTrigger tempShield;

    #endregion

    #region Properties

    public string EnemyID => enemyID;
    public string EnemyName => enemyName;
    public Target EnemyTarget => enemyTarget;
    public Animator EnemyAnimator => animator;
    public Rigidbody EnemyRigidbody => RB;
    public NavMeshAgent navMeshAgent => navAgent;
    public EnemyTrigger Trigger => enemyTrigger;
    public EnemyTypes EnemyType { get; protected set; } //Need to Assign
    public Transform EnemyTransform { get; private set; }
    public float EnemyDodgeChance { get; protected set; } //Need to Assign

    #endregion

    #region General

    protected virtual void Awake()
    {
        CoreSetup();
    }

    //!!!!!!!!!!!!Call this where instantiating enemies or from enemy script!!!!!!!!!!!!!!!!
    public void SetupEnemyID(string uniqueID)
    {
        enemyID = $"{EnemyType}{uniqueID}";
    }

    public virtual void CoreSetup()
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
    }

    #endregion
}
