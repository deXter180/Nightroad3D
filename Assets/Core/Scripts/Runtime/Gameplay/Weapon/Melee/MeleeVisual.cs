using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeleeVisual : MonoBehaviour
{
    #region Variables

    //private string bloodVfx = "Blood_burst_vfx";
    //private ParticleSystem PS;
    private Collider col;
    private Animator animator;
    private TimeStop timeStop;
    private MeleeAttacker meleeAttacker;
    private WeaponBrain weaponBrain;
    private WeaponManager weaponManager;
    private GameController gameController;
    private PlayerController playerController;
    private string shockwaveVfx = "Shockwave_vfx";
    private string bloodVfx = "Blood_burst_vfx";
    public event Action OnStopMeleeAttack;

    //~~~~~~~~~~~~~~ Animation State ~~~~~~~~~~~~~~

    private int AttackHash = Animator.StringToHash("IsAttacking");
    private int AltAttackHash = Animator.StringToHash("IsAltAttacking");
    private int VelocityHash = Animator.StringToHash("Velocity");

    #endregion

    #region General

    private void Awake()
    {
        //PS = GetComponentInChildren<ParticleSystem>();
        animator = GetComponent<Animator>();
        col = GetComponentInChildren<Collider>();
        meleeAttacker = GetComponentInChildren<MeleeAttacker>();
        weaponBrain = GetComponent<WeaponBrain>();
    }

    private void Start()
    {
        col.enabled = true;
        EndAttackAnim();
        EndAltAttackAnim();
        timeStop = TimeStop.Instance;
        playerController = PlayerController.Instance;
        weaponManager = WeaponManager.Instance;
        gameController = GameController.Instance;
    }

    private void OnEnable()
    {
        meleeAttacker.OnStartMeleeAttack += MeleeAttacker_OnStartMeleeAttack;
        meleeAttacker.OnStartAltMeleeAttack += MeleeAttacker_OnStartAltMeleeAttack;
    }

    private void OnDisable()
    {
        meleeAttacker.OnStartMeleeAttack -= MeleeAttacker_OnStartMeleeAttack;
        meleeAttacker.OnStartAltMeleeAttack -= MeleeAttacker_OnStartAltMeleeAttack;
    }

    private void Update()
    {
        RegisterMeleeHit();        
    }

    private void LateUpdate()
    {
        animator.SetFloat(VelocityHash, playerController.PlayerVelocity);
    }

    #endregion

    #region Mechanics

    //public void TriggerEvent()
    //{
    //    if (!PS.isPlaying)
    //    {
    //        PS.Play();
    //    }
    //}

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
            meleeAttacker.ResetHits();
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
        Target target = meleeAttacker.MeleeCollision.collider.GetComponentInParent<Target>();
        if (target == null)
        {
            return;
        }
        if (target.enemyCore == null)
        {
            return;
        }
        if (target.GetEnemy() == true && target.IsDead == false)
        {
            var contactPoint = meleeAttacker.MeleeCollision.GetContact(0);
            AssetLoader.CreateAndReleaseAsset(shockwaveVfx, contactPoint.point + contactPoint.normal * -0.5f, 0.5f);
            AudioManager.PlayWeaponSoundOnce(weaponBrain.GetWeaponTypes(), 1);
            timeStop.StopTime(0.05f, 20, 0.08f);
            weaponManager.SetIsAttacking(true);
            weaponBrain.GetThisWeapon().DoAttack(target, target.enemyCore.EnemyDodgeChance, isCrit);
            AssetLoader.CreateAndReleaseAsset(bloodVfx, contactPoint.point + contactPoint.normal * -0.5f, 0.5f);
            StartCoroutine(Attacking(() => { weaponManager.SetIsAttacking(false); }));
        }
    }

    #endregion
     
    //~~~~~~~~~~~~~~~~~~ Weaapon Animation ~~~~~~~~~~~~~~~~~~

    #region AnimationControl

    public void EnableCollider() //Calling from Anim event
    {
        col.enabled = true;
    }

    public void DisableCollider() //Calling from Anim event
    {
        col.enabled = false;
        EndAttackAnim();
        EndAltAttackAnim();
        meleeAttacker.ResetHits();
        OnStopMeleeAttack?.Invoke();
    }

    public void PlaySwingAudio()  //Calling from Anim event
    {
        AudioManager.PlayWeaponSoundOnce(weaponBrain.GetWeaponTypes(), 0);
    }

    private void EndAttackAnim()
    {
        animator.SetBool(AttackHash, false);
    }

    private void EndAltAttackAnim()
    {
        animator.SetBool(AltAttackHash, false);
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void MeleeAttacker_OnStartMeleeAttack()
    {
        animator.SetBool(AttackHash, true);
    }

    private void MeleeAttacker_OnStartAltMeleeAttack()
    {
        animator.SetBool(AttackHash, true);
        animator.SetBool(AltAttackHash, true);
    }

    #endregion
}
