using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Roam : State
{
    private EnemyBrain enemyBrain;
    private EnemyTrigger enemyTrigger;
    private List<Vector3> path = new List<Vector3>();
    private int pathIndex;
    private bool isLoaded = false;

    public Roam(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
        enemyTrigger = EB.GetComponentInChildren<EnemyTrigger>();
    }

    public override void Tick()
    {
        if (enemyBrain.IsFrozen)
        {
            stateMachine.SetState(States.Stop);
        }
        else
        {
            if (enemyTrigger.IsTargetFleed && !enemyTrigger.IsTargetInRange)
            {
                FollowPath();
            }
            else if (enemyTrigger.IsTargetInRange && !enemyTrigger.IsTargetFleed)
            {
                stateMachine.SetState(States.Attack);
            }
            else if (!enemyTrigger.IsTargetInRange && !enemyTrigger.IsTargetFleed)
            {
                stateMachine.SetState(States.Chase);
            }
        }       
    }

    public override void OnEnter()
    {
        if (isLoaded)
        {
            enemyBrain.navMeshAgent.speed = enemyBrain.GetThisEnemy().ThisEnemySO.MoveSpeed;
        }
        rigidbody.isKinematic = true;
        for (int i = 0; i < 7; i++)
        {
            path.Add(GetRandomPosition());
        }
        enemyBrain.navMeshAgent.isStopped = false;
        pathIndex = 0;
        isLoaded = true;
    }

    public override void OnExit()
    {
        path.Clear();
        enemyBrain.navMeshAgent.isStopped = true;
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
            enemyBrain.navMeshAgent.SetDestination(path[pathIndex]);
        }
        
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        //return enemyTransform.position + randomDir * UnityEngine.Random.Range(800f, 1000f);
        return enemyBrain.StartPos + randomDir * UnityEngine.Random.Range(50f, 100f);
    }

}
