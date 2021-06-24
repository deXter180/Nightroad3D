using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Attack : State
{
    private EnemyBrain enemyBrain;
    private bool IsAttacking;

    public Attack(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
    }

    public override void Tick()
    {
        if (enemyBrain.IsTargetInRange)
        {
            InitiateAttack();
        }
        else
        {
            stateMachine.SetState(States.Chase);
        }
    }

    public override void OnEnter()
    {
        IsAttacking = false;
    }

    public override void OnExit()
    {
        IsAttacking = false;
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
            yield return new WaitForSeconds(enemyBrain.GetThisEnemy().AttackSpeed);
            action.Invoke();
        }
    }

}

