using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chase : State
{
    private float speedMult;   

    public Chase(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
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
            if (enemy.IsTargetInRange)
            {
                stateMachine.SetState(States.Attack);
            }
            else
            {
                if (!enemyTrigger.IsTargetFleed)
                {
                    enemy.Chase();                   
                }
                else
                {
                    stateMachine.SetState(States.Roam);
                }
            }                      
        }        
    }

    public override void OnEnter()
    {
        enemyBrain.navMeshAgent.enabled = true;
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = false;
    }

    public override void OnExit()
    {
        enemyBrain.navMeshAgent.speed = enemySO.MoveSpeed;
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = true;
    }

}
