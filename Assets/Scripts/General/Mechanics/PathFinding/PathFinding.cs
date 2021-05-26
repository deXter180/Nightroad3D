using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    private const int MOVE_STRAIGHT_COST = 150;
    private const int MOVE_DIAGONAL_COST = 212;
    private const float CELL_SIZE = 150f;
    private float cellRadius = CELL_SIZE / 2;
    private LayerMask layerMask;
    private Grid<PathNode> grid;
    private Heap<PathNode> OpenList;
    private List<PathNode> ClosedList;
    //public static PathFinding Instance { get; private set; }

    public PathFinding(int width, int height, Vector3 originPos, LayerMask layerMask)
    {
        this.layerMask = layerMask;
        //Instance = (Instance == null) ? this : null;
        grid = new Grid<PathNode>(width, height, CELL_SIZE, originPos, (Grid<PathNode> g, int x, int z) => new PathNode(g, x, z));
        AddObstacles();
    }

    public void StartFindPath(Vector3 startPos, Vector3 endPos, MonoBehaviour mono)
    {
        mono.StartCoroutine(FindPath(startPos, endPos));
    }

    private IEnumerator FindPath(Vector3 startWorldPos, Vector3 endWorldPos)//int startX, int startZ, int endX, int endZ)
    {
        List<Vector3> pathPoints = new List<Vector3>();
        bool isPathfound = false;
        PathNode startNode = grid.GetGridObject(startWorldPos);
        PathNode endNode = grid.GetGridObject(endWorldPos);
        if (startNode != null && endNode != null)
        {
            if (startNode.IsWalkable && endNode.IsWalkable && startNode != null && endNode != null)
            {
                OpenList = new Heap<PathNode>(grid.GetTotalCells());
                OpenList.Add(startNode);
                ClosedList = new List<PathNode>();
                for (int x = 0; x < grid.GetWidth(); x++)
                {
                    for (int z = 0; z < grid.GetHeight(); z++)
                    {
                        PathNode pathNode = grid.GetGridObject(x, z);
                        pathNode.gCost = int.MaxValue;
                        pathNode.calculateFCost();
                        pathNode.PreviousNode = null;
                    }
                }
                startNode.gCost = 0;
                startNode.hCost = GetDistanceCost(startNode, endNode);
                startNode.calculateFCost();
                while (OpenList.Count() > 0)
                {
                    PathNode currentNode = OpenList.RemoveFirstItem();
                    if (currentNode == endNode)
                    {
                        //Reached final node
                        isPathfound = true;
                        break;
                    }
                    ClosedList.Add(currentNode);
                    foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                    {
                        if (!neighbourNode.IsWalkable || ClosedList.Contains(neighbourNode)) continue;
                        int possibleFCost = currentNode.gCost + GetDistanceCost(currentNode, neighbourNode);
                        if (possibleFCost < neighbourNode.gCost)
                        {
                            neighbourNode.PreviousNode = currentNode;
                            neighbourNode.gCost = possibleFCost;
                            neighbourNode.hCost = GetDistanceCost(neighbourNode, endNode);
                            neighbourNode.calculateFCost();
                            if (!OpenList.ContainItem(neighbourNode))
                            {
                                OpenList.Add(neighbourNode);
                            }
                            else OpenList.UpdateItem(neighbourNode);
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
            if (isPathfound)
            {
                pathPoints = CalculatePath(endNode, startWorldPos);
                PathManager.Instance.FinishedProcessingPath(pathPoints, isPathfound);
            }
        }
        else Debug.Log("Out of Grid!"); 
    }

    private PathNode GetNode (int x, int z)
    {
        return grid.GetGridObject(x, z);
    }

    private PathNode GetNode(Vector3 worldPos)
    {
        return grid.GetGridObject(worldPos);
    }

    private void AddObstacles()
    {
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                if (Physics.CheckSphere(grid.GetWorldPosition(x, z), cellRadius, layerMask))
                {
                    grid.GetGridObject(x, z).SetIsWalkable(false);
                }
                else grid.GetGridObject(x, z).SetIsWalkable(true);
            }
        }
    }

    private int GetDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int zDistance = Mathf.Abs(a.z - b.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private List<Vector3> CalculatePath(PathNode endNode, Vector3 StartPos)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 endNodePos = grid.GetWorldPosition(endNode.x, StartPos.y, endNode.z);
        path.Add(endNodePos);
        PathNode currentNode = endNode;
        while (currentNode.PreviousNode != null)
        {
            path.Add(grid.GetWorldPosition(currentNode.PreviousNode.x, StartPos.y, currentNode.PreviousNode.z));
            currentNode = currentNode.PreviousNode;
        }
        path.Reverse();
        return path;
    }

    private List<PathNode> GetNeighbourList(PathNode node)
    {
        List<PathNode> neighbours = new List<PathNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                    continue;
                int checkX = node.x + x;
                int checkZ = node.z + z;

                if (checkX >= 0 && checkX < grid.GetWidth() && checkZ >= 0 && checkZ < grid.GetHeight())
                {
                    neighbours.Add(grid.GetGridObject(checkX, checkZ));
                }
            }
        }
        return neighbours;
    }

    //private List<PathNode> GetNeighbourList(int x, int z)
    //{
    //    PathNode node = grid.GetGridObject(x, z);
    //    NeighbourDict.TryGetValue(node, out List<PathNode> pathNode);
    //    return pathNode;
    //}

    //private void SetNeighbourList()
    //{
    //    for (int x = 0; x < grid.GetWidth(); x++)
    //    {
    //        for (int z = 0; z < grid.GetHeight(); z++)
    //        {
    //            List<PathNode> path = new List<PathNode>();
    //            PathNode node = GetNode(x, z);
    //            if (node.x - 1 >= 0)
    //            {
    //                //Left
    //                path.Add(GetNode(node.x - 1, node.z));
    //                //Left Down
    //                if (node.z - 1 >= 0) path.Add(GetNode(node.x - 1, node.z - 1));
    //                //Left Up
    //                if (node.z + 1 < grid.GetHeight()) path.Add(GetNode(node.x - 1, node.z + 1));
    //            }
    //            if (node.x + 1 < grid.GetWidth())
    //            {
    //                //Right
    //                path.Add(GetNode(node.x + 1, node.z));
    //                //Right Down
    //                if (node.z - 1 >= 0) path.Add(GetNode(node.x + 1, node.z - 1));
    //                //Right Up
    //                if (node.z + 1 < grid.GetHeight()) path.Add(GetNode(node.x + 1, node.z + 1));
    //            }
    //            //Down
    //            if (node.z - 1 >= 0) path.Add(GetNode(node.x, node.z - 1));
    //            //Up
    //            if (node.z + 1 < grid.GetHeight()) path.Add(GetNode(node.x, node.z + 1));
    //            NeighbourDict.Add(node, path);
    //        }
    //    }
    //} //Calling in PathFinding ctor

    //private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    //{
    //    PathNode lowestFCostNode = pathNodeList[0];

    //    for (int i = 1; i < pathNodeList.Count; i++)
    //    {
    //        if (pathNodeList[i].fCost < lowestFCostNode.fCost)
    //        {
    //            lowestFCostNode = pathNodeList[i];
    //        }
    //    }
    //    return lowestFCostNode;
    //}

    //private List<PathNode> GetNeighbourList(PathNode currentNode) //Currently not using
    //{
    //    List<PathNode> neighbourList = new List<PathNode>();

    //    if (currentNode.x - 1 >= 0)
    //    {
    //        //Left
    //        neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z));
    //        //Left Down
    //        if (currentNode.z - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z - 1));
    //        //Left Up
    //        if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z + 1));
    //    }
    //    if (currentNode.x + 1 < grid.GetWidth())
    //    {
    //        //Right
    //        neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z));
    //        //Right Down
    //        if (currentNode.z - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z - 1));
    //        //Right Up
    //        if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z + 1));                             
    //    }
    //    //Down
    //    if (currentNode.z - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.z - 1));
    //    //Up
    //    if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.z + 1));

    //    return neighbourList;
    //} 

    //private IEnumerator FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    //{
    //    grid.GetCellXZpos(startWorldPos, out int startX, out int startZ);
    //    grid.GetCellXZpos(endWorldPos, out int endX, out int endZ);
    //    //FindPath(startX, startZ, endX, endZ);
    //    yield return new WaitForEndOfFrame();
    //}

}
