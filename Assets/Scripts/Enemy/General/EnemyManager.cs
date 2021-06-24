using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class Enemy : IAttack
{
    protected Transform transform;
    protected Animator animator;
    protected Target target;
    protected EnemyBrain enemyBrain;
    protected Rigidbody rigidbody;

    public abstract EnemyTypes enemyTypes { get; }
    public abstract int MaxHP { get ; }
    public abstract int DamageAmount { get ; }
    public abstract float AttackSpeed { get ; }
    public abstract float MoveSpeed { get ; }
    public abstract float AttackRange { get; }
    public abstract float CritChance { get; }
    public abstract float CritBonus { get; }
    public abstract float DodgeChance { get; }
    public abstract bool IsGroundUnit { get; }
    public abstract float AvoidanceRadius { get; }

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

    public Enemy(EnemyBrain EB)
    {
        this.enemyBrain = EB;
        this.transform = EB.transform;
        this.rigidbody = EB.GetComponent<Rigidbody>();
        this.animator = EB.GetComponentInChildren<Animator>();
        this.target = EB.GetComponent<Target>();
    }

    public virtual void DoAttack(Target enemyTarget, float enemyDodgeChance)
    {
        if (UnityEngine.Random.value <= CritChance) //&& CurrentEnergy >= EnergyCosts[0])
        {
            enemyTarget.DoCritDamage(CritBonus, DamageAmount, enemyDodgeChance);
            OnEnemyDamage?.Invoke(this, new OnEnemyDamageEventArg(true));
            //target.Resource.EnergyExpense(EnergyCosts[0]);
        }
        else
        {
            enemyTarget.DoDamage(DamageAmount, enemyDodgeChance);
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

public enum EnemyTypes
{
    Giant = 0,
    Fighter = 1
}
