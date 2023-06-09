using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Attack : State
{
    private int currentAttackIndex;
    private int attackIndex;
    public Attack(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
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
            if (enemy.IsTargetInRange)
            {
                if (attackIndex > currentAttackIndex)
                {
                    enemy.InitializeAttack(() => { currentAttackIndex++; });
                }
                else
                {
                    stateMachine.SetState(AIStates.Prepare);
                }
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

    public override void OnEnter()
    {
        if (enemyBrain.navMeshAgent.isOnNavMesh)
        {
            enemyBrain.navMeshAgent.isStopped = true;
        }       
        enemy.PreAttack();
        attackIndex = UnityEngine.Random.Range(1, 5);
        currentAttackIndex = 0;
    }

    public override void OnExit()
    {
        enemy.PostAttack();
        attackIndex = 0;
        currentAttackIndex = 0;
    }

}

