using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RayGun : MonoBehaviour
{
    private int PlayerLayer = 9;
    private int EnemyLayer = 12;
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
        bitmask = ~(1 << PlayerLayer) & (1 << EnemyLayer);
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
        Vector3 range = new Vector3(cam.transform.position.x, cam.transform.position.y, weaponBrain.GetThisWeapon().AttackRange);
        Debug.DrawRay(cam.transform.position, range, Color.red);
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Inventory.Instance.IsAttacking = true;
        weaponBrain.GetThisWeapon().RaiseOnAttack(weaponBrain.GetThisWeapon(), weaponBrain.GetWeaponCategories(), weaponBrain.GetWeaponTypes());
        if (Physics.Raycast(ray, out RaycastHit hit, weaponBrain.GetThisWeapon().AttackRange, bitmask))
        {
            if (hit.collider != null)
            {
                Debug.Log(hit.transform.name);
                if (hit.collider.GetComponent<Target>() != null)
                {
                    Target target = hit.collider.GetComponent<Target>();
                    weaponBrain.GetThisWeapon().DoAttack(target, target.GetEBFromTarget().GetThisEnemy().DodgeChance);
                }
            }
        }
        yield return new WaitForSeconds(weaponBrain.GetThisWeapon().AttackSpeed);
        action.Invoke();
    }



}
