using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chase : State
{

    public Chase(EnemyBrain EB, StateMachine SM, AIStates state) : base(EB.gameObject, SM, state)
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
            enemy.CheckDistance();
            if (enemy.IsTargetInRange)
            {
                stateMachine.SetState(AIStates.Attack);
            }
            else
            {
                if (!enemyTrigger.IsTargetFleed)
                {
                    enemy.Chase();
                }
                else
                {
                    stateMachine.SetState(AIStates.Roam);
                }
            }                      
        }        
    }

    public override void OnEnter()
    {
        enemyBrain.navMeshAgent.enabled = true;        
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = true;
        stateSpeed = enemyBrain.navMeshAgent.speed;
        var mult = stateSpeed * enemySO.SpeedMultiplier;
        stateSpeed += mult;
        enemyBrain.navMeshAgent.speed = stateSpeed;
        enemyBrain.navMeshAgent.ResetPath();
    }

    public override void OnExit()
    {        
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = true;
        enemyBrain.navMeshAgent.speed = enemySO.MoveSpeed;
        enemyBrain.navMeshAgent.ResetPath();
    }

}
