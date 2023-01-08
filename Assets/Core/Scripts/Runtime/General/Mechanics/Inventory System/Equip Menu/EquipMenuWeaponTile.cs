using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipMenuWeaponTile : MenuTile
{
    #region Variables

    [SerializeField] private ItemTypes ItemType;
    [SerializeField] private int serialNo;
    public event Action<PlacedObject, int> OnPlacedOnWeaponMenu;
    public event Action<PlacedObject, int> OnRemovedFromWeaponMenu;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        gridWidth = 1;
        gridHeight = 1;
        cellSize = 260;
        grid = new Grid<UIGridObject>(gridWidth, gridHeight, cellSize, transform.position, (Grid<UIGridObject> g, int x, int y) => new UIGridObject(g, x, y));
    }

    public override bool TryPlaceItem(InventoryItemSO inventoryItemSO, Vector2Int placedObjectOrigin, out PlacedObject PO)
    {
        if (inventoryItemSO.ItemType == ItemType)
        {
            var check = base.TryPlaceItem(inventoryItemSO, placedObjectOrigin, out PO);
            if (check)
            {
                OnPlacedOnWeaponMenu?.Invoke(PO, serialNo);
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
            OnRemovedFromWeaponMenu?.Invoke(currentPlacedObject, serialNo);
            currentPlacedObject = null;
        }
        return check;
    }

    public override void ResetTile()
    {
        base.ResetTile();
        OnRemovedFromWeaponMenu?.Invoke(currentPlacedObject, serialNo);
        currentPlacedObject = null;
    }

    #endregion
}
