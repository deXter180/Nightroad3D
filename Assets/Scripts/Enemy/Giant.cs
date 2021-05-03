using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Giant : Enemy
{
    public override EnemyTypes enemyTypes => EnemyTypes.Giant;
    public override int MaxHP => 500;
    public override int DamageAmount => 50;
    public override float AttackSpeed { get => throw new NotImplementedException(); }
    public override int AttackRange => throw new NotImplementedException();
    public override int MoveSpeed { get => throw new NotImplementedException(); }

    public override float CritChance => throw new NotImplementedException();

    public override float CritBonus => throw new NotImplementedException();

    public override float DodgeChance => 0.5f;

    public Giant(EnemyBrain enemyBrain) : base(enemyBrain)
    {

    }

    public override void Attack(Target enemyTarget, float enemyDodgeChance)
    {
        if (UnityEngine.Random.value <= CritChance) //&& CurrentEnergy >= EnergyCosts[0])
        {
            enemyTarget.DoCritDamage(CritBonus, DamageAmount, enemyDodgeChance);
            //target.Resource.EnergyExpense(EnergyCosts[0]);
        }
        else
        {
            enemyTarget.DoDamage(DamageAmount, enemyDodgeChance);
            //target.Resource.EnergyExpense(EnergyCosts[1]);
        }
    }
}
   