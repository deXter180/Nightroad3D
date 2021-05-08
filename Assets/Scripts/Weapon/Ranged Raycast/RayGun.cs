using System.Collections;
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
    //private int ShootHash = Animator.StringToHash("Shoot");

    private void Awake()
    {
        input = GetComponentInParent<InputControl>();
        weaponBrain = GetComponent<WeaponBrain>();
        anim = GetComponent<Animator>();
        cam = GetComponentInParent<Camera>();
        bitmask = ~(1 << PlayerLayer);
    }

    private void FixedUpdate()
    {
        if (input.GetAttack() == 1 && gameObject.activeInHierarchy)
        {
            if (Inventory.Instance.IsAttacking == false)
            {
                StartCoroutine(Shoot(() => { Inventory.Instance.IsAttacking = false; }));
            }
        }
        //anim.SetTrigger(ShootHash);
    }
    public IEnumerator Shoot(Action action)
    {
        //Vector3 range = new Vector3(cam.transform.position.x, cam.transform.position.y, weaponBrain.GetThisWeapon().AttackRange);
        //Debug.DrawRay(cam.transform.position, range, Color.red);
        Vector3 t_bloom = cam.transform.position + cam.transform.forward * 1000f;
        t_bloom += UnityEngine.Random.Range(-weaponBrain.GetIfRanged().bloom, weaponBrain.GetIfRanged().bloom) * cam.transform.up;
        t_bloom += UnityEngine.Random.Range(-weaponBrain.GetIfRanged().bloom, weaponBrain.GetIfRanged().bloom) * cam.transform.right;
        t_bloom -= cam.transform.position;
        t_bloom.Normalize();
        //Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Inventory.Instance.IsAttacking = true;
        weaponBrain.GetThisWeapon().RaiseOnAttack(weaponBrain.GetThisWeapon(), weaponBrain.GetWeaponCategories(), weaponBrain.GetWeaponTypes());
        if (Physics.Raycast(cam.transform.position, t_bloom , out RaycastHit hit, weaponBrain.GetThisWeapon().AttackRange, bitmask))
        {
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<Target>() != null)
                {
                    Target target = hit.collider.GetComponent<Target>();
                    if (target.IsDead == false)
                    {
                        weaponBrain.GetThisWeapon().DoAttack(target, target.GetEBFromTarget().GetThisEnemy().DodgeChance);
                        if (target.IsDead == false)
                        {
                            GameObject bHoleOnEnemy = Instantiate(ObjectPooler.Instance.ProjectileImpacts[0].ImpactSprite, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
                            bHoleOnEnemy.transform.LookAt(hit.point + hit.normal);
                            Destroy(bHoleOnEnemy, 2f);
                        }
                    }
                }
                else
                {
                    GameObject bHole = Instantiate(ObjectPooler.Instance.ProjectileImpacts[0].ImpactSprite, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
                    bHole.transform.LookAt(hit.point + hit.normal);
                    Destroy(bHole, 2f);
                }
                
            }
        }
        yield return new WaitForSeconds(weaponBrain.GetThisWeapon().AttackSpeed);
        action.Invoke();
    }



}
