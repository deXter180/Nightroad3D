using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(CanvasGroup))]
public class InventoryDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Variables

    private CanvasGroup canvasGroup;
    private PlacedObject placedObject;
    private float cellSize;
    private InventoryDragDropSystem dragDropSystem;
    private EquipMenuControl equipMenu;
    private InventorySystem inventorySystem;
    private ItemStash itemStash;
    private GameController gameController;

    #endregion

    #region General

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        placedObject = GetComponent<PlacedObject>();
        cellSize = InventoryUIHandler.Instance.GetCellSize();
    }

    private void OnEnable()
    {
        gameController = GameController.Instance;
        dragDropSystem = InventoryDragDropSystem.Instance;
        equipMenu = EquipMenuControl.Instance;
        inventorySystem = InventorySystem.Instance;
        itemStash = ItemStash.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (placedObject != null)
        {
            transform.SetAsLastSibling();
            canvasGroup.alpha = 0.7f;
            canvasGroup.blocksRaycasts = false;
            if (gameController.IsInventoryActive)
            {
                if (dragDropSystem.IsOnWeaponMenu(eventData.position, out EquipMenuWeaponTile weaponTile) && placedObject.GetWeaponEquipTile() != null)
                {
                    equipMenu.transform.SetAsLastSibling();
                    InventoryItemSO.CreateGridVisual(transform.GetChild(0), placedObject.GetInventoryItemSO(), 200, InventoryItemSO.TileTypes.WeaponTile);
                    dragDropSystem.StartedDragging(placedObject, placedObject.GetWeaponEquipTile(), eventData.position);

                }
                else if (dragDropSystem.IsOnSpellMenu(eventData.position, out EquipMenuSpellTile spellTile) && placedObject.GetSpellEquipTile() != null)
                {
                    equipMenu.transform.SetAsLastSibling();
                    InventoryItemSO.CreateGridVisual(transform.GetChild(0), placedObject.GetInventoryItemSO(), 50, InventoryItemSO.TileTypes.SpellTile);
                    dragDropSystem.StartedDragging(placedObject, placedObject.GetSpellEquipTile(), eventData.position);
                }
            }
            else if (gameController.IsStashActive)
            {
                if (dragDropSystem.IsOnStash(eventData.position))
                {
                    itemStash.transform.SetAsLastSibling();
                    InventoryItemSO.CreateGridVisual(transform.GetChild(0), placedObject.GetInventoryItemSO(), cellSize, InventoryItemSO.TileTypes.InventoryTile);
                    dragDropSystem.StartedDragging(placedObject, eventData.position, false);
                }
            }            
            if (dragDropSystem.IsOnInventory(eventData.position))
            {
                inventorySystem.transform.SetAsLastSibling();
                InventoryItemSO.CreateGridVisual(transform.GetChild(0), placedObject.GetInventoryItemSO(), cellSize, InventoryItemSO.TileTypes.InventoryTile);
                dragDropSystem.StartedDragging(placedObject, eventData.position, true);
            }            
        }       
    }

    public void OnDrag(PointerEventData eventData)
    {
     
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (placedObject != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            dragDropSystem.StoppedDragging(placedObject, eventData.position);     
        }      
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    #endregion
}
