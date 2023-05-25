using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttacker : MonoBehaviour
{
    #region Variables

    private WeaponBrain weaponBrain;
    private Weapons thisWeapon;
    private WeaponManager weaponManager;
    private PlayerInputAsset inputs;
    private GameController gameController;
    private bool isHitting;
    private string enemyName = "Enemy";
    private string enemyHeadName = "EnemyHead";
    private string npcName = "NPC";
    public event Action OnStartMeleeAttack;
    public event Action OnStartAltMeleeAttack;
    public bool IsHittingSomething { get; private set; }
    public bool IsHittingEnemy { get; private set; }
    public bool IsHittingEnemyHead { get; private set; }
    public bool IsHittingNPC { get; private set; }
    public bool IsHittingStatic { get; private set; }

    public Collision MeleeCollision { get; private set; }

    #endregion

    #region General 

    private void Awake()
    {
        weaponBrain = GetComponentInParent<WeaponBrain>();        
    }

    private void Start()
    {
        StartCoroutine(InputDone());
        ResetHits();
        gameController = GameController.Instance;
        weaponManager = WeaponManager.Instance;
        IEnumerator InputDone()
        {
            yield return new WaitUntil(() => InputManager.InputReady);
            inputs = InputManager.InputActions;
            thisWeapon = weaponBrain.GetThisWeapon();
        }
    }

    private void Update()
    {
        if (gameController != null && inputs != null)
        {
            if (!gameController.IsUIActive && !gameController.IsCastingSpell)
            {
                StartAttack();
            }
        }                    
    }    

    public void ResetHits()
    {
        isHitting = false;
        IsHittingSomething = false;
        IsHittingEnemy = false;
        IsHittingEnemyHead = false;
        IsHittingNPC = false;
        IsHittingStatic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.activeInHierarchy && isHitting)
        {
            if (!weaponManager.IsAttacking && collision != null)
            {
                MeleeCollision = collision;
                IsHittingSomething = true;
                if (collision.gameObject.CompareTag(enemyName))
                {
                    if (collision.gameObject.GetComponentInParent<Target>() != null)
                    {
                        IsHittingEnemy = true;
                    }
                }
                else if (collision.gameObject.CompareTag(enemyHeadName))
                {
                    if (collision.gameObject.GetComponentInParent<Target>() != null)
                    {
                        IsHittingEnemyHead = true;
                    }
                }
                else if (collision.gameObject.CompareTag(npcName))
                {
                    if (collision.gameObject.GetComponent<NPCBrain>() != null)
                    {
                        IsHittingNPC = true;
                    }
                }
                else
                {
                    IsHittingStatic = true;
                }
            }
        }
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    if (gameObject.activeInHierarchy && isHitting)
    //    {
    //        if (!weaponManager.IsAttacking && collision != null)
    //        {
    //            MeleeCollision = collision;
    //            if (collision.gameObject.CompareTag(enemyName))
    //            {
    //                if (collision.gameObject.GetComponentInParent<Target>() != null)
    //                {
    //                    IsHittingEnemy = true;
    //                }
    //            }
    //            else if (collision.gameObject.CompareTag(enemyHeadName))
    //            {
    //                if (collision.gameObject.GetComponentInParent<Target>() != null)
    //                {
    //                    IsHittingEnemyHead = true;
    //                }
    //            }
    //            else if (collision.gameObject.CompareTag(npcName))
    //            {
    //                if (collision.gameObject.GetComponent<NPCBrain>() != null)
    //                {
    //                    IsHittingNPC = true;
    //                }
    //            }
    //        }
    //    }
    //}

    private void OnCollisionExit(Collision collision)
    {
        MeleeCollision = null;
        ResetHits();
    }

    #endregion

    #region Mechanics

    private void StartAttack()
    {
        if (gameObject.activeInHierarchy && weaponBrain.IsWeaponReady())
        {
            if (inputs.BasicControls.AltAttack.ReadValue<float>() > 0)
            {
                if (inputs.BasicControls.Shoot.triggered)
                {
                    if (IsAttackSuccessful())
                    {
                        OnStartAltMeleeAttack?.Invoke();
                    }
                }
            }
            else if (inputs.BasicControls.Shoot.triggered)
            {
                if (IsAttackSuccessful())
                {
                    OnStartMeleeAttack?.Invoke();
                }
            }
        }
    }

    private bool IsAttackSuccessful()
    {
        if (!weaponManager.IsAttacking)
        {
            if (!isHitting)
            {
                thisWeapon.RaiseOnPlayerAttack(thisWeapon, false, weaponBrain, weaponBrain.GetWeaponCategories(), weaponBrain.GetWeaponTypes());
                isHitting = true;
                return true;
            }            
        }
        return false;
    }

    #endregion
}




