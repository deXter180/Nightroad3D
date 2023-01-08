using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCore : MonoBehaviour
{
    #region Variables

    protected string enemyID;
    protected string enemyName;
    protected Rigidbody rb;
    protected EnemyTrigger enemyTrigger;
    protected NavMeshAgent navAgent;
    protected Target enemyTarget;
    protected Animator animator;    
    protected TempShieldTrigger tempShield;

    #endregion

    #region Properties

    public string EnemyID => enemyID;
    public string EnemyName => enemyName;
    public Target EnemyTarget => enemyTarget;
    public Animator EnemyAnimator => animator;
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
        navAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        tempShield = GetComponentInChildren<TempShieldTrigger>();
        enemyTarget = GetComponent<Target>();
        enemyTrigger = GetComponentInChildren<EnemyTrigger>();
    }

    #endregion
}
