using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.PlayerLoop;

public class ProjectileGun : MonoBehaviour
{
    [SerializeField] private Transform FiringPoint;
    private WeaponBrain weaponBrain;
    private Input input;
    

    private void Awake()
    {
        weaponBrain = GetComponent<WeaponBrain>();
        input = GetComponentInParent<InputControl>();
    }

    private void Update()
    {
        if (input.GetAttack() == 1 && gameObject.activeInHierarchy)
        {
            if (Inventory.Instance.IsAttacking == false)
            {
                StartCoroutine(Shoot(() => { Inventory.Instance.IsAttacking = false; }));
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
            Inventory.Instance.IsAttacking = true;
            var shot = ObjectPooler.Instance.GetPooledObject(GetProjectile(weaponBrain.GetWeaponTypes()));
            shot.transform.rotation = FiringPoint.rotation;
            shot.transform.position = FiringPoint.position;
            shot.gameObject.SetActive(true);
            weaponBrain.GetThisWeapon().RaiseOnAttack(weaponBrain.GetThisWeapon(), weaponBrain.GetWeaponCategories(), weaponBrain.GetWeaponTypes());
            yield return new WaitForSeconds(weaponBrain.GetThisWeapon().AttackSpeed);
            action.Invoke();
        }
    }
}
