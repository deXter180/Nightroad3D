using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGroundTank : MeleeGround
{
    private float chaseSlowRange = 0;

    public MeleeGroundTank(EnemyBrain EB) : base(EB)
    {

    }

    protected override void SetAttackType()
    {
        float dist = GetDistanceFromPlayer();
        var randomIndex = Random.Range(0, 2);
        if (!isFirstAttackDone)
        {
            if (randomIndex == 0)
            {
                AttackType = AttackTypes.Attack3;
                enemyBrain.SetAttack3(true);
            }
            else
            {
                AttackType = AttackTypes.Attack2;
                enemyBrain.SetAttack2(true);
            }
            isFirstAttackDone = true;
        }
        else
        {

            if (GetDistanceFromPlayer() < enemySO.AttackRange * 0.7f)
            {
                if (GetDistanceFromPlayer() < enemySO.AttackRange * 0.5f)
                {
                    AttackType = AttackTypes.Attack1;
                    enemyBrain.SetAttack1Index();
                    enemyBrain.SetAttack1(true);
                }
                else
                {
                    AttackType = AttackTypes.Attack4;
                    enemyBrain.SetAttack4(true);
                }

            }
            else
            {
                if (randomIndex == 1)
                {
                    AttackType = AttackTypes.Attack3;
                    enemyBrain.SetAttack3(true);
                }
                else
                {
                    AttackType = AttackTypes.Attack2;
                    enemyBrain.SetAttack2(true);
                }
            }
        }
        base.SetAttackType();
    }

    public override void Chase()
    {
        if (chaseSlowRange == 0)
        {
            chaseSlowRange = enemySO.AttackRange + (enemySO.AttackRange * 1.2f);
        }
        if (GetDistanceFromPlayer() <= chaseSlowRange)
        {
            navAgent.speed = GetPctSpeed(enemySO.MoveSpeed, 0.4f);
        }
        base.Chase();
    }

    protected override void Prepare()
    {
        targetPosition = GetRandomXAxisPosPrepare(enemySO.PostAttackMoveRange, 1.5f);
        base.Prepare();
    }
}
