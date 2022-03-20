using System;
using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    private bool dodging;
    private EnemyBrain EB;
    [SerializeField] private bool IsEnemy;
    [HideInInspector] public int MaxHP { get; set; }
    [HideInInspector] public bool IsDead { get; set; }
    public ResourceManagement Resource = new ResourceManagement();
    public EnemyBrain enemyBrain { get => EB; }
    public bool Dodging { get => dodging; }
    public bool Blocking { get; set; }
    public event Action OnDodge;
    public static event Action<EnemyBrain> OnEnemyDead;
    

    //~~~~~~~~~~~~~~~~~~~~~~~~ Initialization ~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void OnEnable()
    {
        if (Resource != null)                              // HealthSystem Event subscription
        {
            Resource.OnDamaged += OnDamage;
            Resource.OnKilled += OnKilled;
        }
        dodging = false;
        Blocking = false;
    }

    private void OnDisable()
    {
        if (Resource != null)                              // HealthSystem Event desubscription
        {
            Resource.OnDamaged -= OnDamage;
            Resource.OnKilled -= OnKilled;
        }
    }

    public void SetupMaxHP(int maxHP)
    {
        Resource.SetHealth(maxHP);
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

    //~~~~~~~~~~~~~~~~~~~~~~~~~~ Events Callback ~~~~~~~~~~~~~~~~~~~~~~~~

    private void OnDamage(object sender, ResourceManagement.DamagedEventArgs e)
    {
        if (e != null)
        {
            Debug.Log(e.CurrentHP);
        }
    }
    private void OnKilled(object sender, EventArgs e)
    {
        if (e != null)
        {
            if (Resource.IsDead == true)
            {
                IsDead = Resource.IsDead;
                if (IsEnemy)
                {
                    OnEnemyDead?.Invoke(EB);
                }
                Debug.Log("Dead !!");               
            }
        }
    }
}