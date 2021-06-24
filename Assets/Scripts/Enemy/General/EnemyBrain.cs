using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private EnemyTypes enemyType;
    private Vector3 startPos;
    private Enemy enemy;
    private StateMachine stateMachine;
    private SphereCollider sphereCollider;
    private BoxCollider hitBox;
    private NavMeshAgent navAgent;
    public Vector3 StartPos { get => startPos; }
    public NavMeshAgent navMeshAgent { get => navAgent; }
    public bool IsTargetFleed { get; private set; }
    public bool IsTargetInRange { get; private set; }
    private NavMeshPath meshPath;
    private int layer = 1 << 0;


    private void Awake()
    {
        SetEnemy();
        stateMachine = new StateMachine(this);
        sphereCollider = GetComponent<SphereCollider>();
        hitBox = GetComponentInChildren<BoxCollider>();
        navAgent = GetComponent<NavMeshAgent>();
        meshPath = new NavMeshPath();
    }

    private void OnEnable()
    {
        stateMachine.OnStateChange += StateMachine_OnStateChange;
        enemy.OnEnemyAttack += Enemy_OnEnemyAttack;
        sphereCollider.isTrigger = true;
        startPos = transform.position;
        IsTargetFleed = true;
        IsTargetInRange = false;
        navAgent.speed = enemy.MoveSpeed;
    }

    private void OnDisable()
    {
        stateMachine.OnStateChange -= StateMachine_OnStateChange;
        enemy.OnEnemyAttack -= Enemy_OnEnemyAttack;
    }

    private void Update()
    {
        stateMachine.Tick();
    }

    public Enemy GetThisEnemy()
    {
        return enemy;
    }

    public EnemyTypes GetEnemyType()
    {
        return enemyType;
    }

    private Enemy SetEnemy()
    {
        switch (enemyType)
        {
            case EnemyTypes.Giant:
                enemy = new Giant(this);
                break;
            case EnemyTypes.Fighter:
                enemy = new Fighter(this);
                break;
        }
        return enemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IsTargetFleed = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Vector3.Distance(transform.position, other.transform.position) <= enemy.AttackRange)
            {
                IsTargetInRange = true;
            }
            else
            {
                IsTargetInRange = false;
            }
        }
    }

    private IEnumerator OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            yield return new WaitForSeconds(3f);
            IsTargetInRange = false;
            IsTargetFleed = true;
        }
    }

    private void StateMachine_OnStateChange(State obj)
    {
        Debug.Log(obj);
    }

    private void Enemy_OnEnemyAttack(object sender, OnEnemyAttackEventArg e)
    {
        
    }
}
