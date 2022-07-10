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
            stateMachine.SetState(States.Stop);
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
    }

    public override void OnEnter()
    {
        enemy.IsPrepDone = false;
        enemyBrain.navMeshAgent.enabled = true;
        enemyBrain.navMeshAgent.speed = enemyBrain.navMeshAgent.speed * enemySO.SpeedMultiplier;
        enemyBrain.navMeshAgent.isStopped = false;
    }

    public override void OnExit()
    {
        enemy.IsPrepDone = false;
        enemyBrain.navMeshAgent.isStopped = true;
        enemyBrain.navMeshAgent.speed = enemySO.MoveSpeed;
    }

}
