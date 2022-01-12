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
    private float animDelay;    
    public float AnimDelay { get => animDelay; }
    private enum AnimType
    {
        Idle,
        Walk,
        Attack,
        Reload
    }

    //~~~~~~~~~~~~~~ Animation State ~~~~~~~~~~~~~~

    private int AttackHash = Animator.StringToHash("Attack");
    private int IdleHash = Animator.StringToHash("Idle");
    private int ReloadHash = Animator.StringToHash("Reload");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();       
    }

    private void OnEnable()
    {
        StartCoroutine(SetWeapon());
        Weapons.OnPlayerAttack += Weapons_OnAttack;
        Weapons.OnPlayerReload += Weapons_OnPlayerReload;
        IsAttacking = false;       
        RayGun.OnStopRayShoot += RayGun_OnStopShoot;
        MeleeAttacker.OnStopMeleeAttack += MeleeAttacker_OnStopMeleeAttack;
        ProjectileGun.OnStopProjectileShoot += ProjectileGun_OnStopProjectileShoot;
        RayShotGun.OnStopSGShoot += RayShotGun_OnStopSGShoot;
        animType = AnimType.Walk;
        playingAnim = false;
    }

    private void OnDisable()
    {
        Weapons.OnPlayerAttack -= Weapons_OnAttack;
        Weapons.OnPlayerReload -= Weapons_OnPlayerReload;
        RayGun.OnStopRayShoot -= RayGun_OnStopShoot;
        MeleeAttacker.OnStopMeleeAttack -= MeleeAttacker_OnStopMeleeAttack;
        ProjectileGun.OnStopProjectileShoot -= ProjectileGun_OnStopProjectileShoot;
        RayShotGun.OnStopSGShoot -= RayShotGun_OnStopSGShoot;
    }

    private void Update()
    {
        PlayAnim();
        AudioManager.Instance.PlayWeaponSound(IsAttacking, weaponTypes);
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
            case ImpactTypes.ShotgunHit:
                return "ShotgunMark_vfx";
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
                ChangeAnimState(IdleHash);
                playingAnim = false;
            }
            else if (animType == AnimType.Attack)
            {
                ChangeAnimState(AttackHash);
                StartCoroutine(ApplyDelay());
            }
            else if (animType == AnimType.Reload && weapon.ThisWeaponSO.IsRanged)
            {
                ChangeAnimState(ReloadHash);
                StartCoroutine(ApplyDelay());
            }
        }        
    }

    private void ChangeAnimState(int animHash)
    {
        if (currentAminHash == animHash) return;
        playingAnim = true;
        animator.PlayInFixedTime(animHash);
        currentAminHash = animHash;     
    }

    private IEnumerator ApplyDelay()
    {
        yield return new WaitForEndOfFrame();
        animDelay = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        if(animType == AnimType.Attack)
        {
            StartCoroutine(DelayAtkAnim(animDelay));
        }
        else if (animType == AnimType.Reload)
        {
            StartCoroutine(DelayReloadAnim(animDelay));
        }
    }

    private IEnumerator DelayAtkAnim(float animDelay)
    {
        yield return new WaitForSeconds(animDelay);
        playingAnim = false;
        if (weaponTypes != WeaponTypes.Rifle)
        {
            animType = AnimType.Walk;
        }
    }

    private IEnumerator DelayReloadAnim(float animDelay)
    {
        yield return new WaitForSeconds(animDelay);
        playingAnim = false;
        animType = AnimType.Walk;
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

    private void Weapons_OnPlayerReload(object sender, OnPlayerReloadEventArg e)
    {
        if (animType != AnimType.Reload)
        {
            animType = AnimType.Reload;
        }
    }

    private void RayGun_OnStopShoot()
    {
        IsAttacking = false;
        if (animType != AnimType.Walk)
            animType = AnimType.Walk;        
    }

    private void MeleeAttacker_OnStopMeleeAttack()
    {
             
    }

    private void ProjectileGun_OnStopProjectileShoot()
    {
        
    }

    private void RayShotGun_OnStopSGShoot()
    {
        
    }

}
