using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class Weapons
{
    private WeaponBrain weaponBrain;
    private WeaponSO weaponSO;
    public WeaponSO ThisWeaponSO => weaponSO;
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

    public Weapons (WeaponBrain wb, WeaponTypes weaponTypes)
    {
        this.weaponBrain = wb;
        weaponSO = WeaponAssets.Instance.GetWeaponSOFromList(weaponTypes);
    }

    public virtual void DoAttack(Target enemyTarget, float enemyDodgeChance)
    {
        if (UnityEngine.Random.value <= weaponSO.CritChance) //&& CurrentEnergy >= EnergyCosts[0])
        {
            enemyTarget.DoCritDamage(weaponSO.CritBonus, weaponSO.DamageAmount, enemyDodgeChance);
            OnPlayerDamage?.Invoke(this, new OnPlayerDamageEventArg(true));
            //target.Resource.EnergyExpense(EnergyCosts[0]);
        }
        else
        {
            enemyTarget.DoDamage(weaponSO.DamageAmount, enemyDodgeChance);
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
