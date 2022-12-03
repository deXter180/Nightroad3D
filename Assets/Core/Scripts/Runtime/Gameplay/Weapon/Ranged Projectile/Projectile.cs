using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile :  MonoBehaviour
{
    #region SerializedVariables

    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private float MaxLifeTime;
    [SerializeField] private ProjectileTypes projectileType;

    #endregion

    #region Variables

    private string bloodVfx = "Blood_burst_vfx";
    private Vector3 firePositionWeapon;
    private float bloodOffset = 5;
    private float LifeTime;
    private PlayerController player;
    private GameController gameController;
    private Weapons attackingWeapon;
    private WeaponBrain weaponBrain;
    private EnemyBrain attackingEnemy;
    private AttributeManager attributeManager;
    private SingleTargetedProjectile spell;
    private int enemyLayer = 1 << 12;
    private Rigidbody rb;
    private ObjectPooler objectPooler;

    #endregion

    #region General

    private void Start()
    {
        player = PlayerController.Instance;
        gameController = GameController.Instance;
        objectPooler = ObjectPooler.Instance;
        attributeManager = AttributeManager.Instance;
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        LifeTime = 0f;
        Weapons.OnPlayerAttack += Weapons_OnAttack;
        SingleTargetedProjectile.OnProjectileSpellCast += SingleTargetedProjectile_OnProjectileSpellCast;
    }

    private void OnDisable()
    {
        Weapons.OnPlayerAttack -= Weapons_OnAttack;
        SingleTargetedProjectile.OnProjectileSpellCast -= SingleTargetedProjectile_OnProjectileSpellCast;
    }

    private void FixedUpdate()
    {
        if (attackingWeapon != null)
        {
            transform.Translate(Vector3.forward * ProjectileSpeed * Time.fixedDeltaTime);
            LifeTime += Time.fixedDeltaTime;
            if (LifeTime > MaxLifeTime || transform.position.z >= firePositionWeapon.z + attackingWeapon.ThisWeaponSO.AttackRange)
            {
                objectPooler.ReturnToPool(this);
                attackingWeapon = null;
            }
        }
        else if (spell != null)
        {
            transform.Translate(Vector3.down * ProjectileSpeed * Time.fixedDeltaTime);
            LifeTime += Time.fixedDeltaTime;
            if (LifeTime > MaxLifeTime)
            {
                objectPooler.ReturnToPool(this);
                spell = null;
            }            
        }
        else if (attackingEnemy != null)
        {
            transform.Translate(GetDirToPlayer() * ProjectileSpeed * Time.fixedDeltaTime);
            LifeTime += Time.fixedDeltaTime;
            if (LifeTime > MaxLifeTime)
            {
                objectPooler.ReturnToPool(this);
                attackingEnemy = null;
            }
        }
    }

    public void SetEnemy(EnemyBrain enemyBrain)
    {
        attackingEnemy = enemyBrain;
    }

    public ProjectileTypes GetProjectileType()
    {
        return projectileType;
    }

    private Vector3 GetDirToPlayer()
    {
        return (player.PlayerTransform.position - attackingEnemy.EnemyTransform.position).normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {       
        if (collision != null)
        {
            objectPooler.ReturnToPool(this);
            ContactPoint contactPoint = collision.GetContact(0);
            if (collision.gameObject.GetComponentInParent<Target>() != null)
            {
                Target target = collision.gameObject.GetComponentInParent<Target>();
                if (target.IsDead == false)
                {    
                    if (projectileType == ProjectileTypes.EnemyFireBall)
                    {
                        if (collision.gameObject.CompareTag("Player") && attackingEnemy != null)
                        {
                            if (collision.collider.material.name == "")
                            {
                                attackingEnemy.GetThisEnemy().DoAttack(target, player.DodgeChace);
                                attackingEnemy = null;
                            }
                        }
                    }
                    else
                    {
                        if (collision.gameObject.CompareTag("Enemy"))
                        {
                            if (target.enemyCore != null && target.GetEnemy() == true)
                            {
                                if (attackingWeapon != null)
                                {
                                    AssetLoader.CreateAndReleaseAsset(bloodVfx, contactPoint.point + contactPoint.normal * bloodOffset, 1);
                                    attackingWeapon.DoAttack(target, target.enemyCore.EnemyDodgeChance);
                                    if (!target.Dodging)
                                    {

                                    }
                                }
                                else if (spell != null)
                                {
                                    Explode(contactPoint.point);
                                }
                            }
                        }
                    }                                                                                
                }                    
                    
            }  
            else if (collision.gameObject.GetComponent<NPCBrain>() != null)
            {
                NPCBrain npc = collision.gameObject.GetComponent<NPCBrain>();
                StartCoroutine(gameController.HighlightNPCSpeech(npc.SpeechBubblePos, npc.GetDialogueText()));
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
                    int modifiedValue = Mathf.FloorToInt(spell.GetModifiedStats());
                    target.DoDamage(modifiedValue, 0);
                    rb.isKinematic = true;
                }
            }          
        }
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~ Event Callback ~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void Weapons_OnAttack(object sender, OnPlayerAttackEventArg e)
    {
        if (e != null && e.weaponCategory == WeaponCategories.ProjectileShoot)
        {          
            attackingWeapon = e.weapon;
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

    #endregion
}