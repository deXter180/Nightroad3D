using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGround : MeleeEnemy
{
    public MeleeGround(EnemyBrain EB) : base(EB)
    {

    }

    protected override void SetAttackType()
    {        
        float dist = GetDistanceFromPlayer();
        if (enemyBrain.ThisStateMachine.GetPreviousState().GetAIState() == AIStates.Chase)
        {
            attackType = AttackTypes.Attack2;
            enemyBrain.SetAttack2(true);
        }
        else
        {
            if (dist < enemySO.AttackRange * 0.8f)
            {
                attackType = AttackTypes.Attack1;
                enemyBrain.SetAttack1Index();
                enemyBrain.SetAttack1(true);
            }
        }
        base.SetAttackType();
    }

    protected override void Prepare()
    {
        targetPosition = GetRandomPosition2(enemySO.PostAttackMoveRange);
        base.Prepare();
    }

    public override void HandleAttack(Target target, float dodgeChance)
    {
        DoAttack(target, dodgeChance);
        base.HandleAttack(target, dodgeChance);
    }

}
