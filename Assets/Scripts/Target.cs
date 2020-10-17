using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    HealthSystem Health;
    MeleeCollision MCol;
    public int MaxHP;
    private bool IsTakingDamage;
    

    private void OnEnable()
    {
        Health = FindObjectOfType<HealthSystem>();
        MCol = FindObjectOfType<MeleeCollision>();
        if (Health != null)
        {
            Health.MaxHealth = MaxHP;
            Health.CurrentHealth = Health.MaxHealth;
            Health.OnDamaged += OnDamage;
            Health.OnKilled += OnKilled;
        }
        if (MCol != null)
        {
            MCol.OnInflictingDamage += WC_OnInflictingDamage;
        }
    }

    private void OnDisable()
    {
        if (Health != null)
        {
            Health.OnDamaged -= OnDamage;
            Health.OnKilled -= OnKilled;
        }
        if (MCol != null)
        {
            MCol.OnInflictingDamage -= WC_OnInflictingDamage;
        }
    }

    private void WC_OnInflictingDamage(object sender, MeleeCollision.DamageEventArgs e)
    {
        if (e != null)
        {
             IsTakingDamage = e.IsAttacking;
             Health.UnderAttack = IsTakingDamage;
             Health.Damage(e.DoDamage);
        }
    }

    private void OnDamage(object sender, HealthSystem.DamagedEventArgs e)
    {
        if (IsTakingDamage == true)
        {
            if (e != null)
            {
                Debug.Log(e.Damage);
            }
            
            Debug.Log(Health.CurrentHealth);
        }
        IsTakingDamage = false;
    }

    private void OnKilled(object sender, EventArgs e)
    {
        if (e != null)
        {
            if (Health.IsDead == true)
            {
                Debug.Log("Dead !!");
            }
        }
    }
}

