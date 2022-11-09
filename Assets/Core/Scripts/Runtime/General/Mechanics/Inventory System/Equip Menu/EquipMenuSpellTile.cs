using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipMenuSpellTile : MenuTile
{
    #region Variables

    [SerializeField] private ItemTypes ItemType;
    [SerializeField] private int serialNo;
    public event Action<PlacedObject, int> OnPlacedOnSpellMenu;
    public event Action<PlacedObject, int> OnRemovedFromSpellMenu;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        gridWidth = 1;
        gridHeight = 1;
        cellSize = 130;
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, transform.position, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
    }

    public override bool TryPlaceItem(InventoryItemSO inventoryItemSO, Vector2Int placedObjectOrigin, out PlacedObject PO)
    {
        if (inventoryItemSO.ItemType == ItemType)
        {
            var check = base.TryPlaceItem(inventoryItemSO, placedObjectOrigin, out PO);
            if (check)
            {
                OnPlacedOnSpellMenu?.Invoke(PO, serialNo);
            }
            return check;
        }
        PO = null;
        return false;
    }

    public override bool TryRemoveItem()
    {
        var check = base.TryRemoveItem();
        if (check)
        {
            OnRemovedFromSpellMenu?.Invoke(currentPlacedObject, serialNo);
            currentPlacedObject = null;
        }
        return check;
    }

    public override void ResetTile()
    {
        base.ResetTile();
        OnRemovedFromSpellMenu?.Invoke(currentPlacedObject, serialNo);
        currentPlacedObject = null;
    }

    #endregion
}
