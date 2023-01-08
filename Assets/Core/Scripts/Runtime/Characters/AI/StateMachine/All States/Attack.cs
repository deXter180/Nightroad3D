using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Attack : State
{
    #region Variables

    private int currentAttackIndex;
    private int attackIndex;
    private float attackResetTime = 0; 
    private float prepTime = 0;
    private float delayTime;

    #endregion

    #region General

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
                    if (enemySO.IsPrepRequired)
                    {
                        if (attackIndex >= currentAttackIndex)
                        {
                            AttackStart();
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
                        AttackStart();
                    }
                }
                else
                {
                    var timeLeft = Time.time - attackResetTime;
                    if (timeLeft > 0.3f)
                    {
                        enemy.ResetAttackAnim();
                        enemy.RotateTowardsPlayer(20);
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
                if (!enemy.IsAttacking)
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
    }

    public override void OnEnter()
    {
        if (enemyBrain.navMeshAgent.isOnNavMesh)
        {
            enemyBrain.navMeshAgent.isStopped = true;
            enemyBrain.navMeshAgent.ResetPath();
        }
        enemyBrain.navMeshAgent.updatePosition = false;
        enemyBrain.navMeshAgent.enabled = false;
        attackIndex = UnityEngine.Random.Range(enemySO.MinAttackForPrep, enemySO.MaxAttackForPrep);
        currentAttackIndex = 0;
        enemy.PreAttack();
        enemy.CheckDistance();
    }

    public override void OnExit()
    {
        enemyBrain.navMeshAgent.enabled = true;
        enemyBrain.navMeshAgent.updatePosition = true;
        attackIndex = 0;
        currentAttackIndex = 0;
        prepTime = 0;
        enemy.PostAttack();
    }

    #endregion

    #region Mechanics

    private void AttackStart()
    {
        enemy.SetAttacking(true);
        if (currentAttackIndex == 0)
        {
            enemy.SetFirstAttackDone(false);
        }
        enemy.InitializeAttack(() =>
        {
            currentAttackIndex++;
            attackResetTime = Time.time;
        });
    }

    #endregion
}

