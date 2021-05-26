using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Giant : Enemy
{
    public override EnemyTypes enemyTypes => EnemyTypes.Giant;
    public override int MaxHP => 5000;
    public override int DamageAmount => 50;
    public override float AttackSpeed { get => throw new NotImplementedException(); }
    public override int AttackRange => throw new NotImplementedException();
    public override float MoveSpeed => 1.7f; 

    public override float CritChance => throw new NotImplementedException();

    public override float CritBonus => throw new NotImplementedException();

    public override float DodgeChance => 0.5f;

    public override bool IsGroundUnit => true;

    public Giant(EnemyBrain enemyBrain) : base(enemyBrain)
    {

    }
}
   