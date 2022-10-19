using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTile : MonoBehaviour
{
    #region Variables

    protected int gridWidth;
    protected int gridHeight;
    protected float cellSize;
    private bool isFilled;
    protected Grid<GridObject> grid;
    protected RectTransform rectTransform;
    protected PlacedObject currentPlacedObject;
    public PlacedObject MenuPlacedObject { get => currentPlacedObject; }
    public bool IsFilled { get => isFilled; }

    #endregion

    #region Utilities
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

    #region General

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        isFilled = false;
    }

    public virtual bool TryPlaceItem(InventoryItemSO inventoryItemSO, Vector2Int placedObjectOrigin, out PlacedObject PO)
    {
        if (transform.childCount < 1 && !isFilled)
        {
            PO = PlacedObject.Create(transform, this, rectTransform.anchoredPosition, placedObjectOrigin, inventoryItemSO);
            transform.SetAsLastSibling();
            if (currentPlacedObject == null)
            {
                currentPlacedObject = PO;
                grid.GetGridObject(0, 0).SetPlacedObject(PO);
                isFilled = true;
                return true;
            }
        }
        PO = null;
        return false;
    }

    public virtual bool TryRemoveItem()
    {
        if (grid.GetGridObject(0, 0).GetPlacedObject() != null)
        {
            if (currentPlacedObject != null)
            {
                currentPlacedObject.DestroySelf();
                grid.GetGridObject(0, 0).ClearPlacedObject();
                isFilled = false;
                currentPlacedObject = null;
                transform.DetachChildren();
                return true;
            }
        }
        return false;
    }

    public virtual void ResetTile()
    {
        transform.DeleteChildren();
        if (currentPlacedObject != null)
        {
            currentPlacedObject = null;
        }
        isFilled = false;
    }

    public virtual bool IsOnTile(Vector2 currentPosition, Camera UICam)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, currentPosition, UICam))
        {
            return true;
        }
        return false;
    }

    

    #endregion
}
