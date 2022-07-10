using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinationGoal : Goal
{
    public string EnemyID { get; private set; }
    public string EnemyName { get; private set; }
    public EnemyTypes EnemyType { get; private set; }

    public AssassinationGoal(string enemyID, string enemyName, EnemyTypes enemyType)
    {
        EnemyID = enemyID;
        EnemyType = enemyType;
        RequiredAmount = 1;
        CurrentAmount = 0;
        IsCompleted = false;
        EnemyName = enemyName;
    }

    public override void Initialize()
    {
        base.Initialize();
        Target.OnEnemyDead += Target_OnEnemyDead;
    }

    public override void Closer()
    {
        base.Closer();
        Target.OnEnemyDead -= Target_OnEnemyDead;
    }

    private void Target_OnEnemyDead(EnemyBrain EB)
    {
        if (EB.GetEnemyType() == EnemyType)
        {
            if (EB.EnemyName == EnemyName && EB.EnemyID == EnemyID)
            {
                CurrentAmount++;
                Evaluate();
            }
        }
    }
}
