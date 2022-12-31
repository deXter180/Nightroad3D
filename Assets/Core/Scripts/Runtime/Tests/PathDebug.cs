#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathDebug : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agentToDebug;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (agentToDebug.hasPath)
        {
            lineRenderer.positionCount = agentToDebug.path.corners.Length;
            lineRenderer.SetPositions(agentToDebug.path.corners);
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}

#endif