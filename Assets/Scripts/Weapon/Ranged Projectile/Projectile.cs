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
                if (collision.gameObject.GetComponent<Target>() != null)
                {
                    //ObjectPooler.Instance.ReturnToPool(this);
                    collision.gameObject.TryGetComponent<Target>(out Target target);
                    collision.gameObject.TryGetComponent<EnemyBrain>(out EnemyBrain enemyBrain);
                    if (enemyBrain != null && target.GetEnemy() == true && target.IsDead == false && AttackWeapon != null)
                    {
                        AttackWeapon.DoAttack(target, enemyBrain.GetThisEnemy().DodgeChance);
                        if (target.IsDead == false)
                        {
                            ContactPoint contactPoint = collision.GetContact(0);
                            GameObject bHoleOnEnemy = Instantiate(ObjectPooler.Instance.ProjectileImpacts[1].ImpactSprite, contactPoint.point + contactPoint.normal * 0.0001f, Quaternion.identity) as GameObject;
                            bHoleOnEnemy.transform.LookAt(contactPoint.point + contactPoint.normal);
                            Destroy(bHoleOnEnemy, 2f);
                        }
                    }
                }
                else
                {
                    ContactPoint contactPoint = collision.GetContact(0);
                    GameObject bHole = Instantiate(ObjectPooler.Instance.ProjectileImpacts[1].ImpactSprite, contactPoint.point + contactPoint.normal * 0.01f, Quaternion.identity) as GameObject;
                    bHole.transform.LookAt(contactPoint.point + contactPoint.normal);
                    Destroy(bHole, 2f);
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





    