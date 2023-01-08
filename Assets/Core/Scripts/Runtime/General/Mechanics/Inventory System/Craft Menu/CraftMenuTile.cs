using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CraftMenuTile : MenuTile
{
    #region Variables

    private ItemTypes itemType;
    public event Action<PlacedObject> OnPlacedInCraftMenu;
    public event Action<PlacedObject> OnRemovedInCraftMenu;
    public ItemTypes ItemType => itemType;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        gridWidth = 1;
        gridHeight = 1;
        cellSize = 125;
        grid = new Grid<UIGridObject>(gridWidth, gridHeight, cellSize, transform.position, (Grid<UIGridObject> g, int x, int y) => new UIGridObject(g, x, y));
    }

    public override bool TryPlaceItem(InventoryItemSO inventoryItemSO, Vector2Int placedObjectOrigin, out PlacedObject PO)
    {
        if (inventoryItemSO.Craftable)
        {
            transform.SetAsFirstSibling();
            var check = base.TryPlaceItem(inventoryItemSO, placedObjectOrigin, out PO);
            if (check)
            {
                itemType = inventoryItemSO.ItemType;
                OnPlacedInCraftMenu?.Invoke(PO);
                return true;
            }            
        }
        PO = null;
        return false;
    }

    public override bool TryRemoveItem()
    {
        
        var check = base.TryRemoveItem();
        if (check)
        {
            itemType = ItemTypes.None;
            OnRemovedInCraftMenu?.Invoke(currentPlacedObject);
            currentPlacedObject = null;
            return true;
        }
        return false;
    }

    public override void ResetTile()
    {
        base.ResetTile();
        OnRemovedInCraftMenu?.Invoke(currentPlacedObject);
        currentPlacedObject = null;
    }

    #endregion
}
