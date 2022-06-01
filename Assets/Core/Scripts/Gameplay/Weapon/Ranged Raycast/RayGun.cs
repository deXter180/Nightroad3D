using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.VFX;

public class RayGun : RangedWeapon
{
    [SerializeField] private int BulletPerSecond;
    [SerializeField] private float RecoilRange = 0.002f;
    [SerializeField] private float RecoilResetTimeInSec;
    [SerializeField] private float RecoilResetSpeed;
    [SerializeField] private float MuzzleFlashTime;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Crosshair.CrosshairProperties crosshairProperty;
    [SerializeField] private CameraShake.ShakeProperty shakeProperty;
    [SerializeField] private RecoilEffect.RecoilProperty recoilProperty;
    private WeaponManager weaponManager;
    private GameController gameController;
    private Crosshair crosshair;
    private bool isRanged;
    private bool isStartedRecoil;
    private Weapons thisWeapon;
    private int PlayerLayer = 9;
    private int bitmask;
    private float recoilTime = 0;
    private float lastFireTime = 0;
    private float defaultOffset = 1.2f;
    private float recoilOffset = 2f;
    private VisualEffect visualEffect;
    private CameraShake camShake;
    private RecoilEffect recoilEffect;
    private Light lighting;
    public static event Action OnStopRayShoot;

    protected override void Awake()
    {
        base.Awake();
        weaponBrain = GetComponent<WeaponBrain>();
        visualEffect = GetComponentInChildren<VisualEffect>();
        bitmask = ~(1 << PlayerLayer);
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
            attackRange = thisWeapon.ThisWeaponSO.AttackRange;
            attackDelay = thisWeapon.ThisWeaponSO.AttackDelay;
            bloom = thisWeapon.ThisWeaponSO.Bloom;
            isRanged = thisWeapon.ThisWeaponSO.IsRanged;
            isReady = true;
        }
    }

    protected override void Start()
    {
        base.Start();
        isStartedRecoil = false;      
        weaponManager = WeaponManager.Instance;
        camShake = CameraShake.Instance;
        recoilEffect = RecoilEffect.Instance;
        gameController = GameController.Instance;
    }

    private void OnEnable()
    {
        if (crosshair == null)
        {
            crosshair = Crosshair.Instance;
        }        
        ChangeVFXRotation(defaultOffset);
        StartCoroutine(crosshair.SetupCrosshair(crosshairProperty));
    }

    private void OnDisable()
    {
        crosshair.RemoveCrosshair();
    }

    private void Update()
    {       
        if (gameController != null && inputs != null)
        {
            if (gameObject.activeInHierarchy && weaponBrain.IsWeaponReady())
            {
                if (!gameController.IsInventoryActive && !gameController.IsMainMenuActive && !gameController.IsStashActive && !gameController.IsCastingSpell)
                {
                    if (!weaponManager.IsAttacking && !isReloading)
                    {
                        if (inputs.BasicControls.Shoot.ReadValue<float>() == 1)
                        {
                            if (Time.time - lastFireTime >= 1f / BulletPerSecond)
                            {
                                if (currentMagazineAmmo > 0)
                                {                                   
                                    Shoot(() =>
                                    {
                                        
                                        currentMagazineAmmo -= 1;
                                        currentTotalAmmo -= 1;
                                        currentMagazineAmmo = currentTotalAmmo == 1 ? 1 : currentMagazineAmmo;
                                        CallEvent(this);
                                        weaponManager.IsAttacking = false;                                       
                                    });
                                    lastFireTime = Time.time;
                                    recoilTime++;
                                    if (!isStartedRecoil && recoilTime > 5)
                                    {
                                        ChangeVFXRotation(recoilOffset);
                                        isStartedRecoil = true;
                                    }
                                }
                                else
                                {
                                    OnStopRayShoot?.Invoke();
                                    recoilTime = 0;
                                    isStartedRecoil = false;
                                    crosshair.SetRisizing(false);
                                    ChangeVFXRotation(defaultOffset);
                                    Debug.Log("RELOAD!");
                                }
                            }
                        }
                        else
                        {
                            recoilTime = 0;
                            isStartedRecoil = false;
                            crosshair.SetRisizing(false);
                            ChangeVFXRotation(defaultOffset);
                            OnStopRayShoot?.Invoke();
                        }
                        if (inputs.BasicControls.Reload.triggered)
                        {
                            StartCoroutine(Reload(thisWeapon, weaponBrain, weaponType));
                        }
                    }
                }
            }
        }          
    }

    private void ChangeVFXRotation(float recoilXOffset)
    {
        visualEffect.transform.localEulerAngles = new Vector3(recoilXOffset, visualEffect.transform.localEulerAngles.y, visualEffect.transform.localEulerAngles.z);
    }

    private void Shoot(Action action)
    {
        StartCoroutine(PlayMuzzleLight());
        if (isRanged)
        {
            weaponManager.IsAttacking = true;
            thisWeapon.RaiseOnPlayerAttack(thisWeapon, true, weaponBrain, weaponCategory, weaponType);
            crosshair.SetRisizing(true);
            visualEffect.Play();
            camShake.ShakeOnce(shakeProperty);
            recoilEffect.ApplyRecoil(recoilProperty);
            if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, attackRange, bitmask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.GetComponentInParent<Target>() != null)
                    {
                        Target target = hit.collider.GetComponentInParent<Target>();
                        if (target.enemyBrain != null && target.GetEnemy() == true && target.IsDead == false)
                        {                           
                            if (hit.collider.CompareTag("Enemy"))
                            {
                                thisWeapon.DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance);
                                if (!target.Dodging)
                                {

                                }
                            }
                        }                      
                    }                    
                    else
                    {

                    }
                    weaponBrain.SpawnHitVfx(hit.point);                    
                }
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * thisWeapon.ThisWeaponSO.ImpactForce, ForceMode.Impulse);
                }
            }
            action.Invoke();
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
