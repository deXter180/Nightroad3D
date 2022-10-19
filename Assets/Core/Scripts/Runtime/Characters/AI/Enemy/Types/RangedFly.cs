using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedFly : RangedEnemy
{
    public RangedFly(EnemyBrain EB) : base(EB)
    {

    }

    protected override void Prepare()
    {
        targetPosition = GetRandomPosition1(enemySO.PostAttackMoveRange);
        base.Prepare();
    }
}
