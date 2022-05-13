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
        enemySO = GameController.GetEnemySOFromList(enemyType);
        this.enemyBrain = EB;
        this.transform = EB.transform;
        this.rigidbody = EB.GetComponent<Rigidbody>();
        this.animator = EB.GetComponentInChildren<Animator>();
        this.target = EB.GetComponent<Target>();        
    }

    public virtual void DoAttack(Target enemyTarget, float enemyDodgeChance)
    {
        int damageAmount = enemySO.DamageAmount - Mathf.RoundToInt((enemySO.DamageAmount * enemyBrain.MenuControl.GetArmorBlockPct()) / 100);
        if (UnityEngine.Random.value <= enemySO.CritChance)
        {
            enemyTarget.DoCritDamage(enemySO.CritBonus, damageAmount, enemyDodgeChance);
            OnEnemyDamage?.Invoke(this, new OnEnemyDamageEventArg(true));
        }
        else
        {
            enemyTarget.DoDamage(damageAmount, enemyDodgeChance);
            OnEnemyDamage?.Invoke(this, new OnEnemyDamageEventArg(false));
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


