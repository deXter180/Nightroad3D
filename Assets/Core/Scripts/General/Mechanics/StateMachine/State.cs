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
    protected Rigidbody rigidbody;
    protected PlayerController player;

    public State(GameObject gameObject, StateMachine SM)
    {
        this.gameObject = gameObject;
        this.enemyTransform = gameObject.transform;
        this.animator = gameObject.GetComponentInChildren<Animator>();
        this.stateMachine = SM;
        this.target = gameObject.GetComponent<Target>();
        this.rigidbody = gameObject.GetComponent<Rigidbody>();
        player = PlayerController.Instance;
    }

    public abstract void Tick();
    public abstract void OnEnter();
    public abstract void OnExit();
    
}



