using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlacedObject : MonoBehaviour
{
    private bool isPlacedOnMenu;
    private bool isPlacedOnInventory;
    private InventoryItemSO inventoryItemSO;
    private Vector2Int origin;
    private Vector2 anchoredPos;
    private InventoryItemSO.Dir dir;
    private EquipMenuWeaponTile weaponTile;
    private EquipMenuSpellTile spellTile;
    private RectTransform rectTransform;
    private TooltipTrigger tooltipTrigger;
    public bool IsPlacedOnMenu { get => isPlacedOnMenu; }
    public bool IsPlaceOnInventory { get => isPlacedOnInventory; }

    //~~~~~~~~~~~~~~~~~ Utilities ~~~~~~~~~~~~~~~~~

    private void Awake()
    {
        tooltipTrigger = GetComponent<TooltipTrigger>();
    }

    private void OnEnable()
    {
        SetupTooltip();
    }

    #region

    public void SetWeaponEquipTile(EquipMenuWeaponTile tile)
    {
        weaponTile = tile;
    }

    public EquipMenuWeaponTile GetWeaponEquipTile()
    {
        return weaponTile;
    }

    public void SetSpellEquipTile(EquipMenuSpellTile tile)
    {
        spellTile = tile;
    }

    public EquipMenuSpellTile GetSpellEquipTile()
    {
        return spellTile;
    }

    public Vector2Int GetGridPos()
    {
        return origin;
    }

    public Vector2 GetAnchoredPos()
    {
        return anchoredPos;
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
        return inventoryItemSO.ItemType;
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

    public void SetupTooltip()
    {
        if (inventoryItemSO != null && tooltipTrigger != null)
        {
            tooltipTrigger.Header = inventoryItemSO.ItemName;
            tooltipTrigger.Content = inventoryItemSO.AttributeAmount.ToString();
        }
    }

    #endregion

    public static PlacedObject Create(Transform parent, Vector2 anchoredPos, Vector2Int origin, InventoryItemSO.Dir dir, InventoryItemSO inventoryItemSO, bool isInventory)
    {
        Transform placedObjectTransform = Instantiate(inventoryItemSO.InventoryPrefab, parent);
        placedObjectTransform.rotation = Quaternion.Euler(0, 0, -inventoryItemSO.GetRotationAngle(dir));
        RectTransform rectTransform = placedObjectTransform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.inventoryItemSO = inventoryItemSO;
        placedObject.dir = dir;
        placedObject.origin = origin;
        placedObject.anchoredPos = anchoredPos;
        placedObject.rectTransform = rectTransform;
        placedObject.SetWeaponEquipTile(null);
        placedObject.SetSpellEquipTile(null);
        placedObject.isPlacedOnMenu = false;
        placedObject.isPlacedOnInventory = isInventory;
        placedObject.SetupTooltip();
        return placedObject;
    }

    public static PlacedObject Create(Transform parent, EquipMenuWeaponTile menuTile, Vector2 anchoredPos, Vector2Int origin, InventoryItemSO inventoryItemSO)
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
        placedObject.anchoredPos = anchoredPos;
        placedObject.rectTransform = rectTransform;
        placedObject.SetWeaponEquipTile(menuTile);
        placedObject.isPlacedOnMenu = true;
        placedObject.isPlacedOnInventory = false;
        placedObject.SetupTooltip();
        return placedObject;
    }

    public static PlacedObject Create(Transform parent, EquipMenuSpellTile menuTile, Vector2 anchoredPos, Vector2Int origin, InventoryItemSO inventoryItemSO)
    {
        Transform placedObjectTransform = Instantiate(inventoryItemSO.InventoryPrefab, parent);
        placedObjectTransform.localScale = new Vector3(0.35f, 0.35f);
        placedObjectTransform.rotation = Quaternion.identity;
        anchoredPos = new Vector2(7, 6);
        RectTransform rectTransform = placedObjectTransform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.inventoryItemSO = inventoryItemSO;
        placedObject.dir = InventoryItemSO.Dir.Down;
        placedObject.origin = origin;
        placedObject.anchoredPos = anchoredPos;
        placedObject.rectTransform = rectTransform;
        placedObject.SetSpellEquipTile(menuTile);
        placedObject.isPlacedOnMenu = true;
        placedObject.isPlacedOnInventory = false;
        placedObject.SetupTooltip();
        return placedObject;
    }
}