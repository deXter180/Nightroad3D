using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Roam : State
{
    #region Variables

    private List<Vector3> path = new List<Vector3>();
    private bool isLoaded = false;

    #endregion

    #region General

    public Roam(EnemyBrain EB, StateMachine SM, AIStates state) : base(EB.gameObject, SM, state)
    {
          
    }

    public override void Tick()
    {
        if (enemyBrain.IsFrozen)
        {
            stateMachine.SetState(AIStates.Stop);
        }
        else
        {
            if (enemyTrigger.IsTargetFleed)
            {
                enemy.Roam();
            }
            else
            {
                stateMachine.SetState(AIStates.Chase);
            }
        }
    }

    public override void OnEnter()
    {
        enemyBrain.navMeshAgent.enabled = true;
        if (enemyBrain.navMeshAgent.isOnNavMesh)
        {
            enemyBrain.navMeshAgent.isStopped = true;
            enemyBrain.navMeshAgent.ResetPath();
            if (!isLoaded)
            {
                enemyBrain.navMeshAgent.speed = enemySO.MoveSpeed;
            }
            for (int i = 0; i < 7; i++)
            {
                path.Add(enemy.GetRandomPosition(50f, 80f));
            }
            enemy.SetupRoam(path);
            isLoaded = true;
        }
    }

    public override void OnExit()
    {
        if (enemyBrain.navMeshAgent.isOnNavMesh)
        {
            enemyBrain.navMeshAgent.isStopped = true;
            path.Clear();
            enemyBrain.navMeshAgent.ResetPath();
        }
    }

    #endregion

}
