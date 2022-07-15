using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Roam : State
{
    private List<Vector3> path = new List<Vector3>();
    private bool isLoaded = false;
    private Rigidbody RB;

    public Roam(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        RB = EB.GetComponent<Rigidbody>();            
    }

    public override void Tick()
    {
        if (enemyBrain.IsFrozen)
        {
            stateMachine.SetState(States.Stop);
        }
        else
        {
            if (enemyTrigger.IsTargetFleed)
            {
                enemy.Roam();
            }
            else
            {
                stateMachine.SetState(States.Chase);
            }
        }       
    }

    public override void OnEnter()
    {
        enemyBrain.navMeshAgent.enabled = true;
        if (!isLoaded)
        {
            enemyBrain.navMeshAgent.speed = enemySO.MoveSpeed;
        }
        RB.isKinematic = true;
        for (int i = 0; i < 7; i++)
        {
            path.Add(GetRandomPosition());
        }
        enemy.SetupRoam(path);
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = false;          
        isLoaded = true;
    }

    public override void OnExit()
    {
        path.Clear();
        if (enemyBrain.navMeshAgent.isOnNavMesh)
            enemyBrain.navMeshAgent.isStopped = true;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        return enemyBrain.EnemyTransform.position + randomDir * UnityEngine.Random.Range(50f, 80f);
    }

}
