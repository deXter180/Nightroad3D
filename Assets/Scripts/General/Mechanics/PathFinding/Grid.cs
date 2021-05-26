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
        public int Z { get; set; }

        public OnGridValueChangedEventArgs(int x, int z)
        {
            X = x;
            Z = z;
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
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                for (int i = 0; i < GetTotalCells(); i++)
                {
                    gridArray[x, z] = createGridObject(this, x, z);
                    
                }   
            }
        }
        
        //~~~~~~~~~~~~~~~~~~~~~ Debug ~~~~~~~~~~~~~~~~~~~~~~~~

        bool showDebug = true;
        if (showDebug)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.green, 200f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.green, 200f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.green, 200f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.green, 200f);
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~ Public Utility Methods ~~~~~~~~~~~~~~~~~~~~

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
    
    public Vector3 GetWorldPosition(int x, int z)
    {
        float PosX = x * cellSize;
        float PosZ = z * cellSize;
        Vector3 temp = new Vector3(PosX, 0, PosZ) + originPosition;
        return temp;
    }
    public Vector3 GetWorldPosition(int x, float y, int z)
    {
        float PosX = x * cellSize;
        float PosZ = z * cellSize;
        Vector3 temp = new Vector3(PosX + originPosition.x, y, PosZ + originPosition.z);
        return temp;
    }

    public void GetCellXZpos(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public int GetCellXpos(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        return x;
    }

    public int GetCellZpos(Vector3 worldPosition)
    {
        int z = Mathf.FloorToInt((worldPosition - originPosition).z / (cellSize));
        return z;
    }

    public Vector3 GetOriginPos()
    {
        return originPosition;
    }

    public void SetGridObject(int x, int z, GridObject value)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            gridArray[x, z] = value;
        }
        OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs(x, z));
    }
    
    public void SetGridObject(Vector3 worldPosition, GridObject value)
    {
        GetCellXZpos(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }

    public void TriggerGridObjectChanged(int x, int z)
    {
        OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs(x, z));
    }
    
    public GridObject GetGridObject(int x, int z)
    {
        if(x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x, z];
        }
        else
        {
            return default;
        }
    }
    
    public GridObject GetGridObject(Vector3 worldPosition)
    {
        GetCellXZpos(worldPosition, out int x, out int z);
        return GetGridObject(x, z);
    }
}
