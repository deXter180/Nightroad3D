using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prepare : State
{
    public Prepare(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {

    }

    public override void Tick()
    {
        enemy.CheckDistance();
        if (enemyBrain.IsFrozen)
        {
            stateMachine.SetState(AIStates.Stop);
        }
        else
        {
            if (!enemy.IsPrepDone)
            {
                enemy.DoPreparation();
            }
            else
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
        enemy.IsPrepDone = false;
        enemyBrain.navMeshAgent.enabled = true;
        enemyBrain.navMeshAgent.speed = enemyBrain.navMeshAgent.speed * enemySO.SpeedMultiplier;
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = false;
    }

    public override void OnExit()
    {
        enemy.IsPrepDone = false;
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = true;
        enemyBrain.navMeshAgent.speed = enemySO.MoveSpeed;
    }

}
