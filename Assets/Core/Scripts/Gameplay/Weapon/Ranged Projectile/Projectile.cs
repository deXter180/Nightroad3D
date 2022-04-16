using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile :  MonoBehaviour
{
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private float MaxLifeTime;
    [SerializeField] private ProjectileTypes projectileType;
    private Vector3 firePositionWeapon;
    private Vector3 firePositionSpell;
    private float LifeTime;
    private Weapons AttackWeapon;
    private WeaponBrain weaponBrain;
    private SingleTargetedProjectile spell;
    private int enemyLayer = 1 << 12;

    private void OnEnable()
    {
        LifeTime = 0f;
        firePositionSpell = SpellManager.Instance.FirePoint.position;
        Weapons.OnPlayerAttack += Weapons_OnAttack;
        SingleTargetedProjectile.OnProjectileSpellCast += SingleTargetedProjectile_OnProjectileSpellCast;
    }

    private void OnDisable()
    {
        Weapons.OnPlayerAttack -= Weapons_OnAttack;
        SingleTargetedProjectile.OnProjectileSpellCast -= SingleTargetedProjectile_OnProjectileSpellCast;
    }

    private void Update()
    {
        if (AttackWeapon != null)
        {
            transform.Translate(Vector3.forward * ProjectileSpeed * Time.deltaTime);
            LifeTime += Time.deltaTime;
            if (LifeTime > MaxLifeTime || transform.position.z >= firePositionWeapon.z + AttackWeapon.ThisWeaponSO.AttackRange)
            {
                ObjectPooler.Instance.ReturnToPool(this);
                AttackWeapon = null;
            }
        }
        else if (spell != null)
        {
            transform.Translate(Vector3.down * ProjectileSpeed * Time.deltaTime);
            LifeTime += Time.deltaTime;
            if (LifeTime > MaxLifeTime)
            {
                ObjectPooler.Instance.ReturnToPool(this);
                spell = null;
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
                ContactPoint contactPoint = collision.GetContact(0);
                if (collision.gameObject.GetComponentInParent<Target>() != null)
                {
                    Target target = collision.gameObject.GetComponentInParent<Target>();
                    
                    if (target.enemyBrain != null && target.GetEnemy() == true && target.IsDead == false)
                    {
                        if (AttackWeapon != null)
                        {
                            if (collision.gameObject.CompareTag("Enemy"))
                            {
                                AttackWeapon.DoAttack(target, target.enemyBrain.GetThisEnemy().ThisEnemySO.DodgeChance);
                                if (!target.Dodging)
                                {

                                }
                            }
                        }
                        else if (spell != null)
                        {
                            if (collision.gameObject.CompareTag("Enemy"))
                            {
                                Explode(contactPoint.point);
                            }
                        }
                        
                    }
                }               
                if (weaponBrain != null)
                {
                    weaponBrain.SpawnHitVfx(contactPoint.point + new Vector3(0, 0, -5f));
                }
                if (spell != null)
                {
                    spell.SpawnHitVfx(contactPoint.point);
                }               
           }
    }

    private void Explode(Vector3 pos)
    {
        float radius = spell.ThisSpellSO.Range;
        Collider[] cols = Physics.OverlapSphere(pos, radius, enemyLayer);
        foreach(var col in cols)
        {
            if (col.CompareTag("Enemy"))
            {
                Rigidbody rb = col.GetComponentInParent<Rigidbody>();
                Target target = col.GetComponentInParent<Target>();
                if (rb != null && target != null)
                {
                    rb.isKinematic = false;
                    rb.AddExplosionForce(10f, pos, radius, 1f, ForceMode.Impulse);
                    target.DoDamage(spell.ThisSpellSO.EffectAmount, 0);
                    rb.isKinematic = true;
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
            weaponBrain = e.weaponBrain;
            firePositionWeapon = e.weaponBrain.transform.position;
        }
    }

    private void SingleTargetedProjectile_OnProjectileSpellCast(object sender, OnSTSpellCastEventArg e)
    {
        if (e != null && e.spellCategory == SpellCategories.SingleTargetedProjectile)
        {
            spell = e.spell;
        }
    }

}






    