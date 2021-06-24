using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine
{
    public State currentState;
    private EnemyBrain enemyBrain;
    private Dictionary<States, State> StateDict;
    private bool IsInitialized => StateDict != null;
    public event Action<State> OnStateChange;

    public StateMachine(EnemyBrain enemyBrain)
    {
        this.enemyBrain = enemyBrain;
    }

    private void InitializeDict()
    {
        if (IsInitialized)
            return;
        StateDict = new Dictionary<States, State>();
        StateDict.Add(States.Roam, new Roam(enemyBrain, this));
        StateDict.Add(States.Chase, new Chase(enemyBrain, this));
        StateDict.Add(States.Attack, new Attack(enemyBrain, this));
    }
    public void SetState(States states)
    {
        InitializeDict();
        if (StateDict.TryGetValue(states, out State state))
        {
            if (state == currentState)
                return;

            currentState?.OnExit();
            currentState = state;
            OnStateChange?.Invoke(currentState);
            currentState.OnEnter();
        }
    } 
    public State GetState(States states)
    {
        InitializeDict();
        if (StateDict.TryGetValue(states, out State state))
        {
            return state;
        }
        else return null;
    }

    public State GetThisState()
    {
        return currentState;
    }

    public void Tick()
    {
        InitializeDict();
        if (currentState == null)
        {
            SetState(States.Roam);
        }
        currentState.Tick();
    }
}

public enum States
{
    Roam,
    Chase,
    Attack
}
