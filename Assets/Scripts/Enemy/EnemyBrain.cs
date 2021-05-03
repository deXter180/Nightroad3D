using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField]
    private EnemyTypes enemyType;
    private Enemy enemy;
      

    private void Awake()
    {
        SetEnemy();
        Enemy.EnemyDictionary.Add(enemyType, SetEnemy());
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
            case EnemyTypes.Giant : enemy = new Giant(this);
                break;
            case EnemyTypes.Fighter: enemy = new Fighter(this);
                break;
        }
        
        return enemy;
    }


}
