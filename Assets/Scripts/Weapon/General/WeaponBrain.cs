using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBrain : MonoBehaviour
{
    [SerializeField] private WeaponTypes weaponTypes;
    [SerializeField] private WeaponCategories weaponCategories;
    private Animator animator;
    private Weapons weapon;

    //~~~~~~~~~~~~~~ Animation State ~~~~~~~~~~~~~~

    private int AttackHash = Animator.StringToHash("Attack");

    private void Awake()
    {
        if (SetWeapon() != null)
        {
            SetWeapon();
        }
        animator = GetComponentInChildren<Animator>();
    }
    private void OnEnable()
    {
        Weapons.OnAttack += Weapons_OnAttack;
    }
    private void OnDisable()
    {
        Weapons.OnAttack -= Weapons_OnAttack;
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

    //~~~~~~~~~~~~~~~~~~~~~ Event Callback ~~~~~~~~~~~~~~~~~~~~~

    private void Weapons_OnAttack(object sender, OnAttackEventArg e)
    {
        animator.PlayInFixedTime(AttackHash, 0, Time.deltaTime);
        //switch (e.weaponCategory)
        //{
        //    case (WeaponCategories.ProjectileShoot):
        //        {
        //            break;
        //        }
        //    case (WeaponCategories.RaycastShoot):
        //        {
        //            break;
        //        }
        //    case (WeaponCategories.Melee):
        //        {
        //            break;
        //        }
        //}
    }
}
