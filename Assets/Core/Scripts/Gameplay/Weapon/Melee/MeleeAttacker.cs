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
    private GameController gameController;
    private bool isHitting;
    public static event Action OnStopMeleeAttack;

    private void Awake()
    {
        col = GetComponentInChildren<Collider>();
        weaponBrain = GetComponent<WeaponBrain>();
        StartCoroutine(SetRange());
        IEnumerator SetRange()
        {
            yield return new WaitUntil(() => weaponBrain.IsWeaponReady());
            col.transform.localPosition = new Vector3(ColRange.x, ColRange.y, ColRange.z + weaponBrain.GetThisWeapon().ThisWeaponSO.AttackRange);
        }
    }

    private void Start()
    {
        StartCoroutine(InputDone());
        isHitting = false;
        GetRange();
        gameController = GameController.Instance;
        weaponManager = WeaponManager.Instance;
        IEnumerator InputDone()
        {
            yield return new WaitUntil(() => InputManager.InputReady);
            inputs = InputManager.InputActions;
            weaponBrain.GetThisWeapon();
        }
        void GetRange()
        {
            ColRange = col.transform.localPosition;
        }
    }

    private void Update()
    {
        if (gameController != null && inputs != null)
        {
            if (!gameController.IsMainMenuActive && !gameController.IsInventoryActive && !gameController.IsStashActive)
            {
                StartAttack();
            }
        }                    
    } 

    private void StartAttack()
    {
        if (gameObject.activeInHierarchy && weaponBrain.IsWeaponReady())
        {
            if (inputs.BasicControls.Shoot.ReadValue<float>() == 1)
            {
                if (!weaponManager.IsAttacking)
                {
                    weaponBrain.GetThisWeapon().RaiseOnPlayerAttack(weaponBrain.GetThisWeapon(), false, weaponBrain, weaponBrain.GetWeaponCategories(), weaponBrain.GetWeaponTypes());              
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
        if (!gameController.IsInventoryActive && !gameController.IsMainMenuActive && !gameController.IsStashActive)
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
                                weaponBrain.GetThisWeapon().DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance);
                                StartCoroutine(Attacking(() => { weaponManager.IsAttacking = false; }));
                            }
                        }
                    }
                }
            }
        }      
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!gameController.IsInventoryActive && !gameController.IsMainMenuActive && !gameController.IsStashActive)
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
                                weaponBrain.GetThisWeapon().DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance);
                                StartCoroutine(Attacking(() => { weaponManager.IsAttacking = false; }));
                            }
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
            yield return Helpers.GetWait(weaponBrain.GetThisWeapon().ThisWeaponSO.AttackDelay);
            action.Invoke();
        }
    }

    private IEnumerator PlayVfx()
    {
        yield return Helpers.GetWait(weaponBrain.AnimDelay);
    }

}




