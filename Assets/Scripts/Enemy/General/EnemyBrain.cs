using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private EnemyTypes enemyType;
    [HideInInspector] public Vector3 StartPos;
    [HideInInspector] public List<Collider> EnemyCols;
    private Enemy enemy;
    private StateMachine stateMachine;
    private Collider collider;
    public bool IsTargetFleed { get; private set; }
    public bool IsEnemyEntered { get; private set; }
    
    private void Awake()
    {
        SetEnemy();
        stateMachine = new StateMachine(this);
        collider = GetComponent<SphereCollider>();  
    }

    private void OnEnable()
    {
        stateMachine.OnStateChange += StateMachine_OnStateChange;
        collider.isTrigger = true;
        StartPos = transform.position;
        IsTargetFleed = true;
    }

    private void OnDisable()
    {
        stateMachine.OnStateChange -= StateMachine_OnStateChange;
    }

    private void LateUpdate()
    {
        stateMachine.Tick();
    }

    private void StateMachine_OnStateChange(State obj)
    {
        Debug.Log(obj);
    }

    public Enemy GetThisEnemy()
    {
        return enemy;
    }
    public EnemyTypes GetEnemyType()
    {
        return enemyType;
    }
    private Enemy SetEnemy()
    {
        switch (enemyType)
        {
            case EnemyTypes.Giant:
                enemy = new Giant(this);
                break;
            case EnemyTypes.Fighter:
                enemy = new Fighter(this);
                break;
        }
        return enemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IsTargetFleed = false;
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            IsEnemyEntered = true;
            EnemyCols.Add(other);
        }
    }
    private IEnumerator OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            yield return new WaitForSeconds(3f);
            IsTargetFleed = true;
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            yield return new WaitForSeconds(3f);
            IsEnemyEntered = false;
            if (EnemyCols.Contains(other))
            {
                EnemyCols.Remove(other);
            }
        }
    }

}
