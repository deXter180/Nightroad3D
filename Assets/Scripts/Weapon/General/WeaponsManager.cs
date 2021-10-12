﻿using System.Collections;
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
    public void RaiseOnPlayerAttack(Weapons weapon, WeaponBrain weaponBrain, WeaponCategories weaponCategory, WeaponTypes weaponType)
    {
        InvokeOnAttack(new OnPlayerAttackEventArg(weapon, weaponBrain, weaponCategory, weaponType));
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
        weaponSO = AssetCollections.GetWeaponSOFromList(weaponTypes);
    }

    public virtual void DoAttack(Target enemyTarget, float enemyDodgeChance)
    {
        if (enemyTarget.enemyBrain != null)
        {
            if (UnityEngine.Random.value <= weaponSO.CritChance) //&& CurrentEnergy >= EnergyCosts[0])
            {
                enemyTarget.DoCritDamage(weaponSO.CritBonus, weaponSO.DamageAmount, enemyDodgeChance);
                if (!enemyTarget.Dodging)
                    OnPlayerDamage?.Invoke(this, new OnPlayerDamageEventArg(true, enemyTarget.enemyBrain));
                //target.Resource.EnergyExpense(EnergyCosts[0]);
            }
            else
            {
                enemyTarget.DoDamage(weaponSO.DamageAmount, enemyDodgeChance);
                if (!enemyTarget.Dodging)
                    OnPlayerDamage?.Invoke(this, new OnPlayerDamageEventArg(false, enemyTarget.enemyBrain));
                //target.Resource.EnergyExpense(EnergyCosts[1]);
            }
        }        
    }
}

public class OnPlayerDamageEventArg : EventArgs
{
    public bool IsCrit;
    public EnemyBrain enemyBrain;

    public OnPlayerDamageEventArg(bool crit, EnemyBrain EB)
    {
        IsCrit = crit;
        enemyBrain = EB;
    }
}
public class OnPlayerAttackEventArg : EventArgs
{
    public Weapons weapon;
    public WeaponBrain weaponBrain;
    public WeaponCategories weaponCategory;
    public WeaponTypes weaponType;

    public OnPlayerAttackEventArg(Weapons WP, WeaponBrain WB, WeaponCategories WC, WeaponTypes WT)
    {
        weapon = WP;
        weaponBrain = WB;
        weaponCategory = WC;
        weaponType = WT;
    }
}
