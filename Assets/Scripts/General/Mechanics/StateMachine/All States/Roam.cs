using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Roam : State
{
    private EnemyBrain enemyBrain;
    private Vector3 roamingPos;
    private bool IsPathStart;
    private int currentPathIndex;
    private List<Vector3> path;

    public Roam(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
    }

    public override void Tick()
    {
        if (enemyBrain.IsTargetFleed)
        {
            IsPathStart = true;
        }
        else
        {
            stateMachine.SetState(States.Approach);
        }
        if (enemyBrain.IsEnemyEntered)
        {
            //enemyBrain.EnemyCols.
        }
        else
        {

        }
    }

    public override void OnEnter()
    {
        rigidbody.isKinematic = true;
        currentPathIndex = 0;
        roamingPos = GetRandomPosition();
        PathManager.RequestPath(enemyTransform.position, roamingPos, OnPathFound);
    }

    public override void OnExit()
    {
        IsPathStart = false;
    }

    private void OnPathFound(List<Vector3> newPath, bool successful)
    {
        if (IsPathStart && successful && newPath != null)
        {
            Debug.Log("Roam Pos : " + roamingPos);
            path = newPath;
            enemyBrain.StopCoroutine(FollowPath(path));
            enemyBrain.StartCoroutine(FollowPath(path));
        }
    }

    private IEnumerator FollowPath(List<Vector3> path)
    {
        while (true)
        {
            if (enemyTransform.position == path[currentPathIndex])
            {
                currentPathIndex++;
                if (currentPathIndex >= path.Count)
                {
                    StopMovement();
                    continue;
                }
                else if(IsPathStart == false) 
                {
                    yield break;
                }
            }
            enemyTransform.position = Vector3.MoveTowards(enemyTransform.position, path[currentPathIndex], enemyBrain.GetThisEnemy().MoveSpeed);
            Debug.Log(path[currentPathIndex]);
            yield return new WaitForEndOfFrame();
        }
    }

    private void StopMovement()
    {
        Debug.Log("Stopped");
        currentPathIndex = 0;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        //return enemyTransform.position + randomDir * UnityEngine.Random.Range(800f, 1000f);
        return enemyBrain.StartPos + randomDir * UnityEngine.Random.Range(1000f, 1500f);
    }
    
    
}
