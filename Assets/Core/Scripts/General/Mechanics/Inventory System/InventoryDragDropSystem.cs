using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryDragDropSystem : Singleton<InventoryDragDropSystem>
{
    private PlayerInputAsset inputs;
    private PlayerController playerController;
    private GameController gameController;
    private bool isDraggedFromInventory;
    private bool isDraggedFromMenu;
    public float dragSmoothness = 80f;
    private InventoryItemSO.Dir dir;
    private PlacedObject draggingPOInventory;
    private PlacedObject draggingPOWeaponMenu;
    private PlacedObject draggingPOSpellMenu;
    private EquipMenuWeaponTile draggedWeaponTile;
    private EquipMenuSpellTile draggedSpellTile;
    private InventorySystem inventorySystem;
    private InventoryUIHandler inventoryUI;
    private Vector2Int mouseDragGridPosOffsetInventory;
    private Vector2 mouseDragGridPosOffsetMenu;
    private Vector2 mouseDragAnchoredPosOffsetInv;
    [SerializeField] private Camera UICam;


    protected override void Awake()
    {
        base.Awake();     
    }

    private void Start()
    {
        StartCoroutine(InputDone());
        gameController = GameController.Instance;
        inventorySystem = InventorySystem.Instance;
        inventoryUI = InventoryUIHandler.Instance;
    }

    private void OnEnable()
    {        
        playerController = PlayerController.Instance;
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        InventorySystem.OnPlacedOnInventory += InventorySystem_OnObjectPlaced;
        isDraggedFromInventory = false;
        isDraggedFromMenu = false;
        foreach (var tile in EquipMenuControl.WeaponTileList)
        {
            tile.OnPlacedOnWeaponMenu += Tile_OnObjectPlacedinEquipTile;
        }
        isDraggedFromInventory = false;
        isDraggedFromMenu = false;
    }

    private void OnDisable()
    {
        InventorySystem.OnPlacedOnInventory -= InventorySystem_OnObjectPlaced;
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        foreach (var tile in EquipMenuControl.WeaponTileList)
        {
            tile.OnPlacedOnWeaponMenu -= Tile_OnObjectPlacedinEquipTile;
        }
    }

    private void Update()
    {
        if (inputs != null)
        {
            if (inputs.UI.RotateInventoryItem.triggered)
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
    }

    private IEnumerator InputDone()
    {
        if (inputs == null)
        {
            yield return new WaitUntil(() => InputManager.InputReady);
            inputs = InputManager.InputActions;
        }       
    }

    private void PositionDragObject()
    {
        if (gameController.IsInventoryActive)
        {
            Vector2 mousePos = inputs.BasicControls.MousePosition.ReadValue<Vector2>();
            if (draggingPOInventory != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySystem.GetItemContainer(), mousePos, UICam, out Vector2 targetPosition);
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
            if(draggingPOWeaponMenu != null && draggedWeaponTile != null)
            {               
                RectTransformUtility.ScreenPointToLocalPointInRectangle(draggedWeaponTile.GetRectTransform(), mousePos, UICam, out Vector2 targetPosition);
                targetPosition += new Vector2(-mouseDragGridPosOffsetMenu.x, -mouseDragGridPosOffsetMenu.y);
                targetPosition /= 10f;
                targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
                targetPosition *= 10f;
                RectTransform rect = draggingPOWeaponMenu.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPosition, Time.deltaTime * dragSmoothness);
            }
            if(draggingPOSpellMenu != null && draggedSpellTile != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(draggedSpellTile.GetRectTransform(), mousePos, UICam, out Vector2 targetPosition);
                targetPosition += new Vector2(-mouseDragGridPosOffsetMenu.x, -mouseDragGridPosOffsetMenu.y);
                targetPosition /= 10f;
                targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
                targetPosition *= 10f;
                RectTransform rect = draggingPOSpellMenu.GetComponent<RectTransform>();
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

    public void StartedDragging(PlacedObject placedObject, EquipMenuWeaponTile menuTile, Vector2 mousePos)
    {
        if (placedObject != null && menuTile != null)
        {           
            draggingPOWeaponMenu = placedObject;
            draggedWeaponTile = menuTile;
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

    public void StartedDragging(PlacedObject placedObject, EquipMenuSpellTile menuTile, Vector2 mousePos)
    {
        if (placedObject != null && menuTile != null)
        {
            draggingPOSpellMenu = placedObject;
            draggedSpellTile = menuTile;
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
            draggingPOWeaponMenu = null;
            draggedWeaponTile = null;
            draggedSpellTile = null;
            isDraggedFromInventory = false;
            isDraggedFromMenu = false;
            bool tryPlaceItem = false;
            Cursor.visible = true;
            Vector2Int OriginOnInventory = new Vector2Int();                                
            if (placedObject.GetWeaponEquipTile() != null)
            {
                placedObject.GetWeaponEquipTile().TryRemoveItem();
            }
            else if (placedObject.GetSpellEquipTile() != null)
            {
                placedObject.GetSpellEquipTile().TryRemoveItem();
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
                else if (IsOnWeaponMenu(mousePos, out EquipMenuWeaponTile weaponTile))
                {
                    tryPlaceItem = weaponTile.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO);
                }
                else if (IsOnSpellMenu(mousePos, out EquipMenuSpellTile spellTile))
                {
                    tryPlaceItem = spellTile.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO);
                }
            }            
            if (tryPlaceItem)
            {
                Debug.Log("Item is Placed");
            }
            else
            {
                inventorySystem.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO, placedObject.GetGridPos(), placedObject.GetDir());
                if (placedObject.GetWeaponEquipTile() != null)
                {
                    placedObject.GetWeaponEquipTile().TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO);
                }
            }
        }        
    }

    private void RemoveFromInventory()
    {
        if (gameController.IsInventoryActive)
        {
            Vector2 mousePos = inputs.BasicControls.MousePosition.ReadValue<Vector2>();
            if (inputs.UI.RemoveInventoryItem.triggered && IsOnInventory(mousePos))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySystem.GetItemContainer(), mousePos, UICam, out Vector2 anchoredPosition);
                Vector2Int placedObjectOrigin = inventorySystem.GetGridLocalPos(anchoredPosition);
                if (inventorySystem.GetGrid().GetGridObject(placedObjectOrigin.x, placedObjectOrigin.y).GetPlacedObject() != null)
                {
                    PlacedObject placedObject = inventorySystem.GetGrid().GetGridObject(placedObjectOrigin.x, placedObjectOrigin.y).GetPlacedObject();
                    InventoryItemSO item = placedObject.GetInventoryItemSO();
                    WeaponTypes WT = item.WeaponType;
                    SpellTypes ST = item.SpellType;
                    placedObject.DestroySelf();
                    inventorySystem.RemoveFromInventoryList(placedObject);
                    List<Vector2Int> gridPosList = placedObject.GetGridPosList();
                    foreach (Vector2Int gridPos in gridPosList)
                    {
                        inventorySystem.GetGrid().GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
                    }
                    if (WT == WeaponTypes.None && ST == SpellTypes.None)
                    {
                        PickedObject pickedObject = PickedObject.SpawnItemsWorld(placedObject.GetItemType(), item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f));
                    }
                    else if (WT != WeaponTypes.None && ST == SpellTypes.None)
                    {
                        PickedObject pickedObject = PickedObject.SpawnWeaponWorld(WT, item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f));
                    }
                    else if (ST != SpellTypes.None && WT == WeaponTypes.None)
                    {
                        PickedObject pickedObject = PickedObject.SpawnSpellWorld(ST, item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f));
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

    public bool IsOnWeaponMenu(Vector2 currentPosition, out EquipMenuWeaponTile menuTile)
    {
        foreach (var menu in EquipMenuControl.WeaponTileList)
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

    public bool IsOnSpellMenu(Vector2 currentPosition, out EquipMenuSpellTile menuTile)
    {
        foreach (var menu in EquipMenuControl.SpellTileList)
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
        foreach (var menu in EquipMenuControl.WeaponTileList)
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

    private void SceneLoader_OnNewGameStart()
    {
        StartCoroutine(SetPlayer());
        IEnumerator SetPlayer()
        {
            yield return Helpers.GetWait(1f);
            playerController = PlayerController.Instance;
        }
    }
}
