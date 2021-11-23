using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile :  MonoBehaviour
{
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private float MaxLifeTime;
    [SerializeField] private ProjectileTypes projectileType;
    private Vector3 firePosition;
    private float LifeTime;
    private Weapons AttackWeapon;
    private WeaponBrain weaponBrain;

    private void OnEnable()
    {
        LifeTime = 0f;
        Weapons.OnPlayerAttack += Weapons_OnAttack;
    }

    private void OnDisable()
    {
        Weapons.OnPlayerAttack -= Weapons_OnAttack;
    }

    private void Update()
    {
        if (AttackWeapon != null)
        {
            transform.Translate(Vector3.forward * ProjectileSpeed * Time.deltaTime);
            LifeTime += Time.deltaTime;
            if (LifeTime > MaxLifeTime || transform.position.z >= firePosition.z + AttackWeapon.ThisWeaponSO.AttackRange)
            {
                if (ObjectPooler.Instance != null)
                    ObjectPooler.Instance.ReturnToPool(this);
            }
        }            
    }

    public ProjectileTypes GetProjectileType()
    {
        return projectileType;
    }

    //~~~~~~~~~~~~~~ Event Callbacks ~~~~~~~~~~~~~~~~~

    private void OnCollisionEnter(Collision collision)
    {
        ObjectPooler.Instance.ReturnToPool(this);
           if (collision != null)
           {
                if (collision.gameObject.GetComponentInParent<Target>() != null)
                {
                    Target target = collision.gameObject.GetComponentInParent<Target>();
                    if (target.enemyBrain != null && target.GetEnemy() == true && target.IsDead == false && AttackWeapon != null)
                    {
                        if (collision.gameObject.CompareTag("Enemy"))
                        {
                            AttackWeapon.DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance, false);
                            if (!target.Dodging)
                            {

                            }
                        }   
                        else if (collision.gameObject.CompareTag("Head"))
                        {
                            AttackWeapon.DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance, true);
                            if (!target.Dodging)
                            {

                            }
                        }
                    }
                }
                ContactPoint contactPoint = collision.GetContact(0);
                if (weaponBrain != null)
                {
                    weaponBrain.SpawnHitVfx(contactPoint.point + new Vector3(0, 0, -5f));
                }
                
           }
    } 

    //~~~~~~~~~~~~~~~~~~~ Event Callback ~~~~~~~~~~~~~~~~~~~

    private void Weapons_OnAttack(object sender, OnPlayerAttackEventArg e)
    {
        if (e != null && e.weaponCategory == WeaponCategories.ProjectileShoot)
        {          
            AttackWeapon = e.weapon;
            weaponBrain = e.weaponBrain;
            firePosition = e.weaponBrain.transform.position;
        }
    }

}






    