using System;
using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    private bool dodging;
    private int currentHP;
    private int currentMana;
    private int maxMana;
    private EnemyBrain EB;
    [SerializeField] private bool IsEnemy;
    [HideInInspector] public int MaxHP { get; set; }
    [HideInInspector] public bool IsDead { get; set; }
    public ResourceManagement Resource = new ResourceManagement();
    public int CurrentHP => currentHP;
    public EnemyBrain enemyBrain => EB;
    public bool Dodging => dodging;
    public bool Blocking { get; set; }
    public event Action OnDodge;
    public static event Action<EnemyBrain> OnEnemyDead;
    public event Action OnCritShot;
    

    //~~~~~~~~~~~~~~~~~~~~~~~~ Initialization ~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void OnEnable()
    {
        if (Resource != null)                              // HealthSystem Event subscription
        {
            Resource.OnHealthLoss += OnDamage;
            Resource.OnKilled += OnKilled;
        }
        dodging = false;
        Blocking = false;
    }

    private void OnDisable()
    {
        if (Resource != null)                              // HealthSystem Event desubscription
        {
            Resource.OnHealthLoss -= OnDamage;
            Resource.OnKilled -= OnKilled;
        }
    }

    public void SetupMaxHP(int maxHP)
    {
        MaxHP = maxHP;
        Resource.SetHealth(maxHP);
        currentHP = maxHP;
    }

    public void SetupMaxMana(int maxMana)
    {
        this.maxMana = maxMana;
        Resource.SetMana(maxMana);
        currentMana = maxMana;
    }

    public void SetEB(EnemyBrain enemyBrain)
    {
        EB = enemyBrain;
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
  
    //~~~~~~~~~~~~~~~~~~~~~~~~ Damage function ~~~~~~~~~~~~~~~~~~~~~~~~~~~~

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
                Resource.Damage(dmg);
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
                Resource.Damage(dmg);
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
        Resource.ManaLoss(manaCost);
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~~~

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
            if (Resource.IsDead == true)
            {
                IsDead = Resource.IsDead;
                currentHP = 0;
                if (IsEnemy)
                {
                    OnEnemyDead?.Invoke(EB);
                }
                Debug.Log("Dead !!");               
            }
        }
    }
}