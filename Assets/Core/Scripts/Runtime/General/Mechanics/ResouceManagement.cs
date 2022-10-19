using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ResourceManagement
{
    #region Variables

    private int MaxMana;
    private int MaxHealth;

    #endregion

    #region Properties

    public int CurrentHealth { get; private set; }
    public int CurrentMana { get; private set; }

    public bool IsDead { get { return CurrentHealth < 1; } }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Declaring eventhandler for ondamage event ~~~~~~~~~~~~~~~~~~~~~~~~~

    #region Events

    public class DamagedEventArgs : EventArgs
    {
        public int ChangeAmount { get; set; } 
        public int CurrentHP { get; set; }
        public float currentHealthPct { get; set; }
        public DamagedEventArgs(int changeAmount, int currentHP, float healthPct)
        {
            ChangeAmount = changeAmount;
            CurrentHP = currentHP;
            currentHealthPct = healthPct;
        }
    }
    public class ManaEventArgs : EventArgs
    {
        public int ChangeAmount { get; set; }
        public int CurrentMana { get; set; }
        public float currentManaPct { get; set; }
        public ManaEventArgs(int changeAmount, int currentMana, float manaPct)
        {
            ChangeAmount = changeAmount;
            CurrentMana = currentMana;
            currentManaPct = manaPct;
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~ Declaring Events ~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public event EventHandler<DamagedEventArgs> OnHealthLoss;
    public event EventHandler<DamagedEventArgs> OnHealthGain;
    public event EventHandler OnKilled;
    public event EventHandler<ManaEventArgs> OnManaLoss;
    public event EventHandler<ManaEventArgs> OnManaGain;

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Setting Max Resources ~~~~~~~~~~~~~~~~~~~~~~~

    #region General

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

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Raising Events ~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    #region Mechanics

    public void Damage(int pAmount)
    {
        if (pAmount > 0)
        {
            int totalDamageTaken = pAmount < CurrentHealth ? pAmount : CurrentHealth;
            CurrentHealth -= totalDamageTaken;
            float currentHealthPct = (float)CurrentHealth / (float)MaxHealth;
            OnHealthLoss?.Invoke(this, new DamagedEventArgs(pAmount, CurrentHealth, currentHealthPct));
            if (IsDead)
            {
                OnKilled?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void ManaLoss(int pAmount)
    {
        if (pAmount > 0)
        {
            int totalManaLoss = pAmount < CurrentMana ? pAmount : CurrentMana;
            CurrentMana -= totalManaLoss;
            float currentManaPct = (float)CurrentMana / (float)MaxMana;
            OnManaLoss?.Invoke(this, new ManaEventArgs(pAmount, CurrentMana, currentManaPct));
        }
    }

    public bool HealthGain(int pAmount)
    {
        if (CurrentHealth < MaxHealth)
        {
            int hpLeft = MaxHealth - CurrentHealth;
            CurrentHealth += pAmount <= hpLeft ? pAmount : hpLeft;
            float currentHealthPct = (float)CurrentHealth / (float)MaxHealth;
            OnHealthGain.Invoke(this, new DamagedEventArgs(pAmount, CurrentHealth, currentHealthPct));
            return true;
        }
        return false;
    }

    public bool ManaGain(int pAmount) 
    {
        if (CurrentMana < MaxMana)
        {
            int manaLeft = MaxMana - CurrentMana;
            CurrentMana += pAmount <= manaLeft ? pAmount : manaLeft;
            float currentManaPct = (float)CurrentMana / (float)MaxMana;
            OnManaGain?.Invoke(this, new ManaEventArgs(pAmount, CurrentMana, currentManaPct));
            return true;
        }
        return false;
    }

    #endregion

}

