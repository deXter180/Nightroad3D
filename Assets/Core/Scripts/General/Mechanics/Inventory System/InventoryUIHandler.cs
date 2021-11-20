using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InventoryUIHandler : MonoBehaviour
{
    public static InventoryUIHandler Instance { get; private set; }
    public bool IsInventoryON { get; set; }
    private Canvas canvas;  
    private RectTransform UIRect;
    public Transform gridVisual;
    private RectTransform canvasRect;
    [SerializeField] private float CellSize;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else Instance = this;
        canvas = GetComponentInChildren<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        UIRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        canvas.enabled = false;
        IsInventoryON = false;      
    }

    public RectTransform GetCanvasTransfrom()
    {
        return canvasRect;
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    public RectTransform GetUIRect()
    {
        return UIRect;
    }

    public float GetCellSize()
    {
        return CellSize;
    }

    public void Control()
    {
        if (!IsInventoryON)
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
    }
}

//~~~~~~~~~~~~~~~ Grid Object Class ~~~~~~~~~~~~~~~~

#region

public class GridObject
{
    private Grid<GridObject> grid;
    private int x;
    private int y;
    public PlacedObject placedObject;

    public GridObject(Grid<GridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        placedObject = null;
    }

    public override string ToString()
    {
        return x + ", " + y + "\n" + placedObject;
    }

    public void SetPlacedObject(PlacedObject placedObject)
    {
        this.placedObject = placedObject;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void ClearPlacedObject()
    {
        placedObject = null;
        grid.TriggerGridObjectChanged(x, y);
    }

    public PlacedObject GetPlacedObject()
    {
        return placedObject;
    }

    public bool CanBuild()
    {
        return placedObject == null;
    }

    public bool HasPlacedObject()
    {
        return placedObject != null;
    }
}

#endregion
