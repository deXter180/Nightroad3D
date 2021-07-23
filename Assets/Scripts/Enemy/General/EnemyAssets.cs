using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAssets : MonoBehaviour
{
    public static EnemyAssets Instance { get; private set; }
    public List<EnemySO> EnemySOList;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    public EnemySO GetEnemySOFromList(EnemyTypes enemyType)
    {
        foreach (EnemySO enemySO in EnemySOList)
        {
            if (enemySO.EnemyType == enemyType)
            {
                return enemySO;
            }
        }
        return null;
    }


}
