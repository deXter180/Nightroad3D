using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBrain : MonoBehaviour
{
    #region Variables

    [SerializeField] private WeaponTypes weaponTypes;
    [SerializeField] private WeaponCategories weaponCategories;   
    private Weapons weapon;    
    private bool IsReady = false;
    public event Action OnStopPlayingReload;

    #endregion

    #region General

    private void Start()
    {
        SetWeapon();
    }

    private void OnEnable()
    {   
        Weapons.OnPlayerAttack += Weapons_OnAttack;
        Weapons.OnPlayerReload += Weapons_OnPlayerReload;      
        RayGun.OnStopRayShoot += RayGun_OnStopShoot;
        ProjectileGun.OnStopProjectileShoot += ProjectileGun_OnStopProjectileShoot;
        RayShotGun.OnStopSGShoot += RayShotGun_OnStopSGShoot;
                  
    }

    private void OnDisable()
    {
        Weapons.OnPlayerAttack -= Weapons_OnAttack;
        Weapons.OnPlayerReload -= Weapons_OnPlayerReload;
        RayGun.OnStopRayShoot -= RayGun_OnStopShoot;
        ProjectileGun.OnStopProjectileShoot -= ProjectileGun_OnStopProjectileShoot;
        RayShotGun.OnStopSGShoot -= RayShotGun_OnStopSGShoot;
    }

    private void Update()
    {
        
    }

    private void SetWeapon()
    {
        if (weapon == null)
        {
            weapon = new Weapons(this, weaponTypes);
            IsReady = true;
        }              
    }

    public void EquipAnim()
    {
        
    }

    public Weapons GetThisWeapon()
    {
        SetWeapon();
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

    public bool IsWeaponReady()
    {
        return IsReady;
    }

    private string GetWeaponName()
    {
        switch (weapon.ThisWeaponSO.ImpactType)
        {
            case ImpactTypes.BulletHole:
                return "BulletMark_vfx";
            case ImpactTypes.FireMark:
                return "FireMark_vfx";
            case ImpactTypes.ShotgunHit:
                return "ShotgunMark_vfx";
            case ImpactTypes.None:
                return null;            
            default: return null;
        }
    }

    public void SpawnHitVfx(Vector3 HitPointPos)
    {
        AssetLoader.CreateAndReleaseAsset(GetWeaponName(), HitPointPos, 0.3f);
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~ Event Callback ~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void Weapons_OnAttack(object sender, OnPlayerAttackEventArg e)
    {
        
    }

    private void Weapons_OnPlayerReload(object sender, OnPlayerReloadEventArg e)
    {
        
    }

    private void RayGun_OnStopShoot()
    {
        
    }

    private void ProjectileGun_OnStopProjectileShoot()
    {
        
    }

    private void RayShotGun_OnStopSGShoot()
    {
        
    }

    #endregion
}
