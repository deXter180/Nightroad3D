using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RayShotGun : RangedWeapon
{
    [SerializeField] private float MuzzleFlashTime;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int PelletsCount;
    [SerializeField] private CameraShake.ShakeProperty properties;
    private GameController gameController;
    private bool isRanged;
    private Weapons thisWeapon;
    private int PlayerLayer = 9;
    private int bitmask;
    private CameraShake camShake;
    private VisualEffect visualEffect;
    private Light lighting;
    public static event Action OnStopSGShoot;

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

    private void OnDisable()
    {
        camShake.StopShake();
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
                        OnStopSGShoot?.Invoke();
                    }
                }
            }
        }
       
    }

    public IEnumerator Shoot(Action action)
    {
        StartCoroutine(PlayMuzzleLight());
        if (isRanged)
        {
            WeaponManager.Instance.IsAttacking = true;
            thisWeapon.RaiseOnPlayerAttack(thisWeapon, true, weaponBrain, weaponCategory, weaponType);
            PlayBulletTrailVfx();
            camShake.StartShake(properties);
            for(int i = 0; i < PelletsCount; i++)
            {
                if (Physics.Raycast(cam.transform.position, GenerateRandomRecoil(), out RaycastHit hit, attackRange, bitmask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider != null)
                    {
                        if(hit.collider.GetComponentInParent<Target>() != null)
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
            }
        }
        yield return Helpers.GetWait(attackDelay);
        action.Invoke();
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
                Vector3 dir = firePoint.forward;
                visualEffect.SetVector3("Direction", dir);
                visualEffect.Play();
            }
        }
    }
}
