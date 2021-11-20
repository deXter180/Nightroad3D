using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlacedObject : MonoBehaviour
{
    private bool isPlacedOnMenu;
    private InventoryItemSO inventoryItemSO;
    private Vector2Int origin;
    private InventoryItemSO.Dir dir;
    private EquipMenuTile equipMenu;
    private RectTransform rectTransform;
    public bool IsPlacedOnMenu { get => isPlacedOnMenu; }

    //~~~~~~~~~~~~~~~~~ Utilities ~~~~~~~~~~~~~~~~~

    #region
    public void SetupDone()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetEquipTile(EquipMenuTile tile)
    {
        equipMenu = tile;
    }

    public EquipMenuTile GetEquipTile()
    {
        return equipMenu;
    }

    public Vector2Int GetGridPos()
    {
        return origin;
    }

    public void SetOrigin(Vector2Int origin)
    {
        this.origin = origin;
    }

    public List<Vector2Int> GetGridPosList()
    {
        return inventoryItemSO.GetGridPositionList(origin, dir);
    }

    public InventoryItemSO.Dir GetDir()
    {
        return dir;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public ItemTypes GetItemType()
    {
        return inventoryItemSO.itemType;
    }

    public string GetItemID()
    {
        return inventoryItemSO.ItemID;
    }

    public string GetItemName()
    {
        return inventoryItemSO.ItemName;
    }

    public InventoryItemSO GetInventoryItemSO()
    {
        return inventoryItemSO;
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    #endregion

    public static PlacedObject Create(Transform parent, Vector2 anchoredPos, Vector2Int origin, InventoryItemSO.Dir dir, InventoryItemSO inventoryItemSO)
    {
        Transform placedObjectTransform = Instantiate(inventoryItemSO.InventoryPrefab, parent);
        placedObjectTransform.rotation = Quaternion.Euler(0, inventoryItemSO.GetRotationAngle(dir), 0);
        RectTransform rectTransform = placedObjectTransform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.inventoryItemSO = inventoryItemSO;
        placedObject.dir = dir;
        placedObject.origin = origin;
        placedObject.SetupDone();
        placedObject.SetEquipTile(null);
        placedObject.isPlacedOnMenu = false;
        return placedObject;
    }

    public static PlacedObject Create(Transform parent, EquipMenuTile menuTile, Vector2 anchoredPos, Vector2Int origin, InventoryItemSO inventoryItemSO)
    {
        Transform placedObjectTransform = Instantiate(inventoryItemSO.InventoryPrefab, parent);
        placedObjectTransform.localScale = new Vector3(0.8f, 0.8f);
        placedObjectTransform.rotation = Quaternion.identity;
        anchoredPos += new Vector2(15, 10);
        RectTransform rectTransform = placedObjectTransform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.inventoryItemSO = inventoryItemSO;
        placedObject.dir = InventoryItemSO.Dir.Down;
        placedObject.origin = origin;
        placedObject.SetupDone();
        placedObject.SetEquipTile(menuTile);
        placedObject.isPlacedOnMenu = true;
        return placedObject;
    }
}