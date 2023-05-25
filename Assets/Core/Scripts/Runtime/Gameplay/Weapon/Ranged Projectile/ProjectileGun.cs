using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectileGun : RangedWeapon
{
    #region SerializedVariables

    [SerializeField] private float MuzzleFlashTime;
    [SerializeField] private Transform FiringPoint;
    [SerializeField] private Crosshair.CrosshairProperties crosshairProperty;
    [SerializeField] private CameraShake.ShakeProperty shakeProperties;
    [SerializeField] private RecoilEffect.RecoilProperty recoilProperty;

    #endregion

    #region Variables

    private GameController gameController;
    private Crosshair crosshair;
    private CameraShake camShake;
    private RecoilEffect recoilEffect;
    private WeaponManager weaponManager;
    private Weapons thisWeapon;
    private Light lighting;
    private ObjectPooler objectPooler;
    public static event Action OnStopProjectileShoot;

    #endregion

    #region General

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

    private void OnEnable()
    {
        if (crosshair == null)
        {
            crosshair = Crosshair.Instance;
        }
        StartCoroutine(crosshair.SetupCrosshair(crosshairProperty)); 
    }

    private void OnDisable()
    {
        camShake.StopShake();
        crosshair.RemoveCrosshair();
    }

    protected override void Start()
    {
        base.Start();
        weaponManager = WeaponManager.Instance;
        objectPooler = ObjectPooler.Instance;
        camShake = CameraShake.Instance;
        gameController = GameController.Instance;
        recoilEffect = RecoilEffect.Instance;
    }

    private void Update()
    {
        if (gameController != null && inputs != null)
        {
            if (gameObject.activeInHierarchy && weaponBrain.IsWeaponReady())
            {
                if (!gameController.IsUIActive && !gameController.IsCastingSpell)
                {
                    if (!weaponManager.IsAttacking)
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
                                    crosshair.SetRisizing(false);
                                    CallEvent(this);                                   
                                    weaponManager.SetIsAttacking(false);
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

    #endregion

    #region Mechanics

    private IEnumerator Shoot(Action action) //Used in PlayerControl
    {
        StartCoroutine(PlayMuzzleLight());
        if (objectPooler.GetPooledObject(GetProjectileType(weaponType)) != null)
        {
            weaponManager.SetIsAttacking(true);
            crosshair.SetRisizing(true);
            recoilEffect.ApplyRecoil(recoilProperty);
            camShake.StartShake(shakeProperties);
            var shot = objectPooler.GetPooledObject(GetProjectileType(weaponType));
            shot.transform.rotation = FiringPoint.rotation;
            shot.transform.position = FiringPoint.position;
            shot.gameObject.SetActive(true);
            thisWeapon.RaiseOnPlayerAttack(thisWeapon, true, weaponBrain, weaponCategory, weaponType);
            yield return Helpers.GetWait(attackDelay);
            action.Invoke();
        }
        ProjectileTypes GetProjectileType(WeaponTypes weaponType)
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

    #endregion
}
