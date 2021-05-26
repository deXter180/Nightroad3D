using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : State
{
    private EnemyBrain enemyBrain;

    public Attack(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
    }

    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public override void Tick()
    {
        throw new System.NotImplementedException();
    }
}
