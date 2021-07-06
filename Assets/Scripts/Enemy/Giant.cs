using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Giant : Enemy
{
    public override EnemyTypes enemyTypes => EnemyTypes.Giant;
    public override int MaxHP => 5000;
    public override int DamageAmount => 50;
    public override float AttackSpeed => 2f;
    public override float AttackRange => 350f;
    public override float MoveSpeed => 20f; 
    public override float CritChance => 0.1f;
    public override float CritBonus => 0.2f;
    public override float DodgeChance => 0.5f;
    public override bool IsGroundUnit => true;
    public override float AvoidanceRadius => 2f;
    public Giant(EnemyBrain enemyBrain) : base(enemyBrain)
    {

    }
}
   