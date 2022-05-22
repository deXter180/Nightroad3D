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
    [SerializeField] private CameraShake.ShakeProperty property;
    [SerializeField] private List<Vector3> RecoilPattern;
    private GameController gameController;
    private bool isRanged;
    private bool isRecoiled = false;
    private Weapons thisWeapon;
    private int currentRecoilIndex = 0;
    private int recoilRound = 0;
    private int PlayerLayer = 9;
    private int bitmask;
    private int defaultRecoilOffset = 7;
    private float lastFireTime = 0;
    private Vector3 firePointRot;
    private VisualEffect visualEffect;
    private CameraShake camShake;
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
                    if (!WeaponManager.Instance.IsAttacking && !isReloading)
                    {
                        if (inputs.BasicControls.Shoot.ReadValue<float>() == 1)
                        {
                            if (Time.time - lastFireTime >= 1f / BulletPerSecond)
                            {
                                if (currentMagazineAmmo > 0)
                                {
                                    HandleRecoil();
                                    Shoot(() =>
                                    {
                                        currentMagazineAmmo -= 1;
                                        currentTotalAmmo -= 1;
                                        currentMagazineAmmo = currentTotalAmmo == 1 ? 1 : currentMagazineAmmo;
                                        CallEvent(this);
                                        WeaponManager.Instance.IsAttacking = false;
                                    });
                                    lastFireTime = Time.time;
                                }
                                else
                                {
                                    Debug.Log("RELOAD!");
                                }
                            }                           
                        }
                        else
                        {
                            OnStopRayShoot?.Invoke();
                        }
                        if (inputs.BasicControls.Reload.triggered)
                        {
                            StartCoroutine(Reload(thisWeapon, weaponBrain, weaponType));
                        }
                    }
                }
                if (isRecoiled && Time.time - lastFireTime >= RecoilResetTimeInSec)
                {
                    isRecoiled = false;
                    SetFirePointRotation(Vector3.zero);
                    recoilRound = 0;
                }
            }
        }          
    }

    private void HandleRecoil()
    {
        if (!isRecoiled)
        {
            SetFirePointRotation(firePoint.localEulerAngles + RecoilPattern[0]/defaultRecoilOffset);
            currentRecoilIndex = 1;
            isRecoiled = true;
        }
        else
        {
            SetFirePointRotation(firePoint.localEulerAngles + RecoilPattern[currentRecoilIndex]/defaultRecoilOffset);
            if (recoilRound < 3)
            {
                if (currentRecoilIndex + 1 <= RecoilPattern.Count - 1)
                {
                    currentRecoilIndex++;
                }
                else
                {
                    currentRecoilIndex = 0;
                    recoilRound++;
                }
            }
            else
            {
                
            }
           
        }    
    }

    private void SetFirePointRotation(Vector3 rotation)
    {
        firePointRot = rotation;
        firePoint.localRotation = Quaternion.Euler(firePointRot);
    }

    private void Shoot(Action action)
    {
        StartCoroutine(PlayMuzzleLight());
        if (isRanged)
        {
            WeaponManager.Instance.IsAttacking = true;
            thisWeapon.RaiseOnPlayerAttack(thisWeapon, true, weaponBrain, weaponCategory, weaponType);           
            camShake.ShakeOnce(property);
            PlayBulletTrailVfx();
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
        void PlayBulletTrailVfx()
        {
            if (visualEffect != null)
            {
                visualEffect.Play();
            }
        }
    }

}
