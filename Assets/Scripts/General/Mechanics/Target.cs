using System;
using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private bool IsEnemy;
    [HideInInspector] public int MaxHP { get; set; }
    [HideInInspector] public bool IsDead { get; set; }
    public ResourceManagement Resource = new ResourceManagement();
    private bool Dodging;
    public bool Blocking { get; set; }
    

    //~~~~~~~~~~~~~~~~~~~~~~~~ Initialization ~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void OnEnable()
    {
        if (Resource != null)                              // HealthSystem Event subscription
        {
            Resource.OnDamaged += OnDamage;
            Resource.OnKilled += OnKilled;
        }
        Dodging = false;
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

    public EnemyBrain GetEBFromTarget()
    {
        if (TryGetComponent(out EnemyBrain enemyBrain))
        {
            return enemyBrain;
        }
        else return null;
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
            if (Dodging == false)
            {
                Resource.Damage(dmg);
            }
            else
            {
                Debug.Log("Dodged");
            }
        }
        
    }
    public void DoCritDamage(float critBonus, int dmg, float dodgeChance)
    {
        if (Blocking == false)
        {
            IsDodging(dodgeChance);
            if (Dodging == false)
            {
                dmg += (int)(dmg * critBonus);
                Resource.Damage(dmg);
            }
            else
            {
                Debug.Log("Dodged");
            }
        }
        
    }
    //public void DoPerTurnDamage(float tickTime, int dmg)
    //{
    //    int startTick = 0;
        
    //}

    //~~~~~~~~~~~~~~~~~~~~~~~~~~ Dodge function ~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public bool GetDodging()
    {
        return Dodging;
    }
    public void SetDodging(bool dodging)
    {
        Dodging = dodging;
    }
    private void IsDodging(float dodgeChance)
    {
        if (UnityEngine.Random.value <= dodgeChance)
        {
            Dodging = true;
        }
        else Dodging = false;
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
                Debug.Log("Dead !!");
                gameObject.SetActive(false);
                if (IsEnemy)
                {
                    AssetCollections.ReleaseAssetInstance(gameObject, "Enemy");
                }
            }
        }
    }
}