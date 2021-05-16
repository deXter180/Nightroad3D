using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private const float CELL_SIZE = 10f;
    private LayerMask layerMask;
    private Grid<PathNode> grid;
    private Heap<PathNode> OpenList;
    private List<PathNode> ClosedList;
    private SortedDictionary<PathNode, List<PathNode>> NeighbourDict;
    //public static PathFinding Instance { get; private set; }

    public PathFinding(int width, int height, Vector3 originPos, LayerMask layerMask)
    {
        this.layerMask = layerMask;
        //Instance = (Instance == null) ? this : null;
        grid = new Grid<PathNode>(width, height, CELL_SIZE, originPos, (Grid<PathNode> g, int x, int z) => 
        {   
            PathNode path = new PathNode(g, x, z);
            AddObstacles(path);
            return path;
        });
        NeighbourDict = new SortedDictionary<PathNode, List<PathNode>>();
        SetNeighbourList();
    }

    private void AddObstacles(PathNode pathNode)
    {
        if (Physics.Raycast(pathNode.GetThisNodePos(), Vector3.up, 0.01f, layerMask))
        {
            pathNode.SetIsWalkable(false);
        }
        else pathNode.SetIsWalkable(true);
    }

    public List<PathNode> FindNodes(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        grid.GetCellXZpos(startWorldPos, out int startX, out int startZ);
        grid.GetCellXZpos(endWorldPos, out int endX, out int endZ);
        List<PathNode> path = FindPath(startX, startZ, endX, endZ);
        if (path != null) return path;
        else return null;
    }

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        grid.GetCellXZpos(startWorldPos, out int startX, out int startZ);
        grid.GetCellXZpos(endWorldPos, out int endX, out int endZ);
        List<PathNode> path = FindPath(startX, startZ, endX, endZ);
        if (path == null) return null;
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode node in path)
            {
                vectorPath.Add(grid.GetWorldPosition(node.x, node.z));
            }
            return vectorPath;
        }
    }

    public List<PathNode> FindPath(int startX, int startZ, int endX, int endZ)
    {
        PathNode startNode = grid.GetGridObject(startX, startZ);
        PathNode endNode = grid.GetGridObject(endX, endZ);
        if (startNode.IsWalkable && endNode.IsWalkable)
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
                    return CalculatePath(endNode);
                }
                ClosedList.Add(currentNode);
                foreach (PathNode neighbourNode in NeighbourDict[currentNode])
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
                    }
                }
            }
        }
        
        // Out of nodes on OpenList
        return null;
    }

    public PathNode GetNode (int x, int z)
    {
        return grid.GetGridObject(x, z);
    }
    public PathNode GetNode(Vector3 worldPos)
    {
        return grid.GetGridObject(worldPos);
    }

    private int GetDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int zDistance = Mathf.Abs(a.z - b.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.PreviousNode != null)
        {
            path.Add(currentNode.PreviousNode);
            currentNode = currentNode.PreviousNode;
        }
        path.Reverse();
        return path;
    }

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

    public List<PathNode> GetNeighbourList(int x, int z)
    {
        PathNode node = grid.GetGridObject(x, z);
        NeighbourDict.TryGetValue(node, out List<PathNode> pathNode);
        return pathNode;
    }

    private void SetNeighbourList()
    {
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                List<PathNode> path = new List<PathNode>();
                PathNode node = GetNode(x, z);
                if (node.x - 1 >= 0)
                {
                    //Left
                    path.Add(GetNode(node.x - 1, node.z));
                    //Left Down
                    if (node.z - 1 >= 0) path.Add(GetNode(node.x - 1, node.z - 1));
                    //Left Up
                    if (node.z + 1 < grid.GetHeight()) path.Add(GetNode(node.x - 1, node.z + 1));
                }
                if (node.x + 1 < grid.GetWidth())
                {
                    //Right
                    path.Add(GetNode(node.x + 1, node.z));
                    //Right Down
                    if (node.z - 1 >= 0) path.Add(GetNode(node.x + 1, node.z - 1));
                    //Right Up
                    if (node.z + 1 < grid.GetHeight()) path.Add(GetNode(node.x + 1, node.z + 1));
                }
                //Down
                if (node.z - 1 >= 0) path.Add(GetNode(node.x, node.z - 1));
                //Up
                if (node.z + 1 < grid.GetHeight()) path.Add(GetNode(node.x, node.z + 1));
                NeighbourDict.Add(node, path);
            }
        }
    } //Calling in PathFinding ctor

    
}
