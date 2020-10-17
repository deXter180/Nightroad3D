using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [HideInInspector] public int CurrentHealth;
    [HideInInspector] public int MaxHealth;
    [HideInInspector] public bool UnderAttack;

    public class DamagedEventArgs : EventArgs
    {
        public int Damage;
        public DamagedEventArgs(int pDamage)
        {
            Damage = pDamage;
        }
    }

    public event EventHandler<DamagedEventArgs> OnDamaged;
    public event EventHandler OnKilled;

    public bool IsDead { get { return CurrentHealth < 1; } }

    
    public void Damage (int pAmount)
    {
        if (UnderAttack == true)
        {
            
            if (pAmount > 0)
            {
                int totalDamageTaken = pAmount < CurrentHealth ? pAmount : CurrentHealth;
                CurrentHealth -= totalDamageTaken;
                OnDamaged?.Invoke(this, new DamagedEventArgs (pAmount));
                if (IsDead)
                {
                    OnKilled?.Invoke(this, EventArgs.Empty);
                }
                
            }
        
        }
    }
}
