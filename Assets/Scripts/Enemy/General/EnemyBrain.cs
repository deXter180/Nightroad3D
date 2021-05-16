using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField]
    private EnemyTypes enemyType;
    private Enemy enemy;
      

    private void Awake()
    {
        if (EnemyFactory.GetEnemy(enemyType, this) != null)
        {
            enemy = EnemyFactory.GetEnemy(enemyType, this);
        }
        //SetEnemy();
    }

    public Enemy GetThisEnemy()
    {
        return enemy;
    }
    public EnemyTypes GetEnemyType()
    {
        return enemyType;
    }
    //private Enemy SetEnemy()
    //{
    //    switch (enemyType)
    //    {
    //        case EnemyTypes.Giant:
    //            enemy = new Giant(this);
    //            break;
    //        case EnemyTypes.Fighter:
    //            enemy = new Fighter(this);
    //            break;
    //    }
    //    return enemy;
    //}


}
