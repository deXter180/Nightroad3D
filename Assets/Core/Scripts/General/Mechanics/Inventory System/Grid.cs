using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid<GridObject>
{
    //~~~~~~~~~~~~~~~~~~~~~~~~ Event Definition ~~~~~~~~~~~~~~~~~~

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }

        public OnGridValueChangedEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~ Variables ~~~~~~~~~~~~~~~~~~~~~

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    public GridObject[,] gridArray { get; private set; }
    private int TotalCells;

    //~~~~~~~~~~~~~~~~~~~~~~ Constructor ~~~~~~~~~~~~~~~~~~~~~~

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<GridObject>, int, int, GridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new GridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y); 
            }
        }
        
        //~~~~~~~~~~~~~~~~~~~~~ Debug ~~~~~~~~~~~~~~~~~~~~~~~~

        bool showDebug = true;
        if (showDebug)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    Debug.DrawLine(GetWorldPosition3D(x, y), GetWorldPosition3D(x, y + 1), Color.green, 200f);
                    Debug.DrawLine(GetWorldPosition3D(x, y), GetWorldPosition3D(x + 1, y), Color.green, 200f);
                }
            }
            Debug.DrawLine(GetWorldPosition3D(0, height), GetWorldPosition3D(width, height), Color.green, 200f);
            Debug.DrawLine(GetWorldPosition3D(width, 0), GetWorldPosition3D(width, height), Color.green, 200f);
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~ Public Utility Methods ~~~~~~~~~~~~~~~~~~~~

    #region

    public int GetWidth()
    {
        return width;
    }
    
    public int GetHeight()
    {
        return height;
    }
    
    public float GetCellSize()
    {
        return cellSize;
    }
    public int GetTotalCells()
    {
        return TotalCells = width * height;
    }
    
    public Vector3 GetWorldPosition3D(int x, int y)
    {
        float PosX = x * cellSize;
        float PosY = y * cellSize;
        Vector3 temp = new Vector3(PosX, PosY, 0) + originPosition;
        return temp;
    }

    public Vector2 GetWorldPosition2D(int x, int y)
    {
        float PosX = x * cellSize;
        float PosY = y * cellSize;
        Vector2 temp = new Vector2(PosX, PosY) + new Vector2(originPosition.x, originPosition.y);
        return temp;
    }

    public Vector3 GetWorldPosAtOrigin(int x, int y)
    {
        float PosX = x * cellSize;
        float PosY = y * cellSize;
        Vector3 temp = new Vector3(PosX, PosY, 0);
        return temp;
    }

    public Vector3 GetWorldPosition(int x, int y, float z)
    {
        float PosX = x * cellSize;
        float PosY = y * cellSize;
        Vector3 temp = new Vector3(PosX + originPosition.x, PosY + originPosition.y, z);
        return temp;
    }

    public void GetCellXYpos(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void GetCellXYLoaclPos(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPos.x / cellSize);
        y = Mathf.FloorToInt(worldPos.y / cellSize);
    }

    public int GetCellXpos(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        return x;
    }

    public int GetCellYpos(Vector3 worldPosition)
    {
        int y = Mathf.FloorToInt((worldPosition - originPosition).y / (cellSize));
        return y;
    }

    public Vector3 GetOriginPos()
    {
        return originPosition;
    }

    public void SetGridObject(int x, int y, GridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
        }
        OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs(x, y));
    }
    
    public void SetGridObject(Vector3 worldPosition, GridObject value)
    {
        GetCellXYpos(worldPosition, out int x, out int y);
        SetGridObject(x, y, value);
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs(x, y));
    }
    
    public GridObject GetGridObject(int x, int y)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default;
        }
    }
    
    public GridObject GetGridObject(Vector3 worldPosition)
    {
        GetCellXYpos(worldPosition, out int x, out int y);
        return GetGridObject(x, y);
    }

    public bool IsValidGridPos(Vector2Int gridPos)
    {
        int x = gridPos.x;
        int y = gridPos.y;
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return true;
        }
        else return false;
    }

    #endregion
}
