using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class Weapons : IModifiedStat
{
    #region Variables

    private WeaponBrain weaponBrain;
    private AttributeManager attributeManager;
    private WeaponSO weaponSO;
    public WeaponSO ThisWeaponSO => weaponSO;
    public static event EventHandler<OnPlayerAttackEventArg> OnPlayerAttack; //Indirectly Raising this event
    public static event EventHandler<OnPlayerDamageEventArg> OnPlayerDamage;
    public static event EventHandler<OnPlayerReloadEventArg> OnPlayerReload; //Indirectly Raising this event

    #endregion

    #region MechanicsFunctions

    public Weapons(WeaponBrain wb, WeaponTypes weaponTypes)
    {
        this.weaponBrain = wb;
        attributeManager = AttributeManager.Instance;
        weaponSO = GameController.GetWeaponSOFromList(weaponTypes);
    }

    public void RaiseOnPlayerAttack(Weapons weapon, bool isRanged, WeaponBrain weaponBrain, WeaponCategories weaponCategory, WeaponTypes weaponType)
    {
        InvokeOnAttack(new OnPlayerAttackEventArg(weapon, isRanged, weaponBrain, weaponCategory, weaponType));
    }

    public void RaiseOnPlayerReload(Weapons weapon, RangedWeapon rangedWeapon, WeaponTypes weaponType)
    {
        InvokeOnReload(new OnPlayerReloadEventArg(weapon, rangedWeapon, weaponType));
    }

    private void InvokeOnAttack(OnPlayerAttackEventArg eventArg)
    {
        var handler = OnPlayerAttack;
        if (handler != null)
        {
            handler(this, eventArg);
        }
    }

    private void InvokeOnReload(OnPlayerReloadEventArg eventArg)
    {
        var handler = OnPlayerReload;
        if (handler != null)
        {
            handler(this, eventArg);
        }
    }

    public int GetModifiedStat()
    {
        float modifiedDamageAmount = 0;
        if (weaponSO.IsRanged)
        {
            modifiedDamageAmount = weaponSO.DamageAmount + (attributeManager.DexterityStat * weaponSO.StatMultiplier);
        }
        else
        {
            modifiedDamageAmount = weaponSO.DamageAmount + (attributeManager.StrengthStat * weaponSO.StatMultiplier);
        }
        return Mathf.RoundToInt(modifiedDamageAmount);
    }

    public virtual void DoAttack(Target enemyTarget, float enemyDodgeChance)
    {
        if (enemyTarget.enemyCore != null)
        {
            int modifiedDamageAmount = GetModifiedStat();                     
            if (UnityEngine.Random.value <= weaponSO.CritChance) //&& CurrentEnergy >= EnergyCosts[0])
            {
                enemyTarget.DoCritDamage(weaponSO.CritBonus, modifiedDamageAmount, enemyDodgeChance);
                if (!enemyTarget.Dodging)
                    OnPlayerDamage?.Invoke(this, new OnPlayerDamageEventArg(true, enemyTarget.enemyCore));
                //target.Resource.EnergyExpense(EnergyCosts[0]);
            }
            else
            {
                enemyTarget.DoDamage(modifiedDamageAmount, enemyDodgeChance);
                if (!enemyTarget.Dodging)
                    OnPlayerDamage?.Invoke(this, new OnPlayerDamageEventArg(false, enemyTarget.enemyCore));
                //target.Resource.EnergyExpense(EnergyCosts[1]);
            }
        }        
    }

    #endregion
}

#region EventArgs

public class OnPlayerDamageEventArg : EventArgs
{
    public bool IsCrit;
    public EnemyCore enemyCore;

    public OnPlayerDamageEventArg(bool crit, EnemyCore EC)
    {
        IsCrit = crit;
        enemyCore = EC;
    }
}

public class OnPlayerAttackEventArg : EventArgs
{
    public Weapons weapon;
    public bool isRanged;
    public WeaponBrain weaponBrain;
    public WeaponCategories weaponCategory;
    public WeaponTypes weaponType;

    public OnPlayerAttackEventArg(Weapons WP, bool ranged, WeaponBrain WB, WeaponCategories WC, WeaponTypes WT)
    {
        weapon = WP;
        isRanged = ranged;
        weaponBrain = WB;
        weaponCategory = WC;
        weaponType = WT;
    }
}

public class OnPlayerReloadEventArg : EventArgs
{
    public Weapons weapon;
    public RangedWeapon rangedWeapon;
    public WeaponTypes weaponType;

    public OnPlayerReloadEventArg(Weapons WP, RangedWeapon RW, WeaponTypes WT)
    {
        weapon = WP;
        rangedWeapon = RW;
        weaponType = WT; 
    }
}

#endregion
