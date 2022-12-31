using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine
{
    #region Variables

    private State currentState;
    private State previousState;
    private EnemyBrain enemyBrain;
    private Dictionary<AIStates, State> StateDict;
    private bool IsInitialized => StateDict != null;
    public event Action<State, State> OnStateChange;

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
        StateDict.Add(AIStates.Roam, new Roam(enemyBrain, this, AIStates.Roam));
        StateDict.Add(AIStates.Chase, new Chase(enemyBrain, this, AIStates.Chase));
        StateDict.Add(AIStates.Prepare, new Prepare(enemyBrain, this, AIStates.Prepare));
        StateDict.Add(AIStates.Attack, new Attack(enemyBrain, this, AIStates.Attack));
        StateDict.Add(AIStates.Stop, new Stop(enemyBrain, this, AIStates.Stop));
    }

    public void SetState(AIStates states)
    {
        InitializeDict();
        if (StateDict.TryGetValue(states, out State state))
        {
            if (state == currentState)
                return;
            
            currentState?.OnExit();
            if (currentState != null)
            {
                previousState = currentState;
            }
            else
            {
                previousState = state;
            }
            currentState = state;
            OnStateChange?.Invoke(currentState, previousState);
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

    public State GetCurrentState()
    {
        return currentState;
    }

    public State GetPreviousState()
    {
        return previousState;
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
