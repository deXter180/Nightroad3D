using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinationGoal : Goal
{
    #region General

    public string EnemyID { get; private set; }
    public string EnemyName { get; private set; }
    public EnemyTypes ET { get; private set; }

    public AssassinationGoal(string enemyID, string enemyName, EnemyTypes enemyType)
    {
        EnemyID = enemyID;
        ET = enemyType;
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

    #endregion

    #region Callbacks

    private void Target_OnEnemyDead(EnemyCore EC)
    {
        if (EC.EnemyType == ET)
        {
            if (EC.EnemyName == EnemyName && EC.EnemyID == EnemyID)
            {
                CurrentAmount++;
                Evaluate();
            }
        }
    }

    #endregion
}
