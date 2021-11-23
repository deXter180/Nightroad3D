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
    private bool isHitting;
    public static event Action OnStopMeleeAttack;

    private void Awake()
    {
        col = GetComponentInChildren<Collider>();
        weaponBrain = GetComponent<WeaponBrain>();
        input = FindObjectOfType<InputControl>();
        StartCoroutine(SetRange());
    }

    private void OnEnable()
    {
        isHitting = false;
        GetRange();
    }

    private void Update()
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
    }

    private void StartAttack()
    {
        if (gameObject.activeInHierarchy && weaponBrain.IsWeaponReady())
        {
            if (input.GetAttackHold() == 1 && !InventoryUIHandler.Instance.IsInventoryON)
            {
                if (!WeaponInventory.Instance.IsAttacking)
                {
                    weaponBrain.GetThisWeapon().RaiseOnPlayerAttack(weaponBrain.GetThisWeapon(), weaponBrain, weaponBrain.GetWeaponCategories(), weaponBrain.GetWeaponTypes());
                    isHitting = true;
                }
                else
                {
                    isHitting = false;
                }
            }
            else
            {
                OnStopMeleeAttack?.Invoke();
                isHitting = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.activeInHierarchy && !InventoryUIHandler.Instance.IsInventoryON && isHitting)
        {
            if (!WeaponInventory.Instance.IsAttacking && collision != null)
            {
                if (collision.gameObject.GetComponentInParent<Target>() != null)
                {
                    Target target = collision.gameObject.GetComponentInParent<Target>();
                    if (target.enemyBrain != null && target.GetEnemy() == true && target.IsDead == false)
                    {
                        if (collision.gameObject.CompareTag("Enemy"))
                        {
                            WeaponInventory.Instance.IsAttacking = true;
                            weaponBrain.GetThisWeapon().DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance, false);
                            StartCoroutine(Attacking(() => { WeaponInventory.Instance.IsAttacking = false; }));
                        }
                        //else if (collision.gameObject.CompareTag("Head"))
                        //{
                        //    WeaponInventory.Instance.IsAttacking = true;
                        //    weaponBrain.GetThisWeapon().DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance, true);
                        //    StartCoroutine(Attacking(() => { WeaponInventory.Instance.IsAttacking = false; }));
                        //}
                                              
                    }
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (gameObject.activeInHierarchy && !InventoryUIHandler.Instance.IsInventoryON && isHitting)
        {
            if (!WeaponInventory.Instance.IsAttacking && collision != null)
            {
                if (collision.gameObject.GetComponentInParent<Target>() != null)
                {
                    Target target = collision.gameObject.GetComponentInParent<Target>();
                    if (target.enemyBrain != null && target.GetEnemy() == true && target.IsDead == false)
                    {
                        if (collision.gameObject.CompareTag("Enemy"))
                        {
                            WeaponInventory.Instance.IsAttacking = true;
                            weaponBrain.GetThisWeapon().DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance, false);
                            StartCoroutine(Attacking(() => { WeaponInventory.Instance.IsAttacking = false; }));
                        }
                        //else if (collision.gameObject.CompareTag("Head"))
                        //{
                        //    WeaponInventory.Instance.IsAttacking = true;
                        //    weaponBrain.GetThisWeapon().DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance, true);
                        //    StartCoroutine(Attacking(() => { WeaponInventory.Instance.IsAttacking = false; }));
                        //}
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




