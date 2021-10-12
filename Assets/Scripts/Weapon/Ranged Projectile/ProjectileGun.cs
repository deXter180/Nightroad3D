using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.PlayerLoop;

public class ProjectileGun : MonoBehaviour
{
    [SerializeField] private float MuzzleFlashTime;
    [SerializeField] private Transform FiringPoint;
    private WeaponBrain weaponBrain;
    private Light lighting;
    private Input input;
    public static event Action OnStopProjectileShoot;
    

    private void Awake()
    {
        weaponBrain = GetComponent<WeaponBrain>();
        input = FindObjectOfType<InputControl>();
        lighting = GetComponentInChildren<Light>();
        if (lighting.gameObject.activeInHierarchy)
            lighting.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy && weaponBrain.IsWeaponReady())
        {
            if (input.GetAttackHold() == 1 && !InventoryUIHandler.Instance.IsInventoryON)
            {
                if (WeaponInventory.Instance.IsAttacking == false)
                {
                    StartCoroutine(Shoot(() => { WeaponInventory.Instance.IsAttacking = false; }));
                }
            }
            else
            {
                OnStopProjectileShoot?.Invoke();
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

    private IEnumerator PlayMuzzleLight()
    {
        if (!lighting.gameObject.activeInHierarchy)
        {
            lighting.gameObject.SetActive(true);
            yield return new WaitForSeconds(MuzzleFlashTime);
            lighting.gameObject.SetActive(false);
        }
    }

    public IEnumerator Shoot(Action action) //Used in PlayerControl
    {
        StartCoroutine(PlayMuzzleLight());
        if (ObjectPooler.Instance.GetPooledObject(GetProjectile(weaponBrain.GetWeaponTypes())) != null)
        {
            WeaponInventory.Instance.IsAttacking = true;
            var shot = ObjectPooler.Instance.GetPooledObject(GetProjectile(weaponBrain.GetWeaponTypes()));
            shot.transform.rotation = FiringPoint.rotation;
            shot.transform.position = FiringPoint.position;
            shot.gameObject.SetActive(true);
            weaponBrain.GetThisWeapon().RaiseOnPlayerAttack(weaponBrain.GetThisWeapon(), weaponBrain, weaponBrain.GetWeaponCategories(), weaponBrain.GetWeaponTypes());
            yield return new WaitForSeconds(weaponBrain.GetThisWeapon().ThisWeaponSO.AttackSpeed);
            action.Invoke();
        }
    }
}
