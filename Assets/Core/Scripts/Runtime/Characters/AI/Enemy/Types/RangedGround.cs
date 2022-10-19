using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedGround : RangedEnemy
{
    public RangedGround(EnemyBrain EB) : base(EB)
    {

    }

    public override void Chase()
    {
        if (IsPlayerApproaching)
        {
            if (navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
            {
                navAgent.speed = defaultSpeed;
                navAgent.SetDestination(GetRandomPosition3(enemySO.AttackRange));
            }
        }
        else
        {
            base.Chase();
        }
    }

    protected override void Prepare()
    {
        targetPosition = GetRandomPosition1(enemySO.PostAttackMoveRange);
        base.Prepare();
    }
}
