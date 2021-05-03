using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.PlayerLoop;

public class ProjectileGun : MonoBehaviour
{
    [SerializeField] private Transform FiringPoint;
    private float LastTimeShot = 0;
    private float AttackDelay = 0.5f;
    private WeaponBrain weaponBrain;
    private Input input;
    public static event EventHandler<OnFireEventArg> OnProjectileFire;
    private bool IsFiring = false;
    public class OnFireEventArg
    {
        public Weapons weapon;
        public WeaponCategories weaponCategory;


        public OnFireEventArg(Weapons WP, WeaponCategories WC)
        {
            weapon = WP;
            weaponCategory = WC;
        }
    }

    private void Awake()
    {
        weaponBrain = GetComponent<WeaponBrain>();
        input = GetComponentInParent<InputControl>();
    }

    private void Update()
    {
        if (input.GetAttack() == 1 && gameObject.activeInHierarchy)
        {
            if (IsFiring == false)
            {
                StartCoroutine(Shoot(() => { IsFiring = false; }));
            }
        }
    }
    private ProjectileTypes GetProjectile(WeaponTypes weaponType)
    {
        switch (weaponType)
        {
            case WeaponTypes.RocketLauncher:
                {
                    return ProjectileTypes.FireBall;
                }
            default: return ProjectileTypes.Bullet;
        }
    }
    public IEnumerator Shoot(Action action) //Used in PlayerControl
    {
        if (ObjectPooler.Instance.GetPooledObject(GetProjectile(weaponBrain.GetWeaponTypes())) != null)
        {
            IsFiring = true;
            var shot = ObjectPooler.Instance.GetPooledObject(GetProjectile(weaponBrain.GetWeaponTypes()));
            shot.transform.rotation = FiringPoint.rotation;
            shot.transform.position = FiringPoint.position;
            shot.gameObject.SetActive(true);
            OnProjectileFire?.Invoke(this, new OnFireEventArg(weaponBrain.GetThisWeapon(), weaponBrain.GetWeaponCategories()));
            yield return new WaitForSeconds(weaponBrain.GetThisWeapon().AttackSpeed);
            action.Invoke();
        }
    }
}
