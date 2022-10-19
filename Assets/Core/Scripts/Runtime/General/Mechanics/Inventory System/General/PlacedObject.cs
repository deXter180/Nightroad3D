using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlacedObject : MonoBehaviour
{
    #region Variables

    private bool isPlacedOnMenu;
    private bool isPlacedOnInventory;
    private bool isStopDragging = false;
    private InventoryItemSO inventoryItemSO;
    private Vector2Int origin;   
    private Vector2 anchoredPos;
    private InventoryItemSO.Dir dir;
    private EquipMenuWeaponTile weaponTile;
    private CanvasGroup canvasGroup;
    private EquipMenuSpellTile spellTile;
    private CraftMenuTile craftTile;
    private RectTransform rectTransform;
    private TooltipTrigger tooltipTrigger;
    private InventoryDragDrop dragDrop;
    private InventorySystem inventorySystem;
    private InventoryDragDropSystem dragDropSystem;
    public bool IsPlacedOnMenu { get => isPlacedOnMenu; }
    public bool IsPlaceOnInventory { get => isPlacedOnInventory; }
    public bool IsStopDragging { get => isStopDragging; }

    #endregion

    //~~~~~~~~~~~~~~~~~ Utilities ~~~~~~~~~~~~~~~~~

    #region General

    private void Update()
    {
        if (dragDropSystem.IsDragging)
        {
            TooltipSystem.Hide();
            tooltipTrigger.SetStop(true);
        }
        else
        {
            tooltipTrigger.SetStop(false);
        }
    }

    public void StopDragging(bool isTrue)
    {
        isStopDragging = isTrue;
    }

    public void TooltipControl(bool isTrue)
    {
        tooltipTrigger.SetStop(isTrue);
    }

    public void UpdateTooltip()
    {
        tooltipTrigger.UpdateTooltip();
    }

    #endregion

    #region Utilities

    public void RemoveMenuTile()
    {
        weaponTile = null;
        spellTile = null;
        craftTile = null;
    }

    public void SetMenuTile(EquipMenuWeaponTile tile)
    {
        weaponTile = tile;
    }

    public void SetMenuTile(EquipMenuSpellTile tile)
    {
        spellTile = tile;
    }

    public void SetMenuTile(CraftMenuTile tile)
    {
        craftTile = tile;
    }

    public EquipMenuWeaponTile GetWeaponEquipTile()
    {
        return weaponTile;
    }

    public EquipMenuSpellTile GetSpellEquipTile()
    {
        return spellTile;
    }

    public CraftMenuTile GetCraftTile()
    {
        return craftTile;
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
            if (inventoryItemSO.Stackable && inventorySystem.ItemDict.ContainsKey(GetItemType()))
            {
                tooltipTrigger.Content1 = $" Amount : {inventorySystem.ItemDict[GetItemType()].ToString()}";                
            }
            else
            {
                tooltipTrigger.Content1 = $" Amount : {1.ToString()}";
            }          
            if (inventoryItemSO.AttributeAmount != 0)
            {
                tooltipTrigger.Content2 = $" Value : {inventoryItemSO.AttributeAmount.ToString()}";
                if (inventoryItemSO.RequirementList.Count > 0)
                {
                    tooltipTrigger.Content3 = inventoryItemSO.GetRequirementText();
                }
            }
            else
            {
                if (inventoryItemSO.RequirementList.Count > 0)
                {
                    tooltipTrigger.Content2 = inventoryItemSO.GetRequirementText();
                }               
            }            
        }
    }

    #endregion

    #region Mechanics

    public static PlacedObject Create(Transform parent, Vector2 anchoredPos, Vector2Int origin, InventoryItemSO.Dir dir, InventoryItemSO inventoryItemSO, bool isInventory)
    {
        Transform placedObjectTransform = Instantiate(inventoryItemSO.InventoryPrefab, parent);
        placedObjectTransform.rotation = Quaternion.Euler(0, 0, -inventoryItemSO.GetRotationAngle(dir));
        RectTransform rectTransform = placedObjectTransform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.tooltipTrigger = placedObject.GetComponent<TooltipTrigger>();
        placedObject.canvasGroup = placedObject.GetComponent<CanvasGroup>();
        placedObject.dragDrop = placedObject.GetComponent<InventoryDragDrop>();
        placedObject.inventorySystem = InventorySystem.Instance;
        placedObject.dragDropSystem = InventoryDragDropSystem.Instance;
        placedObject.inventoryItemSO = inventoryItemSO;
        placedObject.dir = dir;
        placedObject.origin = origin;
        placedObject.anchoredPos = anchoredPos;
        placedObject.rectTransform = rectTransform;
        placedObject.RemoveMenuTile();
        placedObject.isPlacedOnMenu = false;
        placedObject.isPlacedOnInventory = isInventory;
        placedObject.SetupTooltip();
        return placedObject;
    }

    public static PlacedObject Create(Transform parent, MenuTile menuTile, Vector2 anchoredPos, Vector2Int origin, InventoryItemSO inventoryItemSO)
    {       
        Transform placedObjectTransform = Instantiate(inventoryItemSO.InventoryPrefab, parent);
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        RectTransform rectTransform = placedObjectTransform.GetComponent<RectTransform>();
        
        if (menuTile.GetType() == typeof(EquipMenuWeaponTile))
        {
            placedObjectTransform.localScale = new Vector3(0.8f, 0.8f);
            placedObjectTransform.rotation = Quaternion.identity;
            anchoredPos += new Vector2(15, 10);
            placedObject.SetMenuTile(menuTile as EquipMenuWeaponTile);
        }
        else if (menuTile.GetType() == typeof(EquipMenuSpellTile))
        {
            placedObjectTransform.localScale = new Vector3(0.35f, 0.35f);
            placedObjectTransform.rotation = Quaternion.identity;
            anchoredPos = new Vector2(7, 6);
            placedObject.SetMenuTile(menuTile as EquipMenuSpellTile);
        }
        else if (menuTile.GetType() == typeof(CraftMenuTile))
        {
            placedObjectTransform.localScale = new Vector3(0.5f, 0.5f);
            placedObjectTransform.rotation = Quaternion.identity;
            anchoredPos = new Vector2(2, 0);
            placedObject.SetMenuTile(menuTile as CraftMenuTile);
        }
        placedObject.tooltipTrigger = placedObject.GetComponent<TooltipTrigger>();
        placedObject.canvasGroup = placedObject.GetComponent<CanvasGroup>();
        placedObject.dragDrop = placedObject.GetComponent<InventoryDragDrop>();
        placedObject.inventorySystem = InventorySystem.Instance;
        placedObject.dragDropSystem = InventoryDragDropSystem.Instance;
        rectTransform.anchoredPosition = anchoredPos;        
        placedObject.inventoryItemSO = inventoryItemSO;
        placedObject.dir = InventoryItemSO.Dir.Down;
        placedObject.origin = origin;
        placedObject.anchoredPos = anchoredPos;
        placedObject.rectTransform = rectTransform;
        placedObject.isPlacedOnMenu = true;
        placedObject.isPlacedOnInventory = false;
        placedObject.SetupTooltip();
        return placedObject;
    }

    #endregion

}