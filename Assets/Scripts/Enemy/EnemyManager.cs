using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Enemy : IEnemyHP, IAttack, IMove
{
    protected Transform transform;
    protected Animator animator;
    protected Target target;
    protected EnemyBrain enemyBrain;

    public abstract EnemyTypes enemyTypes { get; }
    public abstract int MaxHP { get ; }
    public abstract int DamageAmount { get ; }
    public abstract float AttackSpeed { get ; }
    public abstract int MoveSpeed { get ; }
    public abstract int AttackRange { get; }

    public abstract float CritChance { get; }

    public abstract float CritBonus { get; }

    public abstract float DodgeChance { get; }

    public static Dictionary<EnemyTypes, Enemy> EnemyDictionary = new Dictionary<EnemyTypes, Enemy>();

    public Enemy(EnemyBrain EB)
    {
        this.enemyBrain = EB;
        this.transform = EB.transform;
        this.animator = EB.GetComponentInChildren<Animator>();
        this.target = EB.GetComponent<Target>();
    }

    public abstract void Attack(Target target , float dodgeChance);
}

public enum EnemyTypes
{
    Giant = 0,
    Fighter = 1
}
