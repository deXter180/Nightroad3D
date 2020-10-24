using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

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
    [HideInInspector] public List<Entity> Targets;

    public class Entity
    {
        public Guid Id { get; set; }
        public int CurrentHealth { get; set; }
        public string TargetName { get; set; }
    }


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

    //public bool IsDead { get { return CurrentHealth < 1; } }

    public bool IsDead(Guid targetId)
    {
        return (this.Targets.Where(a => a.Id == targetId).ToList().Count() > 0);
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Setting Max Health ~~~~~~~~~~~~~~~~~~~~~~~

    public void SetHealthSystem(Guid targetId,int MaxHP,string colEntity)
    {
        this.Targets = new List<Entity>();

        this.Targets.Add(
                new Entity {
                   Id = targetId,
                   CurrentHealth = MaxHP,
                   TargetName = colEntity
                }
            );

        //this.CurrentHealth = MaxHP;
        //this.TargetName = colEntity;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~ Raising Events ~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public void Damage(int pAmount,string colEntity)
    {
        // Debug.Log($"Health System script attahced too :- {this.}" +
        //     $" entity that took damage:- {colEntity}");

        foreach (var item in this.Targets)
        {
            Debug.Log($"{item.Id}: {item.TargetName}");
        }

        // if (pAmount > 0 && (string.Compare(this.TargetName, colEntity) == 0))
        // {
        //         int totalDamageTaken = pAmount < this.CurrentHealth ? pAmount : this.CurrentHealth;
        //         this.CurrentHealth -= totalDamageTaken;

        //         this._OnDamaged?.Invoke(this, new DamagedEventArgs(pAmount));
        //         if (IsDead)
        //         {
        //             _OnKilled?.Invoke(this, EventArgs.Empty);
        //         }

        // }
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
