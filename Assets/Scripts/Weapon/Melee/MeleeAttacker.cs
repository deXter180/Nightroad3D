using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttacker : MonoBehaviour
{
    private Collider col;
    private Vector3 ColRange;
    private WeaponBrain weaponBrain;
    private Input input;
    public static event Action OnStopMeleeAttack;
    private bool SetupCompleted = false;

    private void Awake()
    {
        col = GetComponentInChildren<Collider>();
        weaponBrain = GetComponent<WeaponBrain>();
        input = FindObjectOfType<InputControl>();       
    }

    private void OnEnable()
    {
        GetRange();
        StartCoroutine(SetRange());
    }

    private void FixedUpdate()
    {
        StartAttack();       
    }

    private void GetRange()
    {
        ColRange = col.transform.localPosition;
    }

    private IEnumerator SetRange()
    {
        yield return new WaitUntil(() => weaponBrain.IsWeaponReady());
        col.transform.localPosition = new Vector3(ColRange.x, ColRange.y, ColRange.z + weaponBrain.GetThisWeapon().ThisWeaponSO.AttackRange);
        SetupCompleted = true;
    }

    private void StartAttack()
    {
        if (gameObject.activeInHierarchy && weaponBrain.IsWeaponReady())
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

    private void OnCollisionEnter(Collision collision)
    {
        if (input.GetAttackHold() == 1 && gameObject.activeInHierarchy && !InventoryUIHandler.Instance.IsInventoryON)
        {
            if (WeaponInventory.Instance.IsAttacking == false && collision != null)
            {
                if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.GetComponentInParent<Target>())
                {
                    Target target = collision.gameObject.GetComponentInParent<Target>();
                    if (target != null && target.GetEBFromTarget() != null)
                    {
                        if (target.IsDead == false && target.GetEnemy() == true)
                        {
                            WeaponInventory.Instance.IsAttacking = true;
                            weaponBrain.GetThisWeapon().DoAttack(target, target.GetEBFromTarget().GetThisEnemy().ThisEnemySO.DodgeChance);
                            StartCoroutine(Attacking(() => { WeaponInventory.Instance.IsAttacking = false; }));
                        }                        
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
                    if (target != null && target.GetEBFromTarget() != null)
                    {
                        if (target.IsDead == false && target.GetEnemy() == true)
                        {
                            WeaponInventory.Instance.IsAttacking = true;
                            weaponBrain.GetThisWeapon().DoAttack(target, target.GetEBFromTarget().GetThisEnemy().ThisEnemySO.DodgeChance);
                            StartCoroutine(Attacking(() => { WeaponInventory.Instance.IsAttacking = false; }));
                        }                        
                    }
                }
            }
        }
    }

    private IEnumerator Attacking(Action action)
    {
        if (WeaponInventory.Instance.IsAttacking == true)
        {
            yield return new WaitForSeconds(weaponBrain.GetThisWeapon().ThisWeaponSO.AttackSpeed);
            action.Invoke();
        }
    }

}




