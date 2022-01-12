using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(CanvasGroup))]
public class InventoryDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup canvasGroup;
    private PlacedObject placedObject;
    private float cellSize;
    private InventoryDragDropSystem dragDropSystem;
    private EquipMenuControl equipMenu;
    private InventoryUIHandler inventoryUI;
    private InventorySystem inventorySystem;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        placedObject = GetComponent<PlacedObject>();
        cellSize = InventoryUIHandler.Instance.GetCellSize();
    }

    private void Start()
    {
        dragDropSystem = InventoryDragDropSystem.Instance;
        equipMenu = EquipMenuControl.Instance;
        inventoryUI = InventoryUIHandler.Instance;
        inventorySystem = InventorySystem.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (placedObject != null)
        {
            transform.SetAsLastSibling();
            canvasGroup.alpha = 0.7f;
            canvasGroup.blocksRaycasts = false;
            
            if (dragDropSystem.IsOnMenu(eventData.position, out EquipMenuTile menuTile) && placedObject.GetEquipTile() != null)
            {
                equipMenu.transform.SetAsLastSibling();
                InventoryItemSO.CreateGridVisual(transform.GetChild(0), placedObject.GetInventoryItemSO(), 200 , true);
                dragDropSystem.StartedDragging(placedObject, placedObject.GetEquipTile(), eventData.position);

            }
            else if (dragDropSystem.IsOnInventory(eventData.position))
            {
                inventorySystem.transform.SetAsLastSibling();
                InventoryItemSO.CreateGridVisual(transform.GetChild(0), placedObject.GetInventoryItemSO(), cellSize, false);
                dragDropSystem.StartedDragging(placedObject, eventData.position);
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
}