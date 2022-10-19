using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Goal
{
    #region Properties

    public GoalTypes GoalType{ get; set; }
    public string Description { get; set; }
    public string GoalCompleteText { get; set; }
    public string GoalFailureText { get; set; }
    public bool IsCompleted { get; set; }
    public int RequiredAmount { get; set; }
    public int CurrentAmount { get; set; }

    #endregion

    #region Events

    public event Action<string, Goal> OnGoalCompletion;
    public event Action<string, Goal> OnGoalFailure;

    #endregion

    #region General

    public virtual void Initialize()
    {
        
    }

    public virtual void Closer()
    {
        
    }

    public virtual void Evaluate()
    {
        if (CurrentAmount >= RequiredAmount)
        {
            Complete();
        }
        else
        {
            IsCompleted = false;
            OnGoalFailure?.Invoke(GoalFailureText, this);
        }      
    }

    private void Complete()
    {
        IsCompleted = true;
        OnGoalCompletion?.Invoke(GoalCompleteText, this);
    }

    #endregion
}
