using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeleeVisual : MonoBehaviour
{
    #region Variables

    private string bloodVfx = "Blood_burst_vfx";
    private ParticleSystem PS;
    private MeleeAttacker meleeAttacker;
    private WeaponBrain weaponBrain;
    private WeaponManager weaponManager;
    private GameController gameController;
    private float bloodOffset = 5;

    #endregion

    #region General

    private void Awake()
    {
        PS = GetComponentInChildren<ParticleSystem>();
        meleeAttacker = GetComponentInParent<MeleeAttacker>();
        weaponBrain = GetComponentInParent<WeaponBrain>();
    }

    private void Start()
    {
        weaponManager = WeaponManager.Instance;
        gameController = GameController.Instance;
    }

    #endregion

    #region Mechanics

    public void TriggerEvent()
    {
        if (!PS.isPlaying)
        {
            PS.Play();
        }
    }

    public void RegisterMeleeHit()
    {
        if (meleeAttacker.MeleeCollision != null)
        {
            if (meleeAttacker.IsHittingEnemy)
            {
                InvokeAttack(false);
            }
            if (meleeAttacker.IsHittingEnemyHead)
            {
                InvokeAttack(true);
            }
            if (meleeAttacker.IsHittingNPC)
            {
                NPCBrain npc = meleeAttacker.MeleeCollision.gameObject.GetComponent<NPCBrain>();
                StartCoroutine(gameController.HighlightNPCSpeech(npc.SpeechBubblePos, npc.GetDialogueText()));
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

    private void InvokeAttack(bool isCrit)
    {
        Target target = meleeAttacker.MeleeCollision.gameObject.GetComponentInParent<Target>();
        if (target.enemyCore != null && target.GetEnemy() == true && target.IsDead == false)
        {
            var contactPoint = meleeAttacker.MeleeCollision.GetContact(0);
            AssetLoader.CreateAndReleaseAsset(bloodVfx, contactPoint.point + contactPoint.normal * bloodOffset, 1);
            weaponManager.IsAttacking = true;
            weaponBrain.GetThisWeapon().DoAttack(target, target.enemyCore.EnemyDodgeChance, isCrit);
            StartCoroutine(Attacking(() => { weaponManager.IsAttacking = false; }));
        }
    }

    #endregion
}
