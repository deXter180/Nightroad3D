using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Goal
{
    public GoalTypes GoalType{ get; set; }
    public string Description { get; set; }
    public string GoalCompleteText { get; set; }
    public string GoalFailureText { get; set; }
    public bool IsCompleted { get; set; }
    public int RequiredAmount { get; set; }
    public int CurrentAmount { get; set; }

    public event Action<string> OnGoalCompletion;
    public event Action<string> OnGoalFailPostCompletion;

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
            if (IsCompleted)
            {
                IsCompleted = false;
                OnGoalFailPostCompletion?.Invoke(GoalFailureText);
            }
        }      
    }

    private void Complete()
    {
        IsCompleted = true;
        OnGoalCompletion?.Invoke(GoalCompleteText);
    }
}
