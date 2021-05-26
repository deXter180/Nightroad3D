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

    public Roam(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
    }

    public override void Tick()
    {
        if (enemyBrain.TargetFleed)
        {
            IsPathStart = true;
        }
        else
        {
            stateMachine.SetState(States.Approach);
        }
    }

    public override void OnEnter()
    {
        rigidbody.isKinematic = true;
        roamingPos = GetRandomPosition();
        currentPathIndex = 0;
        roamingPos = GetRandomPosition();
        PathManager.RequestPath(enemyTransform.position, roamingPos, OnPathFound);
    }

    public override void OnExit()
    {
        
    }

    private void OnPathFound(List<Vector3> newPath, bool successful)
    {
        if (IsPathStart && successful && newPath != null)
        {
            Debug.Log("Roam Pos : " + roamingPos);
            enemyBrain.StopCoroutine(FollowPath(newPath));
            enemyBrain.StartCoroutine(FollowPath(newPath));
        }
    }

    private IEnumerator FollowPath(List<Vector3> newPath)
    {
        while (true)
        {
            if (enemyTransform.position == newPath[currentPathIndex])
            {
                currentPathIndex++;
                if (currentPathIndex >= newPath.Count)
                {
                    StopMovement();
                    continue;
                }
            }
            enemyTransform.position = Vector3.MoveTowards(enemyTransform.position, newPath[currentPathIndex], enemyBrain.GetThisEnemy().MoveSpeed);
            Debug.Log(newPath[currentPathIndex]);
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
