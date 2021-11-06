using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlacedObject : MonoBehaviour
{
    private InventoryItemSO inventoryItemSO;
    private Vector2Int origin;
    private InventoryItemSO.Dir dir;


    //~~~~~~~~~~~~~~~~~ Utilities ~~~~~~~~~~~~~~~~~

    public void SetupDone()
    {
        
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

    public static PlacedObject Create(Transform parent, Vector2 anchoredPos, Vector2Int origin, InventoryItemSO.Dir dir, InventoryItemSO inventoryItemSO)
    {
        Transform placedObjectTransform = Instantiate(inventoryItemSO.InventoryPrefab, parent);
        placedObjectTransform.rotation = Quaternion.Euler(0, inventoryItemSO.GetRotationAngle(dir), 0);
        placedObjectTransform.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.inventoryItemSO = inventoryItemSO;
        placedObject.dir = dir;
        placedObject.origin = origin;
        placedObject.SetupDone();
        return placedObject;
    }
}