using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Roam : State
{
    private EnemyBrain enemyBrain;
    private List<Vector3> path = new List<Vector3>();
    private int pathIndex;

    public Roam(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
    }

    public override void Tick()
    {
        if (enemyBrain.IsTargetFleed)
        {
            FollowPath();
        }
        else
        {
            enemyBrain.navMeshAgent.isStopped = true;
            stateMachine.SetState(States.Chase);
        }
    }

    public override void OnEnter()
    {
        rigidbody.isKinematic = true;
        enemyBrain.navMeshAgent.isStopped = false;
        for (int i = 0; i < 7; i++)
        {
            path.Add(GetRandomPosition());
        }
        pathIndex = 0;
    }

    public override void OnExit()
    {
        path.Clear();
    }

    private void FollowPath()
    {
        if (enemyBrain.navMeshAgent.remainingDistance <= 5f)
        {
            pathIndex++;
            if (pathIndex >= path.Count)
            {
                pathIndex = 0;
            }
        }
        enemyBrain.navMeshAgent.SetDestination(path[pathIndex]);
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        //return enemyTransform.position + randomDir * UnityEngine.Random.Range(800f, 1000f);
        return enemyBrain.StartPos + randomDir * UnityEngine.Random.Range(1000f, 1500f);
    }

}
