using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Goal
{
    public GoalTypes GoalType{ get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public int RequiredAmount { get; set; }
    public int CurrentAmount { get; set; }

    public event Action OnGoalCompletion;

    public virtual void Initialize()
    {
        
    }

    public virtual void Evaluate()
    {
        if (CurrentAmount >= RequiredAmount)
        {
            Complete();
        }
    }

    private void Complete()
    {
        IsCompleted = true;
        OnGoalCompletion?.Invoke();
    }


}
