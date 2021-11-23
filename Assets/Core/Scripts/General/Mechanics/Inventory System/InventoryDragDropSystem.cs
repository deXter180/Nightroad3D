using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryDragDropSystem : MonoBehaviour
{
    public static InventoryDragDropSystem Instance { get; private set; }
    private Input input;
    private bool isDraggedFromInventory;
    private bool isDraggedFromMenu;
    public float dragSmoothness = 80f;
    private InventoryItemSO.Dir dir;
    private PlacedObject draggingPOInventory;
    private PlacedObject draggingPOMenu;
    private EquipMenuTile draggedMenuTile;
    private InventorySystem inventorySystem;
    private InventoryUIHandler inventoryUI;
    private Vector2Int mouseDragGridPosOffsetInventory;
    private Vector2 mouseDragGridPosOffsetMenu;
    private Vector2 mouseDragAnchoredPosOffsetInv;
    [SerializeField] private Camera UICam;


    private void Awake()
    {
        Instance = this;
        input = FindObjectOfType<InputControl>();
    }

    private void Start()
    {
        inventorySystem = InventorySystem.Instance;
        inventoryUI = InventoryUIHandler.Instance;
        inventorySystem.OnPlacedOnInventory += InventorySystem_OnObjectPlaced;
        isDraggedFromInventory = false;
        isDraggedFromMenu = false;
        foreach(var tile in EquipMenuControl.EquipTileList)
        {
            tile.OnPlacedOnMenu += Tile_OnObjectPlacedinEquipTile;
        }
    }

    private void OnDestroy()
    {
        inventorySystem.OnPlacedOnInventory -= InventorySystem_OnObjectPlaced;
        foreach (var tile in EquipMenuControl.EquipTileList)
        {
            tile.OnPlacedOnMenu -= Tile_OnObjectPlacedinEquipTile;
        }
    }

    private void Update()
    {
        if (input.GetRotationItems())
        {
            if (isDraggedFromInventory)
            {
                dir = InventoryItemSO.GetNextDir(dir);
            }
            else
                dir = InventoryItemSO.Dir.Down;
            
        }        
        RemoveFromInventory();
        PositionDragObject();
    }

    private void PositionDragObject()
    {
        if (InventoryUIHandler.Instance.IsInventoryON)
        {
            if (draggingPOInventory != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySystem.GetItemContainer(), input.GetMousePosition(), UICam, out Vector2 targetPosition);
                targetPosition += new Vector2(-mouseDragAnchoredPosOffsetInv.x, -mouseDragAnchoredPosOffsetInv.y);
                Vector2Int rotationOffset = draggingPOInventory.GetInventoryItemSO().GetRotationOffset(dir);
                targetPosition += new Vector2(rotationOffset.x, rotationOffset.y) * inventorySystem.GetGrid().GetCellSize();
                targetPosition /= 10f;
                targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
                targetPosition *= 10f;
                RectTransform rect = draggingPOInventory.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPosition, Time.deltaTime * dragSmoothness);
                draggingPOInventory.transform.rotation = Quaternion.Lerp(draggingPOInventory.transform.rotation, Quaternion.Euler(0, 0, -draggingPOInventory.GetInventoryItemSO().GetRotationAngle(dir)), Time.deltaTime * 15f);
            }
            if(draggingPOMenu != null && draggedMenuTile != null)
            {               
                RectTransformUtility.ScreenPointToLocalPointInRectangle(draggedMenuTile.GetRectTransform(), input.GetMousePosition(), UICam, out Vector2 targetPosition);
                targetPosition += new Vector2(-mouseDragGridPosOffsetMenu.x, -mouseDragGridPosOffsetMenu.y);
                targetPosition /= 10f;
                targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
                targetPosition *= 10f;
                RectTransform rect = draggingPOMenu.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPosition, Time.deltaTime * dragSmoothness);
            }
        }              
    }

    public void StartedDragging(PlacedObject placedObject, Vector2 mousePos)
    {
        if (placedObject != null)
        {
            draggingPOInventory = placedObject;
            Cursor.visible = false;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySystem.GetItemContainer(), mousePos, UICam, out Vector2 anchoredPosition))
            {
                isDraggedFromInventory = true;
                Vector2Int OriginOnInventory = inventorySystem.GetGridPos(anchoredPosition);
                mouseDragGridPosOffsetInventory = Vector2Int.zero;
                mouseDragGridPosOffsetInventory = OriginOnInventory - placedObject.GetGridPos();
                mouseDragAnchoredPosOffsetInv = anchoredPosition - placedObject.GetComponent<RectTransform>().anchoredPosition;
                dir = placedObject.GetDir();
                Vector2Int rotationOffset = draggingPOInventory.GetInventoryItemSO().GetRotationOffset(dir);
                mouseDragAnchoredPosOffsetInv += new Vector2(rotationOffset.x, rotationOffset.y) * inventorySystem.GetGrid().GetCellSize();
            }                     
        }         
    }

    public void StartedDragging(PlacedObject placedObject, EquipMenuTile menuTile, Vector2 mousePos)
    {
        if (placedObject != null && menuTile != null)
        {           
            draggingPOMenu = placedObject;
            draggedMenuTile = menuTile;
            Cursor.visible = false;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(menuTile.GetRectTransform(), mousePos, UICam, out Vector2 anchoredPosition))
            {
                isDraggedFromMenu = true;
                Vector2Int mouseGridPos = menuTile.GetGridPos(anchoredPosition);
                mouseDragGridPosOffsetMenu = Vector2.zero;
                mouseDragGridPosOffsetMenu = anchoredPosition - placedObject.GetComponent<RectTransform>().anchoredPosition;
            }
        }
    }

    public void StoppedDragging(PlacedObject placedObject, Vector2 mousePos)
    {
        if (placedObject != null)
        {
            draggingPOInventory = null;
            draggingPOMenu = null;
            draggedMenuTile = null;
            isDraggedFromInventory = false;
            isDraggedFromMenu = false;
            bool tryPlaceItem = false;
            Cursor.visible = true;
            Vector2Int OriginOnInventory = new Vector2Int();                                
            if (placedObject.GetEquipTile() != null)
            {
                placedObject.GetEquipTile().TryRemoveItem();
            }
            else
            {
                inventorySystem.TryRemoveItemAt(placedObject.GetGridPos());
            }
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySystem.GetItemContainer(), mousePos, UICam, out Vector2 anchorPosMenu))
            {
                OriginOnInventory = inventorySystem.GetGridPos(anchorPosMenu);
                OriginOnInventory -= mouseDragGridPosOffsetInventory;
                if (inventorySystem.IsValidGridPos(OriginOnInventory))
                {
                    tryPlaceItem = inventorySystem.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO, OriginOnInventory, dir);
                }
                else if (IsOnMenu(mousePos, out EquipMenuTile menuTile))
                {
                    tryPlaceItem = menuTile.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO);
                }
            }            
            if (tryPlaceItem)
            {
                Debug.Log("Item is Placed");
            }
            else
            {
                inventorySystem.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO, placedObject.GetGridPos(), placedObject.GetDir());
                if (placedObject.GetEquipTile() != null)
                {
                    placedObject.GetEquipTile().TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO);
                }
            }
        }        
    }

    private void RemoveFromInventory()
    {
        if (InventoryUIHandler.Instance.IsInventoryON)
        {
            Vector2 mousePos = input.GetMousePosition();
            if (input.GetMouseRightClick() && IsOnInventory(mousePos))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySystem.GetItemContainer(), mousePos, UICam, out Vector2 anchoredPosition);
                Vector2Int placedObjectOrigin = inventorySystem.GetGridLocalPos(anchoredPosition);
                if (inventorySystem.GetGrid().GetGridObject(placedObjectOrigin.x, placedObjectOrigin.y).GetPlacedObject() != null)
                {
                    PlacedObject placedObject = inventorySystem.GetGrid().GetGridObject(placedObjectOrigin.x, placedObjectOrigin.y).GetPlacedObject();
                    WeaponTypes WT = placedObject.GetInventoryItemSO().weaponType;
                    placedObject.DestroySelf();
                    inventorySystem.RemoveFromInventoryList(placedObject);
                    List<Vector2Int> gridPosList = placedObject.GetGridPosList();
                    foreach (Vector2Int gridPos in gridPosList)
                    {
                        inventorySystem.GetGrid().GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
                    }
                    if (WT == WeaponTypes.None)
                    {
                        PickedObject pickedObject = PickedObject.SpawnItemsWorld(placedObject.GetItemType(), placedObject.GetInventoryItemSO(),PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(15f, -15f));
                    }
                    else
                    {
                        PickedObject pickedObject = PickedObject.SpawnWeaponWorld(WT, placedObject.GetInventoryItemSO(), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(15f, -15f));
                    }
                    
                }
            }
        }           
    }

    public Vector2 GetPosOffset(Vector2 mousePos)
    {
        Vector2 mouseOffset = new Vector2();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryUI.GetCanvasTransfrom(), mousePos, UICam, out Vector2 rectPos))
        {
            mouseOffset = rectPos;
        }
        return mouseOffset;
    }

    public bool IsOnMenu(Vector2 currentPosition, out EquipMenuTile menuTile)
    {
        foreach (var menu in EquipMenuControl.EquipTileList)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(menu.GetRectTransform(), currentPosition, UICam))
            {
                menuTile = menu;
                return true;
            }
        }
        menuTile = null;
        return false;
    }

    public bool IsOnMenu(Vector2 currentPosition)
    {
        foreach (var menu in EquipMenuControl.EquipTileList)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(menu.GetRectTransform(), currentPosition, UICam))
            {
                return true;
            }
        }
        
        return false;
    }

    public bool IsOnInventory(Vector2 currentPosition)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(inventorySystem.GetItemContainer(), currentPosition, UICam))
        {
            return true;
        }
        else return false;
    }

    //~~~~~~~~~~~~~~~~~~~~ Event Callback ~~~~~~~~~~~~~~~~~~~~

    private void InventorySystem_OnObjectPlaced(object sender, PlacedObject e)
    {
        
    }

    private void Tile_OnObjectPlacedinEquipTile(object sender, PlacedObject e)
    {
        
    }
}
