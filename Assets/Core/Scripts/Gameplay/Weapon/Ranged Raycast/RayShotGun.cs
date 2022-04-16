using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RayShotGun : MonoBehaviour
{
    public int CurrentMagazineAmmo { get => currentMagazineAmmo; }
    public int CurrentTotalAmmo { get => currentTotalAmmo; }
    [SerializeField] private float MuzzleFlashTime;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int PelletsCount;
    private PlayerInputAsset inputs;
    private GameController gameController;
    private int currentMagazineAmmo;
    private int currentTotalAmmo;
    private int maxMagazineAmmo;
    private int maxTotalAmmo;
    private float attackRange;
    private float attackSpeed;
    private float bloom;
    private bool isReloading;
    private bool isRanged;
    private Weapons thisWeapon;
    private WeaponCategories weaponCategory;
    private WeaponTypes weaponType;
    private int PlayerLayer = 9;
    private int bitmask;
    private Camera cam;
    private WeaponBrain weaponBrain;
    private VisualEffect visualEffect;
    private Light lighting;
    public static event Action OnStopSGShoot;

    private void Awake()
    {
        isReloading = false;
        weaponBrain = GetComponent<WeaponBrain>();
        visualEffect = GetComponentInChildren<VisualEffect>();
        cam = Camera.main;
        bitmask = ~(1 << PlayerLayer);
        lighting = GetComponentInChildren<Light>();
        if (lighting.gameObject.activeInHierarchy)
            lighting.gameObject.SetActive(false);       
    }

    private void OnEnable()
    {
        StartCoroutine(InputDone());
        StartCoroutine(SetupWeapon());
    }

    private void Update()
    {
        if (gameController == null)
        {
            gameController = GameController.Instance;
        }
        if (gameController != null)
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
                        OnStopSGShoot?.Invoke();
                    }
                }
            }
        }
       
    }

    private IEnumerator InputDone()
    {
        yield return new WaitUntil(() => InputManager.InputReady);
        inputs = InputManager.InputActions;
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
        attackRange = thisWeapon.ThisWeaponSO.AttackRange;
        attackSpeed = thisWeapon.ThisWeaponSO.AttackSpeed;
        bloom = thisWeapon.ThisWeaponSO.Bloom;
        isRanged = thisWeapon.ThisWeaponSO.IsRanged;
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

    private void PlayBulletTrailVfx()
    {
        if (visualEffect != null)
        {
            Vector3 dir = firePoint.forward;
            visualEffect.SetVector3("Direction", dir);
            visualEffect.Play();
        }
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

    public IEnumerator Shoot(Action action)
    {
        StartCoroutine(PlayMuzzleLight());
        if (isRanged)
        {
            WeaponManager.Instance.IsAttacking = true;
            thisWeapon.RaiseOnPlayerAttack(thisWeapon, weaponBrain, weaponCategory, weaponType);
            PlayBulletTrailVfx();
            for(int i = 0; i < PelletsCount; i++)
            {
                if (Physics.Raycast(cam.transform.position, GetShootDir(), out RaycastHit hit, attackRange, bitmask, QueryTriggerInteraction.Ignore))
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
                }
            }
        }
        yield return new WaitForSeconds(attackSpeed);
        action.Invoke();
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
                thisWeapon.RaiseOnPlayerReload(thisWeapon, weaponBrain, weaponType);
                yield return new WaitForSeconds(weaponBrain.AnimDelay);
                action.Invoke();
            }
        }
    }

}
