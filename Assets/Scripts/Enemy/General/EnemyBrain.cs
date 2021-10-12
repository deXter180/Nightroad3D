using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using System;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private EnemyTypes enemyType;
    private Vector3 startPos;
    private Enemy enemy;
    private StateMachine stateMachine;
    private EnemyTrigger enemyTrigger;
    private NavMeshAgent navAgent;
    private VisualEffect vfxGraph;
    private Target enemyTarget;
    private TempShieldTrigger tempShield;
    public Vector3 StartPos { get => startPos; }
    public NavMeshAgent navMeshAgent { get => navAgent; }  
    [HideInInspector] public bool IsSetupDone = false;
    private int layer = 1 << 0;


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
        }
        Weapons.OnPlayerDamage -= Weapons_OnPlayerDamage;
    }

    private void Update()
    {
        if (IsSetupDone)
        {
            stateMachine.Tick();
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
        stateMachine = new StateMachine(this);
        navAgent.speed = enemy.ThisEnemySO.MoveSpeed;
        stateMachine.OnStateChange += StateMachine_OnStateChange;
        startPos = transform.position;
        enemyTarget = GetComponent<Target>();
        enemyTarget.SetupMaxHP(enemy.ThisEnemySO.MaxHP);
        enemyTarget.SetEB(this);
        enemyTarget.OnDodge += EnemyTarget_OnDodge;
        IsSetupDone = true;
    }

    //~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~

    private void EnemyTarget_OnDodge()
    {
        StartCoroutine(tempShield.TriggerShield(enemy.ThisEnemySO.ShieldDuration));
    }

    private void Weapons_OnPlayerDamage(object sender, OnPlayerDamageEventArg e)
    {
        if (e.enemyBrain == this)
        {
            vfxGraph.Play();
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
