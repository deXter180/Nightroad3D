using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Approach : State
{
    private EnemyBrain enemyBrain;

    public Approach(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
    }

    public override void OnEnter()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override void Tick()
    {
        
    }
}
