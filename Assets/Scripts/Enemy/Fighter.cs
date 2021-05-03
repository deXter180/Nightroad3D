using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Enemy
{
    public override EnemyTypes enemyTypes { get => EnemyTypes.Fighter; }
    public override int MaxHP { get => throw new System.NotImplementedException(); }
    public override int DamageAmount { get => throw new System.NotImplementedException(); }
    public override float AttackSpeed { get => throw new System.NotImplementedException(); }
    public override int AttackRange => throw new System.NotImplementedException();
    public override int MoveSpeed { get => throw new System.NotImplementedException(); }

    public override float CritChance => throw new System.NotImplementedException();

    public override float CritBonus => throw new System.NotImplementedException();

    public override float DodgeChance => 0.3f;

    public Fighter(EnemyBrain enemyBrain) : base(enemyBrain)
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
