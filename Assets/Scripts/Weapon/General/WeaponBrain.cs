using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBrain : MonoBehaviour
{
    [SerializeField] private WeaponTypes weaponTypes;
    [SerializeField] private WeaponCategories weaponCategories;
    private Animator animator;
    private Weapons weapon;
    private AnimType animType;
    private int currentAminHash;
    private bool playingAnim;
    private bool IsAttacking;
    private bool IsReady = false;
    private enum AnimType
    {
        Idle,
        Walk,
        Attack
    }

    //~~~~~~~~~~~~~~ Animation State ~~~~~~~~~~~~~~

    private int AttackHash = Animator.StringToHash("Attack");
    private int WalkHash = Animator.StringToHash("Walk");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();       
    }

    private void OnEnable()
    {
        StartCoroutine(SetWeapon());
        Weapons.OnPlayerAttack += Weapons_OnAttack;
        IsAttacking = false;       
        RayGun.OnStopRayShoot += RayGun_OnStopShoot;
        MeleeAttacker.OnStopMeleeAttack += MeleeAttacker_OnStopMeleeAttack;
        ProjectileGun.OnStopProjectileShoot += ProjectileGun_OnStopProjectileShoot;
        animType = AnimType.Walk;
        playingAnim = false;
    }

    private void OnDisable()
    {
        Weapons.OnPlayerAttack -= Weapons_OnAttack; 
        RayGun.OnStopRayShoot -= RayGun_OnStopShoot;
        MeleeAttacker.OnStopMeleeAttack -= MeleeAttacker_OnStopMeleeAttack;
        ProjectileGun.OnStopProjectileShoot -= ProjectileGun_OnStopProjectileShoot;
    }

    private void Update()
    {
        PlayAnim();
        PlayWeaponSound();
    }

    private IEnumerator SetWeapon()
    {
        yield return new WaitForSeconds(1f);
        if (weapon == null)
        {
            weapon = new Weapons(this, weaponTypes);
            IsReady = true;
        }              
    }
    public Weapons GetThisWeapon()
    {
        return weapon;
    }
    public WeaponTypes GetWeaponTypes()
    {
        return weaponTypes;
    }
    public WeaponCategories GetWeaponCategories()
    {
        return weaponCategories;
    }
    public bool IsWeaponReady()
    {
        return IsReady;
    }

    private string GetWeaponName()
    {
        switch (weapon.ThisWeaponSO.ImpactType)
        {
            case ImpactTypes.BulletHole:
                return "BulletMark_vfx";
            case ImpactTypes.FireMark:
                return "FireMark_vfx";
            case ImpactTypes.None:
                return null;
            default: return null;
        }
    }

    public void SpawnHitVfx(Vector3 HitPointPos)
    {
        AssetRefLoader.CreateAndReleaseAsset(GetWeaponName(), HitPointPos, 0.3f);
    }

    //~~~~~~~~~~~~~~~~~~ Weaapon Animation ~~~~~~~~~~~~~~~~~~

    private void PlayAnim()
    {
        if (!playingAnim)
        {
            if (animType == AnimType.Walk)
            {
                ChangeAnimState(WalkHash);
                playingAnim = false;
            }
            else if (animType == AnimType.Attack)
            {
                ChangeAnimState(AttackHash);
                StartCoroutine(ApplyDelay(AnimType.Walk));
            }
        }        
    }

    private void PlayWeaponSound()
    {
        AudioManager.Instance.PlayRifleSound(IsAttacking, weaponTypes);
    }

    private void ChangeAnimState(int animHash)
    {
        if (currentAminHash == animHash) return;
        playingAnim = true;
        animator.PlayInFixedTime(animHash);
        currentAminHash = animHash;     
    }

    private IEnumerator ApplyDelay(AnimType anim)
    {
        yield return new WaitForEndOfFrame();
        float animDelay = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(DelayAnim(anim, animDelay));
    }

    private IEnumerator DelayAnim(AnimType anim, float animDelay)
    {
        yield return new WaitForSeconds(animDelay);
        playingAnim = false; 
        if (weaponTypes == WeaponTypes.RocketLauncher)
        {
            animType = AnimType.Walk;
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~ Event Callback ~~~~~~~~~~~~~~~~~~~~~

    private void Weapons_OnAttack(object sender, OnPlayerAttackEventArg e)
    {
        IsAttacking = true;
        if (animType != AnimType.Attack)
        {
            animType = AnimType.Attack;
        }
    }

    private void RayGun_OnStopShoot()
    {
        animType = AnimType.Walk;
        IsAttacking = false;
    }

    private void MeleeAttacker_OnStopMeleeAttack()
    {
        animType = AnimType.Walk;
        IsAttacking = false;
    }

    private void ProjectileGun_OnStopProjectileShoot()
    {
        animType = AnimType.Walk;
        IsAttacking = false;
    }

}
