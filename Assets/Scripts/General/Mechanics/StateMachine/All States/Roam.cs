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
    private int enemyBitMask = 1 >> 12;

    public Roam(EnemyBrain EB, StateMachine SM) : base(EB.gameObject, SM)
    {
        enemyBrain = EB;
    }

    public override void Tick()
    {
        if (enemyBrain.IsTargetFleed)
        {
            IsPathStart = true;
            CheckForCol();
        }
        else
        {
            stateMachine.SetState(States.Approach);
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
            //Debug.Log("Roam Pos : " + roamingPos);
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
            //Debug.Log(path[currentPathIndex]);
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

    private void CheckForCol()
    {
        Debug.DrawRay(enemyTransform.position, enemyTransform.forward, Color.black);
        if (Physics.Raycast(enemyTransform.position, enemyTransform.forward, out RaycastHit hit, enemyBitMask))
        {
            IsPathStart = false;
            enemyBrain.IsEnemyEntered = true;
            if (hit.collider != null)
            {
                Evade(enemyTransform, hit.collider.transform);
            }
        }
    }

    private void Evade(Transform obj1, Transform obj2)
    {
        if (enemyBrain.IsEnemyEntered)
        {
            Vector3 direction = obj1.position - obj2.position;
            direction.y = 0f;
            float distance = direction.magnitude;
            if (distance < 3f) //Add distance constant for enemy
            {
                //Add roational constant for enemy
                obj1.rotation = Quaternion.Slerp(obj1.rotation, Quaternion.LookRotation(direction), Time.deltaTime);
                obj1.position += direction.normalized * enemyBrain.GetThisEnemy().MoveSpeed * Time.deltaTime;
            }
        }
        else
        {
            StopMovement();
            IsPathStart = true;
        }
    }

}
