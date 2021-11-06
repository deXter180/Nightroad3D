using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ResourceManagement
{
    public int CurrentHealth { get; private set; }
    public int CurrentEnergy { get; private set; }
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
    public class EnergyEventArgs : EventArgs
    {
        public int CurrentEnergy { get; set; }
        public EnergyEventArgs(int currentEnergy)
        {
            CurrentEnergy = currentEnergy;
        }
    }
    public class ManaEventArgs
    {
        public int CurrentMana { get; set; }
        public ManaEventArgs(int currentMana)
        {
            CurrentMana = currentMana;
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~ Declaring Events ~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public event EventHandler<DamagedEventArgs> OnDamaged;
    public event EventHandler OnKilled;
    public event EventHandler<EnergyEventArgs> OnEnergyExpense;
    public event EventHandler<ManaEventArgs> OnManaExpense;

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Checking for Death ~~~~~~~~~~~~~~~~~~~~~~~

    public bool IsDead { get { return CurrentHealth < 1; } }

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Setting Max Resources ~~~~~~~~~~~~~~~~~~~~~~~

    public void SetHealth(int MaxHP)
    {
        CurrentHealth = MaxHP;
    }
    public void SetEnregy(int MaxEnergy)
    {
        CurrentEnergy = MaxEnergy;
    }
    public void SetMana(int MaxMana)
    {
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
    public void EnergyExpense(int pAmount) 
    {
        if (pAmount < CurrentEnergy)
        {
            CurrentEnergy -= pAmount;
            OnEnergyExpense?.Invoke(this, new EnergyEventArgs(CurrentEnergy));
        }
    }
    public void EnergyGain(int pAmount)
    {

    }

    

   
}

