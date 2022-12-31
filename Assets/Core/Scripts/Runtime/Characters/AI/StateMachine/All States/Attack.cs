using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Attack : State
{
    private int currentAttackIndex;
    private int attackIndex;
    private float attackResetTime = 0; 
    private float prepTime = 0;
    private float delayTime;

    public Attack(EnemyBrain EB, StateMachine SM, AIStates state) : base(EB.gameObject, SM, state)
    {
        delayTime = EB.ThisEnemySO.AttackDelay;
    }

    public override void Tick()
    {
        if (enemyBrain.IsFrozen)
        {
            stateMachine.SetState(AIStates.Stop);
        }
        else
        {
            if (enemy.IsTargetInRange)
            {
                if (!enemy.IsAttacking)
                {
                    if (attackIndex >= currentAttackIndex)
                    {                        
                        enemy.SetAttacking(true);
                        enemy.InitializeAttack(() =>
                        {
                            currentAttackIndex++;
                            attackResetTime = Time.time;
                            enemy.CheckDistance();
                        });
                    }
                    else
                    {
                        //Prepare Transition
                        prepTime += Time.fixedDeltaTime;
                        if (prepTime > 3f)
                        {
                            enemy.SetAttacking(false);                          
                            stateMachine.SetState(AIStates.Prepare);
                        }
                    }
                }
                else
                {
                    var timeLeft = Time.time - attackResetTime;
                    if (timeLeft > 0.3f)
                    {
                        enemy.ResetAttackAnim();
                        if (timeLeft >= delayTime)
                        {
                            attackResetTime = 0;                            
                            enemy.SetAttacking(false);
                            enemy.CheckDistance();
                        }
                    }                    
                }
            }
            else
            {
                
                //Roam & Chase Transition
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
            enemyBrain.navMeshAgent.ResetPath();
        }          
        attackIndex = UnityEngine.Random.Range(1, 5);
        currentAttackIndex = 0;
        enemy.PreAttack();
        enemy.CheckDistance();
    }

    public override void OnExit()
    {        
        
        if (enemyBrain.navMeshAgent.isOnNavMesh)
        {
            enemyBrain.navMeshAgent.isStopped = true;
            enemyBrain.navMeshAgent.ResetPath();
        }        
        attackIndex = 0;
        currentAttackIndex = 0;
        prepTime = 0;
        enemy.PostAttack();
    }

}

