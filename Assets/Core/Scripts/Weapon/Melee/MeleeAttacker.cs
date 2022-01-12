using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttacker : MonoBehaviour
{
    private Collider col;
    private Vector3 ColRange;
    private WeaponBrain weaponBrain;
    private WeaponManager weaponManager;
    private PlayerInputAsset inputs;
    private bool isHitting;
    public static event Action OnStopMeleeAttack;

    private void Awake()
    {
        col = GetComponentInChildren<Collider>();
        weaponBrain = GetComponent<WeaponBrain>();
        StartCoroutine(SetRange());
    }

    private void OnEnable()
    {
        StartCoroutine(InputDone());    
        weaponManager = WeaponManager.Instance;
        isHitting = false;
        GetRange();
    }

    private void Update()
    {
        if (!InputMenuUIHandler.Instance.IsMainMenuActive && !InventoryUIHandler.Instance.IsInventoryActive)
        {
            StartAttack();
        }              
    }

    private IEnumerator InputDone()
    {
        yield return new WaitUntil(() => InputManager.InputReady);
        inputs = InputManager.InputActions;
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
            if (inputs.BasicControls.Shoot.ReadValue<float>() == 1)
            {
                if (!weaponManager.IsAttacking)
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
        if (!InventoryUIHandler.Instance.IsInventoryActive && !InputMenuUIHandler.Instance.IsMainMenuActive)
        {
            if (gameObject.activeInHierarchy && isHitting)
            {
                if (!weaponManager.IsAttacking && collision != null)
                {
                    if (collision.gameObject.GetComponentInParent<Target>() != null)
                    {
                        Target target = collision.gameObject.GetComponentInParent<Target>();
                        if (target.enemyBrain != null && target.GetEnemy() == true && target.IsDead == false)
                        {
                            if (collision.gameObject.CompareTag("Enemy"))
                            {
                                weaponManager.IsAttacking = true;
                                weaponBrain.GetThisWeapon().DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance, false);
                                StartCoroutine(Attacking(() => { weaponManager.IsAttacking = false; }));
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
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!InventoryUIHandler.Instance.IsInventoryActive && !InputMenuUIHandler.Instance.IsMainMenuActive)
        {
            if (gameObject.activeInHierarchy && isHitting)
            {
                if (!weaponManager.IsAttacking && collision != null)
                {
                    if (collision.gameObject.GetComponentInParent<Target>() != null)
                    {
                        Target target = collision.gameObject.GetComponentInParent<Target>();
                        if (target.enemyBrain != null && target.GetEnemy() == true && target.IsDead == false)
                        {
                            if (collision.gameObject.CompareTag("Enemy"))
                            {
                                weaponManager.IsAttacking = true;
                                weaponBrain.GetThisWeapon().DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance, false);
                                StartCoroutine(Attacking(() => { weaponManager.IsAttacking = false; }));
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
       
    }

    private IEnumerator Attacking(Action action)
    {
        if (weaponManager.IsAttacking == true)
        {
            yield return new WaitForSeconds(weaponBrain.GetThisWeapon().ThisWeaponSO.AttackSpeed);
            action.Invoke();
        }
    }

    private IEnumerator PlayVfx()
    {
        yield return new WaitForSeconds(weaponBrain.AnimDelay);
    }

}




