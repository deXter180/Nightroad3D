using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile :  MonoBehaviour
{
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private float MaxLifeTime;
    [SerializeField] private ProjectileTypes projectileType;
    private float LifeTime;
    private Weapons AttackWeapon;

    private void OnEnable()
    {
        LifeTime = 0f;
        Weapons.OnAttack += Weapons_OnAttack;
    }

    private void OnDisable()
    {
        Weapons.OnAttack -= Weapons_OnAttack;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * ProjectileSpeed * Time.deltaTime);
        LifeTime += Time.deltaTime;
        if (LifeTime > MaxLifeTime)
        {
            ObjectPooler.Instance.ReturnToPool(this);
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
            if (collision.gameObject.CompareTag("Enemy"))
            {
                ObjectPooler.Instance.ReturnToPool(this);
                collision.gameObject.TryGetComponent<Target>(out Target target);
                collision.gameObject.TryGetComponent<EnemyBrain>(out EnemyBrain enemyBrain);
                if (target != null && enemyBrain != null && target.GetEnemy() == true && target.IsDead == false && AttackWeapon != null)
                {
                    AttackWeapon.DoAttack(target, enemyBrain.GetThisEnemy().DodgeChance);
                }
            }
        }
    }
    private void Weapons_OnAttack(object sender, OnAttackEventArg e)
    {
        if (e != null && e.weaponCategory == WeaponCategories.ProjectileShoot)
        {
            AttackWeapon = e.weapon;
        }
    }

}
public enum ProjectileTypes
{
    Bullet, 
    FireBall
}





    