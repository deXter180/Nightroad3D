using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.VFX;

public class RayGun : MonoBehaviour
{
    [SerializeField] private float MuzzleFlashTime;
    [SerializeField] private Transform firePoint;
    private int PlayerLayer = 9;
    private int bitmask;
    private Camera cam;
    private Input input;
    private WeaponBrain weaponBrain;
    private VisualEffect visualEffect;
    private Light lighting;
    public static event Action OnStopRayShoot;
    
    //private int ShootHash = Animator.StringToHash("Shoot");

    private void Awake()
    {
        input = FindObjectOfType<InputControl>();
        weaponBrain = GetComponent<WeaponBrain>();
        visualEffect = GetComponentInChildren<VisualEffect>();
        cam = GetComponentInParent<Camera>();
        bitmask = ~(1 << PlayerLayer);
        lighting = GetComponentInChildren<Light>();
        if (lighting.gameObject.activeInHierarchy)
            lighting.gameObject.SetActive(false);        
    }

    private void FixedUpdate()
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
                OnStopRayShoot?.Invoke();
            }
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

    private IEnumerator PlayMuzzleLight()
    {
        if (!lighting.gameObject.activeInHierarchy)
        {
            lighting.gameObject.SetActive(true);
            yield return new WaitForSeconds(MuzzleFlashTime);
            lighting.gameObject.SetActive(false);
        }
    }

    public IEnumerator Shoot(Action action)
    {
        StartCoroutine(PlayMuzzleLight());
        if (weaponBrain.GetThisWeapon().ThisWeaponSO.IsRanged)
        {
            float bloom = weaponBrain.GetThisWeapon().ThisWeaponSO.Bloom;
            Vector3 t_bloom = cam.transform.position + cam.transform.forward * 1000f;
            t_bloom += UnityEngine.Random.Range(-bloom, bloom) * cam.transform.up;
            t_bloom += UnityEngine.Random.Range(-bloom, bloom) * cam.transform.right;
            t_bloom -= cam.transform.position;
            t_bloom.Normalize();
            WeaponInventory.Instance.IsAttacking = true;
            weaponBrain.GetThisWeapon().RaiseOnPlayerAttack(weaponBrain.GetThisWeapon(), weaponBrain, weaponBrain.GetWeaponCategories(), weaponBrain.GetWeaponTypes());
            PlayBulletTrailVfx();            
            if (Physics.Raycast(cam.transform.position, t_bloom, out RaycastHit hit, weaponBrain.GetThisWeapon().ThisWeaponSO.AttackRange, bitmask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.GetComponentInParent<Target>() && hit.collider.CompareTag("Enemy"))
                    {
                        Target target = hit.collider.GetComponentInParent<Target>();
                        if (target != null && target.enemyBrain != null)
                        {
                            if (target.IsDead == false)
                            {
                                weaponBrain.GetThisWeapon().DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance);
                            }
                            if (!target.Dodging)
                            {

                            }
                        }                                                                      
                    }
                    else
                    {

                    }
                    weaponBrain.SpawnHitVfx(hit.point);                    
                }
            }
            yield return new WaitForSeconds(weaponBrain.GetThisWeapon().ThisWeaponSO.AttackSpeed);
            action.Invoke();
        }       
    }



}
