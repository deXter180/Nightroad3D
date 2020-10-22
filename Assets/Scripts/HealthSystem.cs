using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public sealed class HealthSystem
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Singleton pattern implementation ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    #region Singleton
    private HealthSystem()
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~ List Initialization ~~~~~~~~~~~~~~~~~~~~~~~~

        this.subscribedDmgEvents = new List<EventHandler<DamagedEventArgs>>(); 
        this.subscribedKillEvents = new List<EventHandler>();
    }
    private static readonly Lazy<HealthSystem> lazy = new Lazy<HealthSystem>(() => new HealthSystem());
    public static HealthSystem Instance
    {
        get
        {
            return lazy.Value;
        }

    }
    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Variable for internal Health Calculation ~~~~~~~~~~~~~~~~~~~~~~~~~~~

    [HideInInspector] public int CurrentHealth { get; set; }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Declaring eventhandler for ondamage event ~~~~~~~~~~~~~~~~~~~~~~~~~

    public class DamagedEventArgs : EventArgs
    {
        public int Damage { get; set; }
        public DamagedEventArgs(int pDamage)
        {
            Damage = pDamage;
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~ Declaring Events ~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private List<EventHandler<DamagedEventArgs>> subscribedDmgEvents;     // List for OnDamage event subscribers
    private List<EventHandler> subscribedKillEvents;                      // List for OnKill event subscribers
    private event EventHandler<DamagedEventArgs> _OnDamaged;                  
    private event EventHandler _OnKilled;

    //~~~~~~~~~~~~~~~~~~~~~~~ Adding subscribers to List ~~~~~~~~~~~~~~~~~~~~~~~

    public event EventHandler<DamagedEventArgs> OnDamaged                 
    {
        add
        {
            if (!this.subscribedDmgEvents.Contains(value))
            {
                this._OnDamaged += value;
                this.subscribedDmgEvents.Add(value);
            }
        }
        remove
        {
            if (this.subscribedDmgEvents.Contains(value))
            {
                this._OnDamaged -= value;
                this.subscribedDmgEvents.Remove(value);
            }
        }
    }
    public event EventHandler OnKilled
    {
        add
        {
            if (!this.subscribedKillEvents.Contains(value))
            {
                this._OnKilled += value;
                this.subscribedKillEvents.Add(value);
            }
        }
        remove
        {
            if (this.subscribedKillEvents.Contains(value))
            {
                this._OnKilled -= value;
                this.subscribedKillEvents.Remove(value);
            }
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Checking for Death ~~~~~~~~~~~~~~~~~~~~~~~

    public bool IsDead { get { return CurrentHealth < 1; } }

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Setting Max Health ~~~~~~~~~~~~~~~~~~~~~~~

    public void SetHealth(int MaxHP)
    {
        CurrentHealth = MaxHP;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Raising Events ~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public void Damage(int pAmount)
    {
        
        if (pAmount > 0)
        {
                int totalDamageTaken = pAmount < CurrentHealth ? pAmount : CurrentHealth;
                CurrentHealth -= totalDamageTaken;
                this._OnDamaged?.Invoke(this, new DamagedEventArgs(pAmount));
                if (IsDead)
                {
                    _OnKilled?.Invoke(this, EventArgs.Empty);
                }
                
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~ Events Subscription Methods ~~~~~~~~~~~~~~~~~~~~~~

    public void RegisterDamageEvent(EventHandler<DamagedEventArgs> callBackFunc)
    {
        Instance.OnDamaged += callBackFunc;
    }

    public void UnRegisterDamageEvent(EventHandler<DamagedEventArgs> callBackFunc)
    {
        Instance.OnDamaged -= callBackFunc;
    }

    public void RegisterKillEvent(EventHandler callBackFunc)
    {
        Instance.OnKilled += callBackFunc;
    }

    public void UnRegisterKillEvent(EventHandler callBackFunc)
    {
        Instance.OnKilled -= callBackFunc;
    }
}
