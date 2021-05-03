using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBrain : MonoBehaviour, IGetWeapon
{
    [SerializeField] private WeaponTypes weaponTypes;
    [SerializeField] private WeaponCategories weaponCategories;
    private Weapons weapon;

    private void Awake()
    {
        if (SetWeapon() != null)
        {
            SetWeapon();
        }
    }
    private Weapons SetWeapon()
    {
        weapon = WeaponFactory.GetWeapon(weaponTypes);
        return weapon;
    }
    public Weapons GetThisWeapon()
    {
        return weapon;
    }
    public WeaponTypes GetWeaponTypes()
    {
        return weaponTypes;
    }
    public WeaponCategories GetWeaponCategories()
    {
        return weaponCategories;
    }
}
