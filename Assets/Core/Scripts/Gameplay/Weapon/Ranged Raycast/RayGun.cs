using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.VFX;

public class RayGun : RangedWeapon
{
    [SerializeField] private float MuzzleFlashTime;
    [SerializeField] private Transform firePoint;
    private PlayerInputAsset inputs;
    private GameController gameController;
    private float bloom;
    private bool isRanged;
    private bool isReloading;
    private Weapons thisWeapon;
    private WeaponCategories weaponCategory;
    private WeaponTypes weaponType;
    private int PlayerLayer = 9;
    private int bitmask;
    private Camera cam;
    private WeaponBrain weaponBrain;
    private VisualEffect visualEffect;
    private Light lighting;
    public static event Action OnStopRayShoot;

    private void Awake()
    {
        weaponBrain = GetComponent<WeaponBrain>();
        visualEffect = GetComponentInChildren<VisualEffect>();
        cam = Camera.main;
        bitmask = ~(1 << PlayerLayer);
        lighting = GetComponentInChildren<Light>();
        isReloading = false;
        if (lighting.gameObject.activeInHierarchy)
            lighting.gameObject.SetActive(false);
        StartCoroutine(SetupWeapon());
    }

    private void OnEnable()
    {
        StartCoroutine(InputDone());
    }

    private void Update()
    {
        if (gameController == null)
        {
            gameController = GameController.Instance;
        }
        if (gameController != null && inputs != null)
        {
            if (gameObject.activeInHierarchy && weaponBrain.IsWeaponReady())
            {
                if (!gameController.IsInventoryActive && !gameController.IsMainMenuActive)
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
                                    CallEvent(this);
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
                        OnStopRayShoot?.Invoke();
                    }
                }
            }
        }          
    }

    private IEnumerator InputDone()
    {
        yield return new WaitUntil(() => InputManager.InputReady);
        inputs = InputManager.InputActions;
        weaponBrain.GetThisWeapon();
    }

    private void PlayBulletTrailVfx()
    {
        if (visualEffect != null)
        {
            Vector3 dir = firePoint.forward;
            visualEffect.SetVector3("Direction", dir);
            visualEffect.Play();
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
        currentTotalAmmo =  maxTotalAmmo;
        weaponCategory = weaponBrain.GetWeaponCategories();
        attackRange = thisWeapon.ThisWeaponSO.AttackRange;
        attackSpeed = thisWeapon.ThisWeaponSO.AttackSpeed;
        bloom = thisWeapon.ThisWeaponSO.Bloom;
        isRanged = thisWeapon.ThisWeaponSO.IsRanged;
        isReady = true;
    }

    private Vector3 GetShootDir()
    {
        Vector3 t_bloom = cam.transform.position + cam.transform.forward * attackRange;
        t_bloom += UnityEngine.Random.Range(-bloom, bloom) * cam.transform.up;
        t_bloom += UnityEngine.Random.Range(-bloom, bloom) * cam.transform.right;
        t_bloom -= cam.transform.position;
        t_bloom.Normalize();
        return t_bloom;
    }

    private IEnumerator Shoot(Action action)
    {
        StartCoroutine(PlayMuzzleLight());
        if (isRanged)
        {
            WeaponManager.Instance.IsAttacking = true;
            thisWeapon.RaiseOnPlayerAttack(thisWeapon, weaponBrain, weaponCategory, weaponType);
            PlayBulletTrailVfx();            
            if (Physics.Raycast(cam.transform.position, GetShootDir(), out RaycastHit hit, attackRange, bitmask, QueryTriggerInteraction.Ignore))
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
            }
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
                currentMagazineAmmo = maxMagazineAmmo;
                CallEvent(this);
                thisWeapon.RaiseOnPlayerReload(thisWeapon, weaponBrain, weaponType);
                yield return new WaitForSeconds(weaponBrain.AnimDelay);
                action.Invoke();
            }
        }      
    }

}
