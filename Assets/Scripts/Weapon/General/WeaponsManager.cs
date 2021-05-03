using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public abstract class Weapons : IAttack
{
    public abstract WeaponTypes weaponTypes { get; }
    public abstract int DamageAmount { get; }
    public abstract float AttackSpeed { get; }
    public abstract int AttackRange { get; }

    public abstract float CritChance { get; }

    public abstract float CritBonus { get; }

    public virtual void DoAttack(Target enemyTarget, float enemyDodgeChance)
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

public enum WeaponTypes
{
    Axe,
    Rifle,
    RocketLauncher
}

public enum WeaponCategories
{
    Melee,
    RaycastShoot,
    ProjectileShoot
}