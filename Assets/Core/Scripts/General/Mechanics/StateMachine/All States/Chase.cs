using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chase : State
{
    private EnemyBrain enemyBrain;
    private EnemyTrigger enemyTrigger;
    private float speedMult;   

    public Chase(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
        enemyTrigger = EB.GetComponentInChildren<EnemyTrigger>();
    }

    public override void Tick()
    {
        if (enemyBrain.IsFrozen)
        {
            stateMachine.SetState(States.Stop);
        }
        else
        {
            if (enemyTrigger.IsTargetFleed && !enemyTrigger.IsTargetInRange)
            {
                stateMachine.SetState(States.Roam);
            }
            else if (enemyTrigger.IsTargetInRange && !enemyTrigger.IsTargetFleed)
            {
                stateMachine.SetState(States.Attack);
            }
            else if (!enemyTrigger.IsTargetInRange && !enemyTrigger.IsTargetFleed)
            {
                ChaseTarget();
            }
        }        
    }

    public override void OnEnter()
    {
        enemyBrain.navMeshAgent.isStopped = false;       
    }

    public override void OnExit()
    {
        enemyBrain.navMeshAgent.speed = enemyBrain.GetThisEnemy().ThisEnemySO.MoveSpeed;
        enemyBrain.navMeshAgent.isStopped = true;
    }

    private void ChaseTarget()
    {
        if (enemyTrigger.IsSlowed)
        {
            enemyBrain.navMeshAgent.speed = enemyBrain.GetThisEnemy().ThisEnemySO.MoveSpeed;
        }
        else
        {
            enemyBrain.navMeshAgent.speed = enemyBrain.navMeshAgent.speed * enemyBrain.GetThisEnemy().ThisEnemySO.SpeedMultiplier;           
        }
        enemyBrain.navMeshAgent.SetDestination(PlayerController.Instance.transform.position);
    }

}
