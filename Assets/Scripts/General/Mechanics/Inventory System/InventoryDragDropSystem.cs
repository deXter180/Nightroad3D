using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryDragDropSystem : MonoBehaviour
{
    public static InventoryDragDropSystem Instance { get; private set; }
    private Input input;
    private InventoryItemSO.Dir dir;
    private PlacedObject draggingPlacedObject;
    private Vector2Int mouseDragGridPosOffset;
    private Vector2 mouseDragAnchoredPosOffset;
    private RectTransform ownTransform;
    [SerializeField] private Camera UICam;
    public int PlacedObjectCount { get; private set; }

    private void Awake()
    {
        Instance = this;
        input = FindObjectOfType<InputControl>();
        ownTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        InventorySystem.OnObjectPlaced += InventorySystem_OnObjectPlaced;
    }

    private void Update()
    {
        if (input.GetRotationItems())
        {
            dir = InventoryItemSO.GetNextDir(dir);
        }
        PositionDragObject();
        RemoveFromInventory();
    }

    private void PositionDragObject()
    {
        if (InventoryUIHandler.Instance.IsInventoryON)
        {
            if (draggingPlacedObject != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(InventorySystem.Instance.GetItemContainer(), input.GetMousePosition(), UICam, out Vector2 targetPosition);
                Vector2Int currentGridPos = InventorySystem.Instance.GetGridPos(targetPosition);
                targetPosition += new Vector2(-mouseDragAnchoredPosOffset.x, -mouseDragAnchoredPosOffset.y);
                Vector2Int rotationOffset = draggingPlacedObject.GetInventoryItemSO().GetRotationOffset(dir);
                targetPosition += new Vector2(rotationOffset.x, rotationOffset.y) * InventorySystem.Instance.GetGrid().GetCellSize();
                targetPosition /= 10f;
                targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
                targetPosition *= 10f;
                draggingPlacedObject.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(draggingPlacedObject.GetComponent<RectTransform>().anchoredPosition, targetPosition, Time.deltaTime * 20f);
                draggingPlacedObject.transform.rotation = Quaternion.Lerp(draggingPlacedObject.transform.rotation, Quaternion.Euler(0, 0, -draggingPlacedObject.GetInventoryItemSO().GetRotationAngle(dir)), Time.deltaTime * 15f);
            }
        }              
    }

    public void StartedDragging(PlacedObject placedObject)
    {
        if (placedObject != null)
        {
            draggingPlacedObject = placedObject;
            Cursor.visible = false;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(InventorySystem.Instance.GetItemContainer(), input.GetMousePosition(), UICam, out Vector2 anchoredPosition);
            Vector2Int mouseGridPos = InventorySystem.Instance.GetGridPos(anchoredPosition);
            mouseDragGridPosOffset = mouseGridPos - placedObject.GetGridPos();
            mouseDragAnchoredPosOffset = anchoredPosition - placedObject.GetComponent<RectTransform>().anchoredPosition;
            dir = placedObject.GetDir();
            Vector2Int rotationOffset = draggingPlacedObject.GetInventoryItemSO().GetRotationOffset(dir);
            mouseDragAnchoredPosOffset += new Vector2(rotationOffset.x, rotationOffset.y) * InventorySystem.Instance.GetGrid().GetCellSize();
        }         
    }

    public void StoppedDragging(PlacedObject placedObject)
    {
        if (placedObject != null)
        {
            draggingPlacedObject = placedObject;
            Cursor.visible = true;
            InventorySystem.Instance.TryRemoveItemAt(placedObject.GetGridPos());
            RectTransformUtility.ScreenPointToLocalPointInRectangle(InventorySystem.Instance.GetItemContainer(), input.GetMousePosition(), UICam, out Vector2 anchoredPosition);
            Vector2Int placedObjectOrigin = InventorySystem.Instance.GetGridPos(anchoredPosition);
            placedObjectOrigin = placedObjectOrigin - mouseDragGridPosOffset;
            bool tryPlaceItem = InventorySystem.Instance.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO, placedObjectOrigin, dir);
            if (tryPlaceItem)
            {
                Debug.Log("Item is Placed");
            }
            else
            {
                //TooltipCanvas.ShowTooltip_Static("Cannot Drop Item Here!");
                InventorySystem.Instance.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO, placedObject.GetGridPos(), placedObject.GetDir());
            }
        }        
    }

    private void RemoveFromInventory()
    {   
        if (InventoryUIHandler.Instance.IsInventoryON)
        {
            if (input.GetMouseRightClick())
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(InventorySystem.Instance.GetItemContainer(), input.GetMousePosition(), UICam, out Vector2 anchoredPosition);
                Vector2Int placedObjectOrigin = InventorySystem.Instance.GetGridLocalPos(anchoredPosition);
                if (InventorySystem.Instance.GetGrid().GetGridObject(placedObjectOrigin.x, placedObjectOrigin.y).GetPlacedObject() != null)
                {
                    PlacedObject placedObject = InventorySystem.Instance.GetGrid().GetGridObject(placedObjectOrigin.x, placedObjectOrigin.y).GetPlacedObject();
                    placedObject.DestroySelf();
                    InventorySystem.Instance.RemoveFromInventoryList(placedObject);
                    List<Vector2Int> gridPosList = placedObject.GetGridPosList();
                    foreach (Vector2Int gridPos in gridPosList)
                    {
                        InventorySystem.Instance.GetGrid().GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
                    }
                    PickedObject pickedObject = PickedObject.SpawnItemWorld(placedObject.GetItemType(), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(15f, -15f));
                }
                
            }
        }               
    }

    //~~~~~~~~~~~~~~~~~~~~ Event Callback ~~~~~~~~~~~~~~~~~~~~

    private void InventorySystem_OnObjectPlaced(object sender, PlacedObject e)
    {
        Debug.Log(e.GetInventoryItemSO().itemType);
        PlacedObjectCount = transform.Find("ItemContainer").childCount;
    }
}
