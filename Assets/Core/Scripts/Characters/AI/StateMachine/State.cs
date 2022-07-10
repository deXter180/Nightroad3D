using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class State
{
    protected GameObject gameObject;
    protected Transform enemyTransform;
    protected Animator animator;
    protected StateMachine stateMachine;
    protected Target target;
    protected EnemyBrain enemyBrain;
    protected Enemy enemy;
    protected EnemySO enemySO;
    protected EnemyTrigger enemyTrigger;

    public State(GameObject gameObject, StateMachine SM)
    {
        this.gameObject = gameObject;
        this.enemyTransform = gameObject.transform;
        this.animator = gameObject.GetComponentInChildren<Animator>();
        this.stateMachine = SM;
        this.target = gameObject.GetComponent<Target>();
        this.enemyBrain = gameObject.GetComponent<EnemyBrain>();
        enemyTrigger = enemyBrain.Trigger;        
        enemy = enemyBrain.GetThisEnemy();
        enemySO = enemyBrain.ThisEnemySO;
    }

    public abstract void Tick();
    public abstract void OnEnter();
    public abstract void OnExit();
    
}



