using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chase : State
{
    private EnemyBrain enemyBrain;

    public Chase(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
    }

    public override void Tick()
    {
        if (enemyBrain.IsTargetInRange == false && enemyBrain.IsTargetFleed == false)
        {
            ChaseTarget();
        }
        else if (enemyBrain.IsTargetInRange)
        {
            stateMachine.SetState(States.Attack);
        }
        else if (enemyBrain.IsTargetFleed)
        {
            stateMachine.SetState(States.Roam);
        }
    }

    public override void OnEnter()
    {
        enemyBrain.navMeshAgent.destination = PlayerController.Instance.transform.position;
        enemyBrain.navMeshAgent.isStopped = false;
    }

    public override void OnExit()
    {
        enemyBrain.navMeshAgent.isStopped = true;
    }

    private void ChaseTarget()
    {
        enemyBrain.navMeshAgent.SetDestination(PlayerController.Instance.transform.position);
    }

}
