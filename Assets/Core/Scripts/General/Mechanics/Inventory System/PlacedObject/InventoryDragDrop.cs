using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(CanvasGroup))]
public class InventoryDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup canvasGroup;
    private PlacedObject placedObject;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        placedObject = GetComponent<PlacedObject>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (placedObject != null)
        {            
            transform.SetSiblingIndex(InventoryDragDropSystem.Instance.PlacedObjectCount - 1);
            canvasGroup.alpha = 0.7f;
            canvasGroup.blocksRaycasts = false;
            InventoryItemSO.CreateGridVisual(transform.GetChild(0), placedObject.GetInventoryItemSO(), InventorySystem.Instance.GetGrid().GetCellSize());
            InventoryDragDropSystem.Instance.StartedDragging(placedObject);
        }       
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (placedObject != null)
        {
            InventoryDragDropSystem.Instance.StoppedDragging(placedObject);
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
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
