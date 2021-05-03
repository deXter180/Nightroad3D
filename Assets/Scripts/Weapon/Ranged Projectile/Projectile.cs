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
        ProjectileGun.OnProjectileFire += ProjectileGun_OnFire;
    }

    private void OnDisable()
    {
        ProjectileGun.OnProjectileFire -= ProjectileGun_OnFire;
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            
            ObjectPooler.Instance.ReturnToPool(this);
            collision.gameObject.TryGetComponent<Target>(out Target target);
            collision.gameObject.TryGetComponent<EnemyBrain>(out EnemyBrain enemyBrain);
            if (target != null && enemyBrain != null && target.GetEnemy() == true && target.IsDead == false)
            {
                AttackWeapon.DoAttack(target, enemyBrain.GetThisEnemy().DodgeChance);
            }
        }
    }
    private void ProjectileGun_OnFire(object sender, ProjectileGun.OnFireEventArg e)
    {
        if (e.weaponCategory == WeaponCategories.ProjectileShoot)
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





    