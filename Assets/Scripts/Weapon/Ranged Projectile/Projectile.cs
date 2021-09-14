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
        Weapons.OnPlayerAttack += Weapons_OnAttack;
    }

    private void OnDisable()
    {
        Weapons.OnPlayerAttack -= Weapons_OnAttack;
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
                if (collision.gameObject.GetComponentInParent<Target>() && collision.gameObject.CompareTag("Enemy"))
                {
                    Target target = collision.gameObject.GetComponentInParent<Target>();
                    collision.gameObject.TryGetComponent<EnemyBrain>(out EnemyBrain enemyBrain);
                    if (enemyBrain != null && target.GetEnemy() == true && target.IsDead == false && AttackWeapon != null)
                    {
                        AttackWeapon.DoAttack(target, enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance);
                    }
                }
                ContactPoint contactPoint = collision.GetContact(0);
                if (AssetCollections.Instance.GetImpactAssetRef(ImpactTypes.FireMark) != null)
                {
                    List<GameObject> objs = AssetCollections.InstantiateAndGetAssetsByAssetRef(AssetCollections.Instance.GetImpactAssetRef(ImpactTypes.FireMark));
                    if (objs != null && objs.Count > 0)
                    {
                        GameObject bHole = objs[0];
                        if (contactPoint.otherCollider.CompareTag("Enemy"))
                        {
                       
                        }
                        else
                        {
                            
                        }
                        bHole.transform.SetParent(contactPoint.otherCollider.transform);
                        bHole.transform.rotation = Quaternion.identity;
                        bHole.transform.position = contactPoint.point + contactPoint.normal * 0.05f;
                        bHole.transform.LookAt(contactPoint.point + contactPoint.normal);                        
                        Destroy(bHole, 1.5f);
                    }
                                       
                }
           }
    } 

    //~~~~~~~~~~~~~~~~~~~ Event Callback ~~~~~~~~~~~~~~~~~~~

    private void Weapons_OnAttack(object sender, OnPlayerAttackEventArg e)
    {
        if (e != null && e.weaponCategory == WeaponCategories.ProjectileShoot)
        {
            AttackWeapon = e.weapon;
        }
    }

}






    