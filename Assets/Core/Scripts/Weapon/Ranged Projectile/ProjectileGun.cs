﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.PlayerLoop;

public class ProjectileGun : MonoBehaviour
{
    public int CurrentMagazineAmmo { get => currentMagazineAmmo; }
    public int CurrentTotalAmmo { get => currentTotalAmmo; }
    [SerializeField] private float MuzzleFlashTime;
    [SerializeField] private Transform FiringPoint;
    private PlayerInputAsset inputs;
    private int currentMagazineAmmo;
    private int currentTotalAmmo;
    private int maxMagazineAmmo;
    private int maxTotalAmmo;
    private bool isReloading;
    private float attackSpeed;
    private Weapons thisWeapon;
    private WeaponCategories weaponCategory;
    private WeaponTypes weaponType;   
    private WeaponBrain weaponBrain;
    private Light lighting;
    private ObjectPooler objectPooler;
    public static event Action OnStopProjectileShoot;  

    private void Awake()
    {
        isReloading = false;
        weaponBrain = GetComponent<WeaponBrain>();
        lighting = GetComponentInChildren<Light>();
        if (lighting.gameObject.activeInHierarchy)
            lighting.gameObject.SetActive(false);
        StartCoroutine(SetupWeapon());
    }

    private void OnEnable()
    {
        StartCoroutine(InputDone());
    }

    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy && weaponBrain.IsWeaponReady())
        {
            if (!InventoryUIHandler.Instance.IsInventoryActive && !InputMenuUIHandler.Instance.IsMainMenuActive)
            {
                if (WeaponManager.Instance.IsAttacking == false)
                {
                    if (inputs.BasicControls.Shoot.ReadValue<float>() == 1)
                    {
                        if (currentMagazineAmmo > 0 && !isReloading)
                        {
                            StartCoroutine(Shoot(() => {
                                WeaponManager.Instance.IsAttacking = false;
                                currentMagazineAmmo -= 1;
                                currentTotalAmmo -= 1;
                                currentMagazineAmmo = currentTotalAmmo == 1 ? 1 : currentMagazineAmmo;
                            }));
                        }
                        else
                        {
                            Debug.Log("RELOAD!");
                        }
                    }
                    else if (inputs.BasicControls.Reload.triggered)
                    {
                        StartCoroutine(Reload(() => { isReloading = false; }));
                    }
                }
                else
                {
                    OnStopProjectileShoot?.Invoke();
                }
            }
            
            
        }  
    }

    private IEnumerator InputDone()
    {
        yield return new WaitUntil(() => InputManager.InputReady);
        inputs = InputManager.InputActions;
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

    private IEnumerator PlayMuzzleLight()
    {
        if (!lighting.gameObject.activeInHierarchy)
        {
            lighting.gameObject.SetActive(true);
            yield return new WaitForSeconds(MuzzleFlashTime);
            lighting.gameObject.SetActive(false);
        }
    }

    private IEnumerator SetupWeapon()
    {
        yield return new WaitUntil(() => weaponBrain.IsWeaponReady());
        thisWeapon = weaponBrain.GetThisWeapon();
        weaponType = weaponBrain.GetWeaponTypes();
        maxMagazineAmmo = thisWeapon.ThisWeaponSO.AmmoPerMagazine;
        maxTotalAmmo = thisWeapon.ThisWeaponSO.TotalAmmo;
        currentMagazineAmmo = maxMagazineAmmo;
        currentTotalAmmo = maxTotalAmmo;
        weaponCategory = weaponBrain.GetWeaponCategories();
        attackSpeed = weaponBrain.GetThisWeapon().ThisWeaponSO.AttackSpeed;
    }

    private IEnumerator Shoot(Action action) //Used in PlayerControl
    {
        StartCoroutine(PlayMuzzleLight());
        if (objectPooler.GetPooledObject(GetProjectile(weaponType)) != null)
        {
            WeaponManager.Instance.IsAttacking = true;
            var shot = objectPooler.GetPooledObject(GetProjectile(weaponType));
            shot.transform.rotation = FiringPoint.rotation;
            shot.transform.position = FiringPoint.position;
            shot.gameObject.SetActive(true);
            thisWeapon.RaiseOnPlayerAttack(thisWeapon, weaponBrain, weaponCategory, weaponType);
            yield return new WaitForSeconds(attackSpeed);
            action.Invoke();
        }
    }

    private IEnumerator Reload(Action action)
    {
        if (currentTotalAmmo > 0)
        {
            if (currentMagazineAmmo < maxMagazineAmmo)                
            {
                if (currentTotalAmmo >= maxMagazineAmmo)
                {
                    currentMagazineAmmo = maxMagazineAmmo;
                }
                else
                {
                    currentMagazineAmmo = currentTotalAmmo;
                }
                isReloading = true;                
                thisWeapon.RaiseOnPlayerReload(thisWeapon, weaponBrain, weaponType);
                yield return new WaitForSeconds(weaponBrain.AnimDelay);
                action.Invoke();
            }
        }
    }
}