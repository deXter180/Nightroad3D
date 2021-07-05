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
    private MeleeWeapns meleeWeapn;
    private RangedWeapons rangedWeapon;
    private AnimType animType;
    private int currentAminHash;
    private bool playingAnim;
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
        if (SetWeapon() != null)
        {
            SetWeapon();
        }
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        Weapons.OnPlayerAttack += Weapons_OnAttack;
        RayGun.OnStopShoot += RayGun_OnStopShoot;
        animType = AnimType.Walk;
        playingAnim = false;
    }

    private void OnDisable()
    {
        Weapons.OnPlayerAttack -= Weapons_OnAttack; 
        RayGun.OnStopShoot -= RayGun_OnStopShoot;
    }

    private void Update()
    {
        PlayAnim();
    }

    private Weapons SetWeapon()
    {
        weapon = WeaponFactory.GetWeapon(weaponTypes, out MeleeWeapns melee, out RangedWeapons ranged);
        if (melee != null)
        {
            meleeWeapn = melee;
        }
        else if (ranged != null)
        {
            rangedWeapon = ranged;
        }
        return weapon;
    }
    public Weapons GetThisWeapon()
    {
        return weapon;
    }
    public MeleeWeapns GetIfMelee()
    {
        if (meleeWeapn != null)
        {
            return meleeWeapn;
        }
        else return default;
    }
    public RangedWeapons GetIfRanged()
    {
        if (rangedWeapon != null)
        {
            return rangedWeapon;
        }
        else return default;
    }
    public WeaponTypes GetWeaponTypes()
    {
        return weaponTypes;
    }
    public WeaponCategories GetWeaponCategories()
    {
        return weaponCategories;
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

            //Debug.Log(animType);
        }        
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
        if (weaponTypes != WeaponTypes.Rifle)
        {
            animType = anim;
        }       
    }

    //~~~~~~~~~~~~~~~~~~~~~ Event Callback ~~~~~~~~~~~~~~~~~~~~~

    private void Weapons_OnAttack(object sender, OnPlayerAttackEventArg e)
    {
        if (animType != AnimType.Attack)
        {
            animType = AnimType.Attack;
        }
        if (e.weaponType == WeaponTypes.Rifle)
        {
            //AudioManager.PlaySound();
        }
    }

    private void RayGun_OnStopShoot()
    {
        animType = AnimType.Walk;
        AudioManager.IsPlaySound = false;
    }

}
