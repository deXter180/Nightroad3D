using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Enemy
{
    private Transform transform;
    private Animator animator;
    private Target target;
    private EnemyBrain enemyBrain;
    private Rigidbody rigidbody;
    private EnemySO enemySO;
    public EnemySO ThisEnemySO { get => enemySO; }

    public event EventHandler<OnEnemyAttackEventArg> OnEnemyAttack;
    public event EventHandler<OnEnemyDamageEventArg> OnEnemyDamage;

    public void RaiseOnEnemyAttack(EnemyTypes enemyType)
    {
        InvokeOnAttack(new OnEnemyAttackEventArg(enemyType));
    }
    
    private void InvokeOnAttack(OnEnemyAttackEventArg eventArgs)
    {
        var handler = OnEnemyAttack;
        if (handler != null)
        {
            handler(this, eventArgs);
        }
    }

    public Enemy(EnemyBrain EB, EnemyTypes enemyType)
    {
        enemySO = AssetCollections.GetEnemySOFromList(enemyType);
        this.enemyBrain = EB;
        this.transform = EB.transform;
        this.rigidbody = EB.GetComponent<Rigidbody>();
        this.animator = EB.GetComponentInChildren<Animator>();
        this.target = EB.GetComponent<Target>();        
    }

    public virtual void DoAttack(Target enemyTarget, float enemyDodgeChance)
    {
        if (UnityEngine.Random.value <= enemySO.CritChance) //&& CurrentEnergy >= EnergyCosts[0])
        {
            enemyTarget.DoCritDamage(enemySO.CritBonus, enemySO.DamageAmount, enemyDodgeChance);
            OnEnemyDamage?.Invoke(this, new OnEnemyDamageEventArg(true));
            //target.Resource.EnergyExpense(EnergyCosts[0]);
        }
        else
        {
            enemyTarget.DoDamage(enemySO.DamageAmount, enemyDodgeChance);
            OnEnemyDamage?.Invoke(this, new OnEnemyDamageEventArg(false));
            //target.Resource.EnergyExpense(EnergyCosts[1]);
        }
    }
}

public class OnEnemyDamageEventArg : EventArgs
{
    public bool IsCrit;

    public OnEnemyDamageEventArg(bool crit)
    {
        IsCrit = crit;
    }
}

public class OnEnemyAttackEventArg : EventArgs
{
    public EnemyTypes enemyType;

    public OnEnemyAttackEventArg(EnemyTypes ET)
    {
        enemyType = ET;
    }
}


