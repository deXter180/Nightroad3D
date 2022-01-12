using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : State
{
    private EnemyBrain enemyBrain;
    private EnemyTrigger enemyTrigger;

    public Stop(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
        enemyTrigger = EB.GetComponentInChildren<EnemyTrigger>();
    }

    public override void Tick()
    {
        if (!enemyBrain.IsFrozen)
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
                stateMachine.SetState(States.Chase);
            }
        }      
    }

    public override void OnEnter()
    {
        enemyBrain.navMeshAgent.isStopped = true;
    }

    public override void OnExit()
    {
        enemyBrain.navMeshAgent.isStopped = false;
    }

    
}
