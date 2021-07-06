﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.PlayerLoop;

public class ProjectileGun : MonoBehaviour
{
    [SerializeField] private Transform FiringPoint;
    private WeaponBrain weaponBrain;
    private Input input;
    

    private void Awake()
    {
        weaponBrain = GetComponent<WeaponBrain>();
        input = GetComponentInParent<InputControl>();
    }

    private void Update()
    {
        if (input.GetAttackHold() == 1 && gameObject.activeInHierarchy)
        {
            if (WeaponInventory.Instance.IsAttacking == false)
            {
                StartCoroutine(Shoot(() => { WeaponInventory.Instance.IsAttacking = false; }));
            }
        }
    }
    private ProjectileTypes GetProjectile(WeaponTypes weaponType)
    {
        switch (weaponType)
        {
            case WeaponTypes.RocketLauncher:
                {
                    return ProjectileTypes.FireBall;
                }
            default: return ProjectileTypes.Bullet;
        }
    }
    public IEnumerator Shoot(Action action) //Used in PlayerControl
    {
        if (ObjectPooler.Instance.GetPooledObject(GetProjectile(weaponBrain.GetWeaponTypes())) != null)
        {
            WeaponInventory.Instance.IsAttacking = true;
            var shot = ObjectPooler.Instance.GetPooledObject(GetProjectile(weaponBrain.GetWeaponTypes()));
            shot.transform.rotation = FiringPoint.rotation;
            shot.transform.position = FiringPoint.position;
            shot.gameObject.SetActive(true);
            weaponBrain.GetThisWeapon().RaiseOnPlayerAttack(weaponBrain.GetThisWeapon(), weaponBrain.GetWeaponCategories(), weaponBrain.GetWeaponTypes());
            yield return new WaitForSeconds(weaponBrain.GetThisWeapon().AttackSpeed);
            action.Invoke();
        }
    }
}
