using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckPointsArea : MonoBehaviour
{
    #region SerializedVariables

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize;
    [SerializeField] private CheckPoint CheckPointPrefab;

    #endregion

    #region Variables

    private Grid<CheckPointGridObject> grid;
    private List<CheckPoint> checkPointList;
    private bool isInitialized => checkPointList != null;
    public bool IsReady { get; private set; }

    #endregion

    #region General

    private void Awake()
    {
        IsReady = false;
        grid = new Grid<CheckPointGridObject>(gridWidth, gridHeight, cellSize, transform.position, (Grid<CheckPointGridObject> g, int x, int y) => new CheckPointGridObject(g, x, y), false, true);
        InitializeInventory();
        for (int i = 0; i <= gridWidth; i++)
        {
            for (int j = 0; j <= gridHeight; j++)
            {
                var obj = Instantiate(CheckPointPrefab, grid.GetWorldPosition3D(i, j), Quaternion.identity);
                checkPointList.Add(obj);
            }
        }
        IsReady = true;
    }

    #endregion

    #region Utility

    public Grid<CheckPointGridObject> GetGrid()
    {
        return grid;
    }

    public int GetWidth()
    {
        return gridWidth;
    }

    public int GetHeight()
    {
        return gridHeight;
    }

    public Vector2Int GetGridPos(Vector3 worldPos)
    {
        grid.GetCellXYpos(worldPos, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public Vector2Int GetGridLocalPos(Vector3 worldPos)
    {
        grid.GetCellXYLoaclPos(worldPos, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public bool IsValidGridPos(Vector2Int gridPos)
    {
        return grid.IsValidGridPos(gridPos);
    }

    #endregion

    #region Mechanics

    public void InitializeInventory()
    {
        if (!isInitialized)
        {
            checkPointList = new List<CheckPoint>();
        }
        else
            return;
    }

    #endregion
}
