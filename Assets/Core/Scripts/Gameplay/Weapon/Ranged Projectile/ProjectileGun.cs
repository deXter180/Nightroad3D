using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.PlayerLoop;

public class ProjectileGun : RangedWeapon
{
    [SerializeField] private float MuzzleFlashTime;
    [SerializeField] private Transform FiringPoint;
    [SerializeField] private CameraShake.ShakeProperty properties;
    private GameController gameController;
    private CameraShake camShake;
    private Weapons thisWeapon;
    private Light lighting;
    private ObjectPooler objectPooler;
    public static event Action OnStopProjectileShoot;  

    protected override void Awake()
    {
        base.Awake();
        weaponBrain = GetComponent<WeaponBrain>();
        lighting = GetComponentInChildren<Light>();
        if (lighting.gameObject.activeInHierarchy)
            lighting.gameObject.SetActive(false);
        StartCoroutine(SetupWeapon());
        IEnumerator SetupWeapon()
        {
            yield return new WaitUntil(() => weaponBrain.IsWeaponReady());
            thisWeapon = weaponBrain.GetThisWeapon();
            weaponType = weaponBrain.GetWeaponTypes();
            maxMagazineAmmo = thisWeapon.ThisWeaponSO.AmmoPerMagazine;
            maxTotalAmmo = thisWeapon.ThisWeaponSO.TotalAmmo;
            currentMagazineAmmo = maxMagazineAmmo;
            currentTotalAmmo = maxTotalAmmo;
            weaponCategory = weaponBrain.GetWeaponCategories();
            attackDelay = weaponBrain.GetThisWeapon().ThisWeaponSO.AttackDelay;
            isReady = true;
        }
    }

    private void OnDisable()
    {
        camShake.StopShake();
    }

    protected override void Start()
    {
        base.Start();
        objectPooler = ObjectPooler.Instance;
        camShake = CameraShake.Instance;
        gameController = GameController.Instance;
    }

    private void Update()
    {
        if (gameController != null && inputs != null)
        {
            if (gameObject.activeInHierarchy && weaponBrain.IsWeaponReady())
            {
                if (!gameController.IsInventoryActive && !gameController.IsMainMenuActive && !gameController.IsCastingSpell)
                {
                    if (!WeaponManager.Instance.IsAttacking)
                    {
                        if (inputs.BasicControls.Shoot.ReadValue<float>() == 1)
                        {
                            if (currentMagazineAmmo > 0 && !isReloading)
                            {
                                StartCoroutine(Shoot(() => {                                   
                                    currentMagazineAmmo -= 1;
                                    currentTotalAmmo -= 1;
                                    currentMagazineAmmo = currentTotalAmmo == 1 ? 1 : currentMagazineAmmo;
                                    camShake.StopShake();
                                    CallEvent(this);
                                    WeaponManager.Instance.IsAttacking = false;
                                }));
                            }
                            else
                            {
                                Debug.Log("RELOAD!");
                            }
                        }
                        else if (inputs.BasicControls.Reload.triggered)
                        {
                            StartCoroutine(Reload(thisWeapon, weaponBrain, weaponType));
                        }
                    }
                    else
                    {
                        OnStopProjectileShoot?.Invoke();
                    }
                }
            }
        }            
    }

    private IEnumerator Shoot(Action action) //Used in PlayerControl
    {
        StartCoroutine(PlayMuzzleLight());
        if (objectPooler.GetPooledObject(GetProjectile(weaponType)) != null)
        {
            WeaponManager.Instance.IsAttacking = true;
            camShake.StartShake(properties);
            var shot = objectPooler.GetPooledObject(GetProjectile(weaponType));
            shot.transform.rotation = FiringPoint.rotation;
            shot.transform.position = FiringPoint.position;
            shot.gameObject.SetActive(true);
            thisWeapon.RaiseOnPlayerAttack(thisWeapon, true, weaponBrain, weaponCategory, weaponType);
            yield return Helpers.GetWait(attackDelay);
            action.Invoke();
        }
        ProjectileTypes GetProjectile(WeaponTypes weaponType)
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
        IEnumerator PlayMuzzleLight()
        {
            if (!lighting.gameObject.activeInHierarchy)
            {
                lighting.gameObject.SetActive(true);
                yield return Helpers.GetWait(MuzzleFlashTime);
                lighting.gameObject.SetActive(false);
            }
        }
    }
}
