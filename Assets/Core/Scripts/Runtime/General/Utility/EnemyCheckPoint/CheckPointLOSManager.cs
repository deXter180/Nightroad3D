using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System.Linq;

public class CheckPointLOSManager : Singleton<CheckPointLOSManager>
{
    #region SerializedVariables

    [SerializeField] private LayerMask layerToIgnore;
    [SerializeField] private int maxColliderToHit = 1;
    [SerializeField][Range(0, 2)] private float playerHeightOffset = 1f;
    [SerializeField][Range(1, 500)] private int minJobSize = 10;

    #endregion

    #region Variables

    private Transform playerTransform;
    private bool isReady = false;
    private string playerTag = "Player";
    private string frictionLayerTag = "NoFriction";
    private List<CheckPoint> availableCheckPoints;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        
    }

    private void Start()
    {
        playerTransform = PlayerController.Instance.transform;
    }

    private void Update()
    {
        if (isReady)
        {
            CheckForLOS();
        }
    }

    #endregion

    #region Mechanics

    public void Setup()
    {
        availableCheckPoints = new List<CheckPoint>();
        availableCheckPoints = FindObjectsOfType<CheckPoint>().ToList();
        isReady = true;
    }

    private void CheckForLOS()
    {
        var commands = new NativeArray<RaycastCommand>(
            availableCheckPoints.Count,
            Allocator.TempJob
            );

        var results = new NativeArray<RaycastHit>(
            availableCheckPoints.Count,
            Allocator.TempJob
            );

        Vector3 playerPos = playerTransform.position + Vector3.up * playerHeightOffset;

        for (int i = 0; i < availableCheckPoints.Count; i++)
        {
            commands[i] = new RaycastCommand(
                availableCheckPoints[i].transform.position,
                (playerPos - availableCheckPoints[i].transform.position).normalized,
                float.MaxValue,
                layerToIgnore,
                maxColliderToHit
                );
        }

        JobHandle raycastJob = RaycastCommand.ScheduleBatch(
            commands,
            results,
            minJobSize
            );

        StartCoroutine(OnComplete());

        IEnumerator OnComplete()
        {
            yield return null;
            raycastJob.Complete();
            for (int i = 0; i < availableCheckPoints.Count; i++)
            {
                if (results[i].collider != null)
                {
                    if (results[i].collider.CompareTag(playerTag) || results[i].collider.CompareTag(frictionLayerTag))
                    {
                        //Player Sighted
                        availableCheckPoints[i].SetAvailability(true);
                    }
                    else
                    {
                        //Player Not Sighted
                        availableCheckPoints[i].SetAvailability(false);
                    }                                       
                }
                else
                {
                    //Player Not Sighted
                    availableCheckPoints[i].SetAvailability(false);
                }
            }

            commands.Dispose();
            results.Dispose();
        }
    }

    #endregion
}
