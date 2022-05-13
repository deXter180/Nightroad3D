using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RangedWeapon : MonoBehaviour
{
    protected int currentMagazineAmmo;
    protected int currentTotalAmmo;
    protected int maxMagazineAmmo;
    protected int maxTotalAmmo;
    protected float attackRange;
    protected float attackSpeed;
    protected bool isReady = false;

    public static event Action<RangedWeapon> OnAmmoAffected;

    protected static void CallEvent(RangedWeapon RG)
    {
        OnAmmoAffected?.Invoke(RG);
    }

    public int CurrentMagazineAmmo => currentMagazineAmmo;
    public int CurrentTotalAmmo => currentTotalAmmo;
    public int MaxMagazineAmmo => maxMagazineAmmo;
    public int MaxTotalAmmo => maxTotalAmmo;
    public bool IsReady => isReady;

    public bool AddAmmo(int ammoAmount)
    {
        if (maxTotalAmmo - currentTotalAmmo >= ammoAmount)
        {
            currentTotalAmmo += ammoAmount;
            OnAmmoAffected?.Invoke(this);
            return true;
        }
        return false;
    }
}
