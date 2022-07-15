using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : State
{

    public Stop(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        
    }

    public override void Tick()
    {
        enemy.CheckDistance();
        if (!enemyBrain.IsFrozen)
        {
            if (enemy.IsTargetInRange)
            {
                stateMachine.SetState(States.Attack);
            }
            else
            {
                if (enemyTrigger.IsTargetFleed)
                {
                    stateMachine.SetState(States.Roam);
                }
                else
                {
                    stateMachine.SetState(States.Chase);
                }
            }            
        }      
    }

    public override void OnEnter()
    {
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = true;
    }

    public override void OnExit()
    {
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = false;
    }

    
}
