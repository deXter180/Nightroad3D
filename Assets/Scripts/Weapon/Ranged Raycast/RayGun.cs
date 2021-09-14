﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class RayGun : MonoBehaviour
{
    private int PlayerLayer = 9;
    private int bitmask;
    private Camera cam;
    private Animator anim;
    private Input input;
    private WeaponBrain weaponBrain;
    public static event Action OnStopRayShoot;
    
    //private int ShootHash = Animator.StringToHash("Shoot");

    private void Awake()
    {
        input = FindObjectOfType<InputControl>();
        weaponBrain = GetComponent<WeaponBrain>();
        anim = GetComponent<Animator>();
        cam = GetComponentInParent<Camera>();
        bitmask = ~(1 << PlayerLayer);
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
    public IEnumerator Shoot(Action action)
    {
        if (weaponBrain.GetThisWeapon().ThisWeaponSO.IsRanged)
        {
            float bloom = weaponBrain.GetThisWeapon().ThisWeaponSO.Bloom;
            //Vector3 range = new Vector3(cam.transform.position.x, cam.transform.position.y, weaponBrain.GetThisWeapon().AttackRange);
            //Debug.DrawRay(cam.transform.position, range, Color.red);
            Vector3 t_bloom = cam.transform.position + cam.transform.forward * 1000f;
            t_bloom += UnityEngine.Random.Range(-bloom, bloom) * cam.transform.up;
            t_bloom += UnityEngine.Random.Range(-bloom, bloom) * cam.transform.right;
            t_bloom -= cam.transform.position;
            t_bloom.Normalize();
            //Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            WeaponInventory.Instance.IsAttacking = true;
            weaponBrain.GetThisWeapon().RaiseOnPlayerAttack(weaponBrain.GetThisWeapon(), weaponBrain.GetWeaponCategories(), weaponBrain.GetWeaponTypes());
            if (Physics.Raycast(cam.transform.position, t_bloom, out RaycastHit hit, weaponBrain.GetThisWeapon().ThisWeaponSO.AttackRange, bitmask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider != null)
                {
                    GameObject tempGO = new GameObject();
                    if (hit.collider.GetComponentInParent<Target>() && hit.collider.CompareTag("Enemy"))
                    {
                        Target target = hit.collider.GetComponentInParent<Target>();
                        if (target != null && target.GetEBFromTarget() != null)
                        {
                            if (target.IsDead == false)
                            {
                                weaponBrain.GetThisWeapon().DoAttack(target, target.GetEBFromTarget().GetThisEnemy().ThisEnemySO.DodgeChance);
                            }
                        }                                                                      
                    }
                    else
                    {
                        
                    }
                    if (AssetCollections.Instance.GetImpactAssetRef(ImpactTypes.BulletHole) != null)
                    {
                        List<GameObject> objs = AssetCollections.InstantiateAndGetAssetsByAssetRef(AssetCollections.Instance.GetImpactAssetRef(ImpactTypes.BulletHole));
                        if (objs != null && objs.Count > 0)
                        {
                            GameObject bHole = objs[0];
                            bHole.transform.SetParent(hit.transform);
                            bHole.transform.rotation = Quaternion.identity;
                            bHole.transform.position = hit.point + hit.normal * 0.05f;
                            bHole.transform.LookAt(hit.point + hit.normal);
                            Destroy(bHole, 1.5f);
                        }                       
                    }
                }
            }
            yield return new WaitForSeconds(weaponBrain.GetThisWeapon().ThisWeaponSO.AttackSpeed);
            action.Invoke();
        }       
    }



}
