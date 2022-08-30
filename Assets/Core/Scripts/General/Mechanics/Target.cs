using System;
using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    #region Variables

    private bool dodging;
    private int currentHP;
    private int currentMana;
    private int maxMana;
    private EnemyCore EC;
    private ResourceManagement resource = new ResourceManagement();
    [SerializeField] private bool IsEnemy;

    #endregion

    #region Properties

    [HideInInspector] public int MaxHP { get; set; }
    [HideInInspector] public bool IsDead { get; set; }
    public ResourceManagement Resource => resource;
    public int CurrentHP => currentHP;
    public EnemyCore enemyCore => EC;
    public bool Dodging => dodging;
    public bool Blocking { get; set; }

    #endregion

    #region Events

    public event Action OnDodge;
    public static event Action<EnemyCore> OnEnemyDead;
    public event Action OnCritShot;

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~ Initialization ~~~~~~~~~~~~~~~~~~~~~~~~~~~

    #region General

    private void OnEnable()
    {
        if (resource != null)                              // HealthSystem Event subscription
        {
            resource.OnHealthLoss += OnDamage;
            resource.OnKilled += OnKilled;
        }
        dodging = false;
        Blocking = false;
    }

    private void OnDisable()
    {
        if (resource != null)                              // HealthSystem Event desubscription
        {
            resource.OnHealthLoss -= OnDamage;
            resource.OnKilled -= OnKilled;
        }
    }

    public void SetupMaxHP(int maxHP)
    {
        MaxHP = maxHP;
        resource.SetHealth(maxHP);
        currentHP = maxHP;
    }

    public void SetupMaxMana(int maxMana)
    {
        this.maxMana = maxMana;
        resource.SetMana(maxMana);
        currentMana = maxMana;
    }

    public void SetEB(EnemyCore enemyCore)
    {
        EC = enemyCore;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~ Get Position & Verify for Enemy ~~~~~~~~~~~~~~~~~~~~~

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public bool GetEnemy()
    {
        return IsEnemy;
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~ Damage function ~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    #region Mechanics

    public void DoDamage(int dmg, float dodgeChance)
    {
        if (Blocking == false)
        {
            IsDodging(dodgeChance);
            if (dodging == false)
            {
                if (dmg >= currentHP)
                {
                    OnCritShot?.Invoke();
                }
                resource.Damage(dmg);
            }
            else
            {
                OnDodge?.Invoke();
                Debug.Log("Dodged");
            }
        }
        
    }
    public void DoCritDamage(float critBonus, int dmg, float dodgeChance)
    {
        if (Blocking == false)
        {
            IsDodging(dodgeChance);
            if (dodging == false)
            {
                dmg += (int)(dmg * critBonus);
                OnCritShot?.Invoke();
                resource.Damage(dmg);
            }
            else
            {
                OnDodge?.Invoke();
                Debug.Log("Dodged");
            }
        }
        
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~ Dodge function ~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void IsDodging(float dodgeChance)
    {
        if (UnityEngine.Random.value <= dodgeChance)
        {
            dodging = true;
        }
        else dodging = false;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Mana Functions ~~~~~~~~~~~~~~~~~~~~~~~~

    public void UseMana(int manaCost)
    {
        resource.ManaLoss(manaCost);
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void OnDamage(object sender, ResourceManagement.DamagedEventArgs e)
    {
        if (e != null)
        {
            Debug.Log(e.CurrentHP);
            currentHP = e.CurrentHP;
        }
    }

    private void OnKilled(object sender, EventArgs e)
    {
        if (e != null)
        {
            if (resource.IsDead == true)
            {
                IsDead = resource.IsDead;
                currentHP = 0;
                if (IsEnemy)
                {
                    OnEnemyDead?.Invoke(EC);
                }
                Debug.Log("Dead !!");               
            }
        }
    }

    #endregion
}