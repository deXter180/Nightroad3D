using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryDragDropSystem : Singleton<InventoryDragDropSystem>
{
    private int XTransitionOffset = 8;
    private PlayerInputAsset inputs;
    private PlayerController playerController;
    private GameController gameController;
    private bool isDraggedFromInventory;
    private bool isDraggedFromStash;
    public float dragSmoothness = 80f;
    private InventoryItemSO.Dir dir;
    private PlacedObject draggingPOInventory;
    private PlacedObject draggingPOStash;
    private PlacedObject draggingPOWeaponMenu;
    private PlacedObject draggingPOSpellMenu;
    private EquipMenuWeaponTile draggedWeaponTile;
    private EquipMenuSpellTile draggedSpellTile;
    private InventorySystem inventorySystem;
    private ItemStash itemStash;
    private InventoryUIHandler inventoryUI;
    private Vector2Int mouseDragGridPosOffset;
    private Vector2 mouseDragGridPosOffsetMenu;
    private Vector2 mouseDragAnchoredPosOffsetInv;
    private Vector2 mouseDragAnchoredPosOffsetSth;
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
        itemStash = ItemStash.Instance;
        inventoryUI = InventoryUIHandler.Instance;
    }

    private void OnEnable()
    {        
        playerController = PlayerController.Instance;
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        isDraggedFromInventory = false;
        isDraggedFromStash = false;
        foreach (var tile in EquipMenuControl.WeaponTileList)
        {
            tile.OnPlacedOnWeaponMenu += Tile_OnObjectPlacedinEquipTile;
        }
    }

    private void OnDisable()
    {
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
            if (isDraggedFromInventory || isDraggedFromStash )
            {
                if (draggingPOInventory != null || draggingPOStash != null)
                {
                    if (inputs.UI.RotateItem.triggered)
                    {
                        dir = InventoryItemSO.GetNextDir(dir);
                    }
                }               
            }
            else
            {
                dir = InventoryItemSO.Dir.Down;
            }
            RemoveFromInventory();
            PositionDragObject();
            ConsumeItem();
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
        if (gameController.IsInventoryActive || gameController.IsStashActive)
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
            if (draggingPOStash != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(itemStash.GetItemContainer(), mousePos, UICam, out Vector2 targetPosition);
                targetPosition += new Vector2(-mouseDragAnchoredPosOffsetSth.x, -mouseDragAnchoredPosOffsetSth.y);
                Vector2Int rotationOffset = draggingPOStash.GetInventoryItemSO().GetRotationOffset(dir);
                targetPosition += new Vector2(rotationOffset.x, rotationOffset.y) * itemStash.GetGrid().GetCellSize();
                targetPosition /= 10f;
                targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
                targetPosition *= 10f;
                RectTransform rect = draggingPOStash.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPosition, Time.deltaTime * dragSmoothness);
                draggingPOStash.transform.rotation = Quaternion.Lerp(draggingPOStash.transform.rotation, Quaternion.Euler(0, 0, -draggingPOStash.GetInventoryItemSO().GetRotationAngle(dir)), Time.deltaTime * 15f);
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

    public void StartedDragging(PlacedObject placedObject, Vector2 mousePos, bool isInventory)
    {
        if (placedObject != null)
        {           
            Cursor.visible = false;
            if (isInventory)
            {
                draggingPOInventory = placedObject;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySystem.GetItemContainer(), mousePos, UICam, out Vector2 anchoredPosition))
                {
                    isDraggedFromInventory = true;
                    Vector2Int OriginOnInventory = inventorySystem.GetGridPos(anchoredPosition);
                    mouseDragGridPosOffset = Vector2Int.zero;
                    mouseDragGridPosOffset = OriginOnInventory - placedObject.GetGridPos();
                    mouseDragAnchoredPosOffsetInv = anchoredPosition - placedObject.GetComponent<RectTransform>().anchoredPosition;
                    dir = placedObject.GetDir();
                    Vector2Int rotationOffset = draggingPOInventory.GetInventoryItemSO().GetRotationOffset(dir);
                    mouseDragAnchoredPosOffsetInv += new Vector2(rotationOffset.x, rotationOffset.y) * inventorySystem.GetGrid().GetCellSize();
                }
            }
            else
            {
                draggingPOStash = placedObject;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(itemStash.GetItemContainer(), mousePos, UICam, out Vector2 anchoredPosition))
                {
                    isDraggedFromStash = true;
                    Vector2Int OriginOnInventory = itemStash.GetGridPos(anchoredPosition);
                    mouseDragGridPosOffset = Vector2Int.zero;
                    mouseDragGridPosOffset = OriginOnInventory - placedObject.GetGridPos();
                    mouseDragAnchoredPosOffsetSth = anchoredPosition - placedObject.GetComponent<RectTransform>().anchoredPosition;
                    dir = placedObject.GetDir();
                    Vector2Int rotationOffset = draggingPOStash.GetInventoryItemSO().GetRotationOffset(dir);
                    mouseDragAnchoredPosOffsetSth += new Vector2(rotationOffset.x, rotationOffset.y) * itemStash.GetGrid().GetCellSize();
                }
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
                isDraggedFromStash = true;
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
                isDraggedFromStash = true;
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
            draggingPOStash = null;
            draggingPOWeaponMenu = null;
            draggedWeaponTile = null;
            draggedSpellTile = null;
            bool tryPlaceItem = false;
            bool tryRemoveItem = false;
            Cursor.visible = true;
            Vector2Int Origin = new Vector2Int();    
            if (placedObject.IsPlacedOnMenu)
            {
                if (placedObject.GetWeaponEquipTile() != null)
                {
                    tryRemoveItem = placedObject.GetWeaponEquipTile().TryRemoveItem();
                }
                else if (placedObject.GetSpellEquipTile() != null)
                {
                    tryRemoveItem = placedObject.GetSpellEquipTile().TryRemoveItem();
                }
            }           
            else
            {
                if (placedObject.IsPlaceOnInventory)
                {
                    tryRemoveItem = inventorySystem.TryRemoveItemAt(placedObject.GetGridPos());
                }
                else
                {
                    tryRemoveItem = itemStash.TryRemoveItemAt(placedObject.GetGridPos());
                }               
            }
            if (tryRemoveItem)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySystem.GetItemContainer(), mousePos, UICam, out Vector2 anchorPosMenu))
                {
                    Origin = inventorySystem.GetGridPos(anchorPosMenu);
                    Origin -= mouseDragGridPosOffset;
                    if (isDraggedFromStash)
                    {
                        Origin += new Vector2Int(XTransitionOffset, 0);
                    }
                    if (inventorySystem.IsValidGridPos(Origin))
                    {
                        tryPlaceItem = inventorySystem.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO, Origin, dir, out PlacedObject PO);
                    }
                }
                if (itemStash.gameObject.activeSelf)
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(itemStash.GetItemContainer(), mousePos, UICam, out Vector2 anchorPos))
                    {
                        Origin = itemStash.GetGridPos(anchorPos);
                        Origin -= mouseDragGridPosOffset;
                        if (isDraggedFromInventory)
                        {
                            Origin += new Vector2Int(-XTransitionOffset, 0);
                        }
                        if (itemStash.IsValidGridPos(Origin))
                        {
                            tryPlaceItem = itemStash.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO, Origin, dir, out PlacedObject PO);
                        }
                    }
                }
                else
                {
                    if (IsOnWeaponMenu(mousePos, out EquipMenuWeaponTile weaponTile))
                    {
                        tryPlaceItem = weaponTile.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO);
                    }
                    if (IsOnSpellMenu(mousePos, out EquipMenuSpellTile spellTile))
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
                    if (!placedObject.IsPlacedOnMenu)
                    {
                        if (placedObject.IsPlaceOnInventory)
                        {
                            inventorySystem.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO, placedObject.GetGridPos(), placedObject.GetDir(), out PlacedObject PO);
                        }
                        else
                        {
                            itemStash.TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO, placedObject.GetGridPos(), placedObject.GetDir(), out PlacedObject PObj);
                        }
                    }                   
                    else
                    {
                        if (placedObject.GetWeaponEquipTile() != null)
                        {
                            placedObject.GetWeaponEquipTile().TryPlaceItem(placedObject.GetInventoryItemSO() as InventoryItemSO);
                        }
                    }                   
                }
            }
            isDraggedFromInventory = false;
            isDraggedFromStash = false;
        }        
    }

    private void RemoveFromInventory()
    {
        if (gameController.IsInventoryActive || gameController.IsStashActive)
        {
            if (draggingPOInventory == null)
            {
                if (inputs.UI.RemoveItem.triggered)
                {
                    Vector2 mousePos = inputs.BasicControls.MousePosition.ReadValue<Vector2>();
                    if (IsOnInventory(mousePos))
                    {
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySystem.GetItemContainer(), mousePos, UICam, out Vector2 anchoredPosition);
                        Vector2Int placedObjectOrigin = inventorySystem.GetGridLocalPos(anchoredPosition);
                        if (inventorySystem.GetGrid().GetGridObject(placedObjectOrigin.x, placedObjectOrigin.y).GetPlacedObject() != null)
                        {
                            PlacedObject placedObject = inventorySystem.GetGrid().GetGridObject(placedObjectOrigin.x, placedObjectOrigin.y).GetPlacedObject();
                            InventoryItemSO item = placedObject.GetInventoryItemSO();
                            ItemTypes IT = item.ItemType;
                            WeaponTypes WT = item.WeaponType;
                            SpellTypes ST = item.SpellType;
                            ArmorTypes AT = item.ArmorType;
                            placedObject.DestroySelf();
                            TooltipSystem.Hide();
                            inventorySystem.RemoveFromInventoryList(placedObject);
                            List<Vector2Int> gridPosList = placedObject.GetGridPosList();
                            foreach (Vector2Int gridPos in gridPosList)
                            {
                                inventorySystem.GetGrid().GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
                            }
                            if (IT == ItemTypes.Spell)
                            {
                                if (ST != SpellTypes.None)
                                {
                                    PickedObject pickedObject = PickedObject.SpawnSpellWorld(ST, item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f));
                                }
                            }
                            else if (IT == ItemTypes.Weapon)
                            {
                                if (WT != WeaponTypes.None)
                                {
                                    PickedObject pickedObject = PickedObject.SpawnWeaponWorld(WT, item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f));
                                }
                            }
                            else if (IT == ItemTypes.Armor)
                            {
                                if (AT != ArmorTypes.None)
                                {
                                    PickedObject pickedObject = PickedObject.SpawnArmorWorld(AT, item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f));
                                }
                            }
                            else
                            {
                                if (WT != WeaponTypes.None)
                                {
                                    PickedObject pickedObject = PickedObject.SpawnItemsWorld(placedObject.GetItemType(), item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f), WT);
                                }
                                else
                                {
                                    PickedObject pickedObject = PickedObject.SpawnItemsWorld(placedObject.GetItemType(), item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f));
                                }

                            }
                        }
                    }
                }
            }            
        }           
    }

    private void ConsumeItem()
    {
        if (gameController.IsInventoryActive)
        {            
            if (inputs.UI.ConsumeItem.triggered)
            {
                Vector2 mousePos = inputs.BasicControls.MousePosition.ReadValue<Vector2>();
                if (IsOnInventory(mousePos))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySystem.GetItemContainer(), mousePos, UICam, out Vector2 anchoredPosition);
                    Vector2Int placedObjectOrigin = inventorySystem.GetGridLocalPos(anchoredPosition);
                    if (inventorySystem.GetGrid().GetGridObject(placedObjectOrigin.x, placedObjectOrigin.y).GetPlacedObject() != null)
                    {
                        PlacedObject placedObject = inventorySystem.GetGrid().GetGridObject(placedObjectOrigin.x, placedObjectOrigin.y).GetPlacedObject();
                        InventoryItemSO item = placedObject.GetInventoryItemSO();
                        if (item.Usable)
                        {
                            if (item.ItemType == ItemTypes.HealthPotion)
                            {
                                if (playerController.PlayerTarget.Resource.HealthGain(item.AttributeAmount))
                                {
                                    placedObject.DestroySelf();
                                    inventorySystem.ConsumeFromInventory(placedObject);
                                    List<Vector2Int> gridPosList = placedObject.GetGridPosList();
                                    foreach (Vector2Int gridPos in gridPosList)
                                    {
                                        inventorySystem.GetGrid().GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
                                    }
                                }
                            }
                            if (item.ItemType == ItemTypes.ManaPotion)
                            {
                                if (playerController.PlayerTarget.Resource.ManaGain(item.AttributeAmount))
                                {
                                    placedObject.DestroySelf();
                                    inventorySystem.ConsumeFromInventory(placedObject);
                                    List<Vector2Int> gridPosList = placedObject.GetGridPosList();
                                    foreach (Vector2Int gridPos in gridPosList)
                                    {
                                        inventorySystem.GetGrid().GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
                                    }
                                }
                            }
                        }
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
        return false;
    }

    public bool IsOnStash(Vector2 currentPosition)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(itemStash.GetItemContainer(), currentPosition, UICam))
        {
            return true;
        }
        return false;
    }

    //~~~~~~~~~~~~~~~~~~~~ Event Callback ~~~~~~~~~~~~~~~~~~~~

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
