using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttacker : MonoBehaviour
{
    private Collider[] colliders;
    private Vector3[] ColRange;
    private WeaponBrain weaponBrain;
    private Input input;
    public static event Action OnStopMeleeAttack;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        weaponBrain = GetComponentInParent<WeaponBrain>();
        input = GetComponentInParent<InputControl>();
        GetRange();
        SetRange();
    }

    private void FixedUpdate()
    {
        if (gameObject.activeInHierarchy)
        {
            if (input.GetAttackHold() == 1 && !InventoryUIHandler.Instance.IsInventoryON)
            {
                if (WeaponInventory.Instance.IsAttacking == false)
                {
                    weaponBrain.GetThisWeapon().RaiseOnPlayerAttack(weaponBrain.GetThisWeapon(), weaponBrain.GetWeaponCategories(), weaponBrain.GetWeaponTypes());
                }
            }
            else
            {
                OnStopMeleeAttack?.Invoke();
            }
        }
    }

    private void GetRange()
    {
        for (int i = 0; i == colliders.Length; i++)
        {
            ColRange[i] = colliders[i].transform.position;
        }
    }

    private void SetRange()
    {
        for (int i = 0; i == colliders.Length; i++)
        {
            colliders[i].transform.position = new Vector3(ColRange[i].x, ColRange[i].y, weaponBrain.GetThisWeapon().AttackRange);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (input.GetAttackHold() == 1 && gameObject.activeInHierarchy && !InventoryUIHandler.Instance.IsInventoryON)
        {
            if (WeaponInventory.Instance.IsAttacking == false && collision != null)
            {
                if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.GetComponentInParent<Target>())
                {
                    Target target = collision.gameObject.GetComponentInParent<Target>();
                    if (target != null && target.IsDead == false && target.GetEnemy() == true)
                    {
                        WeaponInventory.Instance.IsAttacking = true;
                        weaponBrain.GetThisWeapon().DoAttack(target, target.GetEBFromTarget().GetThisEnemy().DodgeChance);
                        StartCoroutine(Attacking(() => { WeaponInventory.Instance.IsAttacking = false; }));
                    }
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (input.GetAttackHold() == 1 && gameObject.activeInHierarchy && !InventoryUIHandler.Instance.IsInventoryON)
        {
            if (WeaponInventory.Instance.IsAttacking == false && collision != null)
            {
                if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.activeInHierarchy)
                {
                    collision.gameObject.TryGetComponent<Target>(out Target target);
                    if (target != null && target.IsDead == false && target.GetEnemy() == true)
                    {
                        WeaponInventory.Instance.IsAttacking = true;
                        weaponBrain.GetThisWeapon().DoAttack(target, target.GetEBFromTarget().GetThisEnemy().DodgeChance);
                        StartCoroutine(Attacking(() => { WeaponInventory.Instance.IsAttacking = false; }));
                    }
                }
            }
        }
    }

    private IEnumerator Attacking(Action action)
    {
        if (WeaponInventory.Instance.IsAttacking == true)
        {
            yield return new WaitForSeconds(weaponBrain.GetThisWeapon().AttackSpeed);
            action.Invoke();
        }
    }

}




