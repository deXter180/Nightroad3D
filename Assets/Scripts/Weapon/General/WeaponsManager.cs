using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public abstract class Weapons : IAttack
{
    public abstract WeaponTypes weaponTypes { get; }
    public abstract int DamageAmount { get; }
    public abstract float AttackSpeed { get; }
    public abstract float AttackRange { get; }

    public abstract float CritChance { get; }

    public abstract float CritBonus { get; }

    public static event EventHandler<OnPlayerAttackEventArg> OnPlayerAttack; //Indirectly Raising this event
    public static event EventHandler<OnPlayerDamageEventArg> OnPlayerDamage;
    public void RaiseOnPlayerAttack(Weapons weapon, WeaponCategories weaponCategory, WeaponTypes weaponType)
    {
        InvokeOnAttack(new OnPlayerAttackEventArg(weapon, weaponCategory, weaponType));
    }

    private void InvokeOnAttack(OnPlayerAttackEventArg eventArg)
    {
        var handler = OnPlayerAttack;
        if (handler != null)
        {
            handler(this, eventArg);
        }
    }

    public virtual void DoAttack(Target enemyTarget, float enemyDodgeChance)
    {
        if (UnityEngine.Random.value <= CritChance) //&& CurrentEnergy >= EnergyCosts[0])
        {
            enemyTarget.DoCritDamage(CritBonus, DamageAmount, enemyDodgeChance);
            OnPlayerDamage?.Invoke(this, new OnPlayerDamageEventArg(true));
            //target.Resource.EnergyExpense(EnergyCosts[0]);
        }
        else
        {
            enemyTarget.DoDamage(DamageAmount, enemyDodgeChance);
            OnPlayerDamage?.Invoke(this, new OnPlayerDamageEventArg(false));
            //target.Resource.EnergyExpense(EnergyCosts[1]);
        }
    }
}

public class OnPlayerDamageEventArg : EventArgs
{
    public bool IsCrit;

    public OnPlayerDamageEventArg(bool crit)
    {
        IsCrit = crit;
    }
}
public class OnPlayerAttackEventArg : EventArgs
{
    public Weapons weapon;
    public WeaponCategories weaponCategory;
    public WeaponTypes weaponType;

    public OnPlayerAttackEventArg(Weapons WP, WeaponCategories WC, WeaponTypes WT)
    {
        weapon = WP;
        weaponCategory = WC;
        weaponType = WT;
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