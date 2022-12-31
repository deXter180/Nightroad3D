using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : State
{
    public Stop(EnemyBrain EB, StateMachine SM, AIStates state) : base(EB.gameObject, SM, state)
    {
        
    }

    public override void Tick()
    {
        if (!enemyBrain.IsDead)
        {
            enemy.CheckDistance();
            if (!enemyBrain.IsFrozen)
            {
                if (enemy.IsTargetInRange)
                {
                    stateMachine.SetState(AIStates.Attack);
                }
                else
                {
                    if (enemyTrigger.IsTargetFleed)
                    {
                        stateMachine.SetState(AIStates.Roam);
                    }
                    else
                    {
                        stateMachine.SetState(AIStates.Chase);
                    }
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
