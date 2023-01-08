using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class State
{
    #region Variables

    protected float stateSpeed;
    protected AIStates state;
    protected GameObject gameObject;
    protected Transform enemyTransform;
    protected Animator animator;
    protected StateMachine stateMachine;
    protected EnemyBrain enemyBrain;
    protected Enemy enemy;
    protected EnemySO enemySO;
    protected EnemyTrigger enemyTrigger;

    #endregion

    #region General

    public State(GameObject gameObject, StateMachine SM, AIStates aiState)
    {
        this.gameObject = gameObject;
        this.state = aiState;       
        this.animator = gameObject.GetComponentInChildren<Animator>();
        this.stateMachine = SM;
        this.enemyBrain = gameObject.GetComponent<EnemyBrain>();
        this.enemyTransform = enemyBrain.EnemyTransform;
        enemyTrigger = enemyBrain.Trigger;
        enemy = enemyBrain.GetThisEnemy();
        enemySO = enemyBrain.ThisEnemySO;                            
    }

    public AIStates GetAIState()
    {
        return state;
    }

    public abstract void Tick();
    public abstract void OnEnter();
    public abstract void OnExit();

    #endregion
}



