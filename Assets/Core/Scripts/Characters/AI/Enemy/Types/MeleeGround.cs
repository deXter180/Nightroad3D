using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGround : MeleeEnemy
{
    public MeleeGround(EnemyBrain EB) : base(EB)
    {

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
