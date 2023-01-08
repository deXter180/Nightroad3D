using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGround : MeleeEnemy
{
    public MeleeGround(EnemyBrain EB) : base(EB)
    {

    }

    public override void HandleAttack(Target target, float dodgeChance)
    {
        DoAttack(target, dodgeChance);
        base.HandleAttack(target, dodgeChance);
    }

}
