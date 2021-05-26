using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private EnemyTypes enemyType;
    [HideInInspector] public Vector3 StartPos;
    private Enemy enemy;
    private StateMachine stateMachine;
    private Collider collider;
    public bool TargetFleed { get; private set; }
     
    private void Awake()
    {
        SetEnemy();
        stateMachine = new StateMachine(this);
        collider = GetComponent<SphereCollider>();
        collider.isTrigger = true;
        
    }

    private void OnEnable()
    {
        stateMachine.OnStateChange += StateMachine_OnStateChange;
        StartPos = transform.position;
        TargetFleed = true;
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
            TargetFleed = false;
        }
    }
    private IEnumerator OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            yield return new WaitForSeconds(3f);
            TargetFleed = true;
        } 
    }

}
