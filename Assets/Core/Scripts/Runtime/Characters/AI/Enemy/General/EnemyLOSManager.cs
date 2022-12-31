using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Jobs;
using Unity.Collections;

public class EnemyLOSManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private EnemyTypes enemyType;
    [SerializeField][Range(0, 2)] private float playerHeightOffset = 1f;
    [SerializeField][Range(0.001f, 1f)] private float spherecastRadius = 0.15f;
    [SerializeField] private LayerMask LOSLayers;
    [SerializeField] private bool useJob;
    [SerializeField][Range(1, 500)] private int minJobSize = 10;
    private Transform playerTransform;
    private List<EnemyBrain> AliveEnemyList;

    #endregion

    #region General

    private void Start()
    {
        playerTransform = PlayerController.Instance.transform;
    }

    private void Update()
    {
        CheckForLOS();
    }

    #endregion

    #region Mechanics

    public void CheckForEnemy()
    {
        AliveEnemyList = new List<EnemyBrain>();
        AliveEnemyList = FindObjectsOfType<EnemyBrain>().Where(EB => EB.EnemyType == enemyType).ToList();
    }

    public void RemoveEnemy(EnemyBrain enemyBrain)
    {
        AliveEnemyList.Remove(enemyBrain);  
    }

    private void CheckForLOS()
    {
        if (useJob)
        {
            DoJobsLOSCheck();
        }
        else
        {
            DoSingleThreadLOSCheck();
        }
    }

    private void DoJobsLOSCheck()
    {
        NativeArray<SpherecastCommand> spherecastCommandList = new NativeArray<SpherecastCommand>(
            AliveEnemyList.Count,
            Allocator.TempJob            
            );
        NativeArray<RaycastHit> raycastHitList = new NativeArray<RaycastHit>(
            AliveEnemyList.Count,
            Allocator.TempJob
            );

        Vector3 playerPos = playerTransform.position + Vector3.up * playerHeightOffset;
        for (int i = 0; i < AliveEnemyList.Count; i++)
        {
            spherecastCommandList[i] = new SpherecastCommand(
                AliveEnemyList[i].transform.position,
                spherecastRadius,
                (playerPos - AliveEnemyList[i].transform.position).normalized,
                float.MaxValue,
                LOSLayers
                );
        }

        JobHandle spherecastJob = SpherecastCommand.ScheduleBatch(
            spherecastCommandList,
            raycastHitList,
            minJobSize
            );

        StartCoroutine(OnComplete());

        IEnumerator OnComplete()
        {
            yield return null;
            spherecastJob.Complete();
            for (int i = 0; i < AliveEnemyList.Count; i++)
            {
                if (raycastHitList[i].collider != null && raycastHitList[i].collider.CompareTag("Player"))
                {
                    //Enemy sighted
                }
                else
                {
                    //Enemy not sighted
                }
            }

            spherecastCommandList.Dispose();
            raycastHitList.Dispose();
        }
    }

    private void DoSingleThreadLOSCheck()
    {
        Vector3 playerPos = playerTransform.position + Vector3.up * playerHeightOffset;

        for (int i = 0; i < AliveEnemyList.Count; i++)
        {
            if (Physics.SphereCast(
                AliveEnemyList[i].transform.position,
                spherecastRadius,
                (playerPos - AliveEnemyList[i].transform.position).normalized,
                out RaycastHit hit,
                float.MaxValue,
                LOSLayers
                ) && hit.collider != null && hit.collider.CompareTag("Player"))
            {
                //Enemy sighted
            }
            else
            {
                //Enemy not sighted
            }
        }
    }

    #endregion
}

