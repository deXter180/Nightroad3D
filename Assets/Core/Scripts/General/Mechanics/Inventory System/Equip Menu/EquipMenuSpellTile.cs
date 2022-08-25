using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipMenuSpellTile : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize;
    [SerializeField] private ItemTypes itemType;
    [SerializeField] private int serialNo;
    private bool isFilled;
    private Grid<GridObject> grid;
    private RectTransform rectTransform;
    private PlacedObject currentPlacedObject;
    public PlacedObject MenuPlacedObject { get => currentPlacedObject; }
    public bool IsFilled { get => isFilled; }
    public event Action<PlacedObject, int> OnPlacedOnSpellMenu;
    public event Action<PlacedObject, int> OnRemovedFromSpellMenu;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, transform.position, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        isFilled = false;
    }

    #region 
    public Grid<GridObject> GetGrid()
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

    public float GetCellSize()
    {
        return cellSize;
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

    public PlacedObject GetPOinMenu()
    {
        return grid.GetGridObject(0, 0).GetPlacedObject();
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    #endregion

    public bool TryPlaceItem(InventoryItemSO inventoryItemSO, Vector2Int placedObjectOrigin)
    {
        //bool isValidPos = grid.IsValidGridPos(placedObjectOrigin);
        bool isValidType = inventoryItemSO.ItemType == itemType;
        if (isValidType && transform.childCount < 1 && !isFilled)
        {
            PlacedObject placedObject = PlacedObject.Create(transform, this, rectTransform.anchoredPosition, placedObjectOrigin, inventoryItemSO);
            transform.SetAsLastSibling();
            if (currentPlacedObject == null)
            {
                currentPlacedObject = placedObject;
                grid.GetGridObject(0, 0).SetPlacedObject(placedObject);
                isFilled = true;
                OnPlacedOnSpellMenu?.Invoke(placedObject, serialNo);
                return true;
            }           
        }
        return false;
    }

    public bool TryRemoveItem()
    {
        if (grid.GetGridObject(0, 0).GetPlacedObject() != null)
        {
            if (currentPlacedObject != null)
            {
                currentPlacedObject.DestroySelf();
                grid.GetGridObject(0, 0).ClearPlacedObject();
                OnRemovedFromSpellMenu?.Invoke(currentPlacedObject, serialNo);
                isFilled = false;
                currentPlacedObject = null;
                transform.DetachChildren();
                return true;
            }
        }
        return false;
    }

    public void ResetTile()
    {
        transform.DeleteChildren();
        if (currentPlacedObject != null)
        {
            OnRemovedFromSpellMenu?.Invoke(currentPlacedObject, serialNo);
            currentPlacedObject = null;
        }       
        isFilled = false;
    }
}
