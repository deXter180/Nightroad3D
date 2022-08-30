using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine
{
    #region Variables

    public State currentState;
    private EnemyBrain enemyBrain;
    private Dictionary<AIStates, State> StateDict;
    private bool IsInitialized => StateDict != null;
    public event Action<State> OnStateChange;

    #endregion

    #region General

    public StateMachine(EnemyBrain enemyBrain)
    {
        this.enemyBrain = enemyBrain;
    }

    private void InitializeDict()
    {
        if (IsInitialized)
            return;
        StateDict = new Dictionary<AIStates, State>();
        StateDict.Add(AIStates.Roam, new Roam(enemyBrain, this));
        StateDict.Add(AIStates.Chase, new Chase(enemyBrain, this));
        StateDict.Add(AIStates.Prepare, new Prepare(enemyBrain, this));
        StateDict.Add(AIStates.Attack, new Attack(enemyBrain, this));
        StateDict.Add(AIStates.Stop, new Stop(enemyBrain, this));
    }
    public void SetState(AIStates states)
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
    public State GetState(AIStates states)
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
            SetState(AIStates.Roam);
        }
        currentState.Tick();
    }

    #endregion
}
