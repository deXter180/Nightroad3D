using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RayShotGun : MonoBehaviour
{
    [SerializeField] private float MuzzleFlashTime;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int PelletsCount;
    private float attackRange;
    private float attackSpeed;
    private float bloom;
    private bool isRanged;
    private Weapons thisWeapon;
    private WeaponCategories weaponCategory;
    private WeaponTypes weaponType;
    private int PlayerLayer = 9;
    private int bitmask;
    private Camera cam;
    private Input input;
    private WeaponBrain weaponBrain;
    private VisualEffect visualEffect;
    private Light lighting;
    public static event Action OnStopSGShoot;

    private void Awake()
    {
        input = FindObjectOfType<InputControl>();
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
        StartCoroutine(SetupWeapon());
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
                OnStopSGShoot?.Invoke();
            }
        }
    }

    private IEnumerator SetupWeapon()
    {
        yield return new WaitUntil(() => weaponBrain.IsWeaponReady());
        thisWeapon = weaponBrain.GetThisWeapon();
        weaponType = weaponBrain.GetWeaponTypes();
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
            WeaponInventory.Instance.IsAttacking = true;
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
                                    thisWeapon.DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance, false);
                                    if (!target.Dodging)
                                    {

                                    }
                                }
                                else if (hit.collider.CompareTag("Head"))
                                {
                                    thisWeapon.DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance, true);
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

}
