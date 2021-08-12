using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Attack : State
{
    private EnemyBrain enemyBrain;
    private EnemyTrigger enemyTrigger;
    private bool IsAttacking;

    public Attack(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
        enemyTrigger = EB.GetComponentInChildren<EnemyTrigger>();
    }

    public override void Tick()
    {
        if (enemyTrigger.IsTargetInRange && !enemyTrigger.IsTargetFleed)
        {
            InitiateAttack();
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

    public override void OnEnter()
    {
        IsAttacking = false;
        enemyBrain.navMeshAgent.isStopped = true;
    }

    public override void OnExit()
    {
        IsAttacking = false;
        enemyBrain.navMeshAgent.isStopped = false;
    }

    private void InitiateAttack()
    {
        if (!IsAttacking && PlayerController.Instance.PlayerTarget != null)
        {
            if (!PlayerController.Instance.PlayerTarget.IsDead && !PlayerController.Instance.PlayerTarget.GetEnemy())
            {
                IsAttacking = true;
                enemyBrain.GetThisEnemy().DoAttack(PlayerController.Instance.PlayerTarget, PlayerController.Instance.DodgeChace);
                enemyBrain.GetThisEnemy().RaiseOnEnemyAttack(enemyBrain.GetEnemyType());
                enemyBrain.StartCoroutine(Attacking(() => { IsAttacking = false; }));
            }

        }
    }

    private IEnumerator Attacking (Action action)
    {
        if (IsAttacking)
        {
            yield return new WaitForSeconds(enemyBrain.GetThisEnemy().ThisEnemySO.AttackSpeed);
            action.Invoke();
        }
    }

}

