using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(CanvasGroup))]
public class InventoryDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
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
            canvasGroup.alpha = 0.7f;
            canvasGroup.blocksRaycasts = false;
            InventoryItemSO.CreateVisualGrid(transform.GetChild(0), placedObject.GetInventoryItemSO() as InventoryItemSO, InventorySystem.Instance.GetGrid().GetCellSize());
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
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            InventoryDragDropSystem.Instance.StoppedDragging(placedObject);
        }      
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
