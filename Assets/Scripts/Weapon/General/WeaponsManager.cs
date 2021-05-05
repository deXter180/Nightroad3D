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

    public static event EventHandler<OnAttackEventArg> OnAttack; //Indirectly Raising this event
    public static event EventHandler<OnDamageEventArg> OnDamage;
    public void RaiseOnAttack(Weapons weapon, WeaponCategories weaponCategory, WeaponTypes weaponType)
    {
        InvokeOnAttack(new OnAttackEventArg(weapon, weaponCategory, weaponType));
    }

    private void InvokeOnAttack(OnAttackEventArg eventArg)
    {
        var handler = OnAttack;
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
            OnDamage?.Invoke(this, new OnDamageEventArg(true));
            //target.Resource.EnergyExpense(EnergyCosts[0]);
        }
        else
        {
            enemyTarget.DoDamage(DamageAmount, enemyDodgeChance);
            OnDamage?.Invoke(this, new OnDamageEventArg(false));
            //target.Resource.EnergyExpense(EnergyCosts[1]);
        }
    }
}

public class OnDamageEventArg : EventArgs
{
    public bool IsCrit;

    public OnDamageEventArg(bool crit)
    {
        IsCrit = crit;
    }
}
public class OnAttackEventArg : EventArgs
{
    public Weapons weapon;
    public WeaponCategories weaponCategory;
    public WeaponTypes weaponType;

    public OnAttackEventArg(Weapons WP, WeaponCategories WC, WeaponTypes WT)
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