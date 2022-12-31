using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttacker : MonoBehaviour
{
    #region Variables

    private Collider col;
    private Vector3 ColRange;
    private WeaponBrain weaponBrain;
    private WeaponManager weaponManager;
    private PlayerInputAsset inputs;
    private GameController gameController;
    private bool isHitting;
    private string enemyName = "Enemy";
    private string enemyHeadName = "EnemyHead";
    private string npcName = "NPC";
    public static event Action OnStopMeleeAttack;
    public bool IsHittingEnemy { get; private set; }
    public bool IsHittingEnemyHead { get; private set; }
    public bool IsHittingNPC { get; private set; }

    public Collision MeleeCollision { get; private set; }

    #endregion

    #region General 

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
        IsHittingEnemy = false;
        IsHittingEnemyHead = false;
        IsHittingNPC = false;
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
            if (!gameController.IsUIActive && !gameController.IsCastingSpell)
            {
                StartAttack();
            }
        }                    
    }    

    private void OnCollisionEnter(Collision collision)
    {
        if (!gameController.IsUIActive && !gameController.IsCastingSpell)
        {
            if (gameObject.activeInHierarchy && isHitting)
            {
                if (!weaponManager.IsAttacking && collision != null)
                {
                    MeleeCollision = collision;
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
                    
                }
            }
        }      
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!gameController.IsUIActive && !gameController.IsCastingSpell)
        {
            if (gameObject.activeInHierarchy && isHitting)
            {
                if (!weaponManager.IsAttacking && collision != null)
                {
                    MeleeCollision = collision;
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
                }
            }
        }
       
    }

    private void OnCollisionExit(Collision collision)
    {
        MeleeCollision = null;
        IsHittingEnemy = false;
        IsHittingEnemyHead = false;
        IsHittingNPC = false;
    }

    #endregion

    #region Mechanics

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
                    IsHittingEnemy = false;
                    IsHittingEnemyHead = false;
                    IsHittingNPC = false;
                }
            }
            else
            {
                OnStopMeleeAttack?.Invoke();
                isHitting = false;
                IsHittingEnemy = false;
                IsHittingEnemyHead = false;
                IsHittingNPC = false;
            }
        }
    }

    #endregion

}




