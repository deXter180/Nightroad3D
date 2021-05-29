using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathManager : MonoBehaviour
{
    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest currentPathRequest;
    private PathFinding pathfind;
    private bool IsProcessingPath;
    [SerializeField] private LayerMask layerMask;

    public static PathManager Instance { get; private set; }

    private void Awake()
    {
        Instance = Instance == null ? this : null;
        pathfind = new PathFinding(60, 60, transform.position, layerMask);
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<List<Vector3>, bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        Instance.pathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!Instance.IsProcessingPath && Instance.pathRequestQueue.Count > 0)
        {
            Instance.currentPathRequest = pathRequestQueue.Dequeue();
            Instance.IsProcessingPath = true;
            pathfind.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, this);
        }
    }

    public void FinishedProcessingPath(List<Vector3> path, bool success)
    {
        Instance.currentPathRequest.callback(path, success);
        Instance.IsProcessingPath = false;
        Instance.TryProcessNext();
    }

    private struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<List<Vector3>, bool> callback;

        public PathRequest(Vector3 start, Vector3 end, Action<List<Vector3>, bool> _callback)
        {
            pathStart = start;
            pathEnd = end;
            callback = _callback;
        }
    }
}
