using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : IHeapItem<PathNode>
{
    private Grid<PathNode> grid;
    private int heapIndex;
    public int x { get; private set; }
    public int z { get; private set; }
    public int gCost;
    public int hCost;
    public int fCost;
    public bool IsWalkable { get; private set; }
    public int HeapIndex { get => heapIndex; set => heapIndex = value; }

    public PathNode PreviousNode;

    public PathNode(Grid<PathNode> grid, int x, int z)
    {
        this.grid = grid;
        this.x = x;
        this.z = z;
    }
    public Vector3 GetThisNodePos()
    {
        return grid.GetWorldPosition(x, z);
    }
    public void SetIsWalkable(bool isWalkable)
    {
        IsWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, z);
    }
    public void calculateFCost()
    {
        fCost = gCost + hCost;
    }

    public int CompareTo(PathNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
