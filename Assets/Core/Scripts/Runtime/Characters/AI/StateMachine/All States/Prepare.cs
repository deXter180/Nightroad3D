using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Prepare : State
{

    public Prepare(EnemyBrain EB, StateMachine SM, AIStates state) : base(EB.gameObject, SM, state)
    {

    }

    public override void Tick()
    {       
        if (enemyBrain.IsFrozen)
        {
            stateMachine.SetState(AIStates.Stop);
        }
        else
        {
            if (!enemy.IsPrepDone)
            {
                if (!enemyTrigger.IsTargetFleed)
                {
                    enemy.DoPreparation();
                }
                else
                {
                    stateMachine.SetState(AIStates.Chase);
                }
            }                        
            else
            {
                enemy.CheckDistance();
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
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = true;
        enemyBrain.navMeshAgent.ResetPath();
        stateSpeed = enemyBrain.navMeshAgent.speed;
        enemyBrain.navMeshAgent.speed = (float)Math.Round(stateSpeed * 0.7f, 1);
        enemyBrain.navMeshAgent.enabled = true;
    }

    public override void OnExit()
    {
        enemy.IsPrepDone = false;        
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = true;
        enemyBrain.navMeshAgent.ResetPath();
        enemyBrain.SetInBattle(false);
        enemyBrain.navMeshAgent.speed = enemySO.MoveSpeed;
    }

}
