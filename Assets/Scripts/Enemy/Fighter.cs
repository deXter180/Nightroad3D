using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Enemy
{
    public override EnemyTypes enemyTypes { get => EnemyTypes.Fighter; }
    public override int MaxHP { get => throw new System.NotImplementedException(); }
    public override int DamageAmount { get => throw new System.NotImplementedException(); }
    public override float AttackSpeed { get => throw new System.NotImplementedException(); }
    public override float AttackRange => throw new System.NotImplementedException();
    public override float MoveSpeed { get => throw new System.NotImplementedException(); }

    public override float CritChance => throw new System.NotImplementedException();

    public override float CritBonus => throw new System.NotImplementedException();

    public override float DodgeChance => 0.3f;

    public override bool IsGroundUnit => true;

    public override float AvoidanceRadius => 1f;

    public Fighter(EnemyBrain enemyBrain) : base(enemyBrain)
    {

    }
}
