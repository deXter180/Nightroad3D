using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ResourceManagement
{
    private int MaxMana;
    private int MaxHealth;
    public int CurrentHealth { get; private set; }
    public int CurrentMana { get; private set; }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Declaring eventhandler for ondamage event ~~~~~~~~~~~~~~~~~~~~~~~~~

    public class DamagedEventArgs : EventArgs
    {
        public int CurrentHP { get; set; }
        public DamagedEventArgs(int currentHP)
        {
            CurrentHP = currentHP;
        }
    }
    public class ManaEventArgs : EventArgs
    {
        public int CurrentMana { get; set; }
        public ManaEventArgs(int currentMana)
        {
            CurrentMana = currentMana;
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~ Declaring Events ~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public event EventHandler<DamagedEventArgs> OnDamaged;
    public event EventHandler<DamagedEventArgs> OnHealthGain;
    public event EventHandler OnKilled;
    public event EventHandler<ManaEventArgs> OnManaExpense;
    public event EventHandler<ManaEventArgs> OnManaGain;

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Checking for Death ~~~~~~~~~~~~~~~~~~~~~~~

    public bool IsDead { get { return CurrentHealth < 1; } }

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Setting Max Resources ~~~~~~~~~~~~~~~~~~~~~~~

    public void SetHealth(int MaxHP)
    {
        this.MaxHealth = MaxHP;
        CurrentHealth = MaxHP;
    }
    public void SetMana(int MaxMana)
    {
        this.MaxMana = MaxMana;
        CurrentMana = MaxMana;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Raising Events ~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public void Damage(int pAmount)
    {
        if (pAmount > 0)
        {
            int totalDamageTaken = pAmount < CurrentHealth ? pAmount : CurrentHealth;
            CurrentHealth -= totalDamageTaken;
            OnDamaged?.Invoke(this, new DamagedEventArgs(CurrentHealth));
            if (IsDead)
            {
                OnKilled?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void HealthGain(int pAmount)
    {
        if (pAmount + CurrentHealth <= MaxHealth)
        {
            CurrentHealth += pAmount;
            OnHealthGain.Invoke(this, new DamagedEventArgs(CurrentHealth));
        }
    }

    public void ManaExpense(int pAmount) 
    {
        if (pAmount < CurrentMana)
        {
            CurrentMana -= pAmount;
            OnManaExpense?.Invoke(this, new ManaEventArgs(CurrentMana));
        }
    }
    public void EnergyGain(int pAmount)
    {
        if (pAmount + CurrentMana <= MaxMana)
        {
            CurrentMana += pAmount;
            OnManaGain?.Invoke(this, new ManaEventArgs(CurrentMana));
        }
    }

    

   
}

