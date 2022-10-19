using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryDragDropSystem : Singleton<InventoryDragDropSystem>
{
    #region Variables

    private int XTransitionOffset = 8;
    private PlayerInputAsset inputs;
    private PlayerController playerController;
    private GameController gameController;
    private AttributeManager attributeManager;
    private bool isDraggedFromInventory;
    private bool isDraggedFromEquipMenu;
    private bool isDraggedFromCrafting;
    private bool isDraggedFromStash;
    private bool isDragging;
    public float dragSmoothness = 80f;
    private InventoryItemSO.Dir dir;
    private PlacedObject draggingPOInventory;
    private PlacedObject draggingPOStash;
    private PlacedObject draggingPOWeaponMenu;
    private PlacedObject draggingPOSpellMenu;
    private PlacedObject draggingPOCraft;
    private EquipMenuWeaponTile draggedWeaponTile;
    private EquipMenuSpellTile draggedSpellTile;
    private CraftMenuTile draggedCraftTile;
    private CraftMenuControl craftMenu;
    private EquipMenuControl equipMenu;
    private InventorySystem inventorySystem;
    private ItemStash itemStash;
    private InventoryUIHandler inventoryUI;
    private Vector2Int mouseDragGridPosOffset;
    private Vector2Int mouseStashDragGridPosOffset;
    private Vector2 mouseDragGridPosOffsetMenu;
    private Vector2 mouseDragAnchoredPosOffsetInv;
    private Vector2 mouseDragAnchoredPosOffsetSth;
    public bool IsDragging => isDragging;
    private MenuTile[] tileList;
    [SerializeField] private Camera UICam;

    #endregion

    #region GeneralFunctions

    protected override void Awake()
    {
        base.Awake();
        tileList = GetComponentsInChildren<MenuTile>();
    }

    private void Start()
    {
        StartCoroutine(InputDone());
        gameController = GameController.Instance;
        playerController = PlayerController.Instance;
        inventorySystem = InventorySystem.Instance;
        itemStash = ItemStash.Instance;
        inventoryUI = InventoryUIHandler.Instance;
        attributeManager = AttributeManager.Instance;
        craftMenu = CraftMenuControl.Instance;
        equipMenu = EquipMenuControl.Instance;
    }

    private void OnEnable()
    {                
        isDraggedFromInventory = false;
        isDraggedFromEquipMenu = false;
        isDraggedFromStash = false;
        isDragging = false;
        foreach (var tile in EquipMenuControl.WeaponTileList)
        {
            tile.OnPlacedOnWeaponMenu += Tile_OnObjectPlacedinEquipTile;
        }
    }

    private void OnDisable()
    {
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
            SetTransformOrder();
        }
    }

    private void SetTransformOrder()
    {
        if (isDraggedFromInventory)
        {
            inventorySystem.transform.SetAsLastSibling();
        }
        else if(isDraggedFromStash)
        {
            itemStash.transform.SetAsLastSibling();
        }
        else if (isDraggedFromEquipMenu)
        {
            equipMenu.transform.SetAsLastSibling();
        }
        else if (isDraggedFromCrafting)
        {
            craftMenu.transform.SetAsLastSibling();
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

    #endregion

    #region MechanicsFunctions

    private void PositionDragObject()
    {
        if (gameController.IsInventoryActive || gameController.IsStashActive || gameController.IsCraftingActive)
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
            if (draggingPOWeaponMenu != null && draggedWeaponTile != null)
            {               
                RectTransformUtility.ScreenPointToLocalPointInRectangle(draggedWeaponTile.GetRectTransform(), mousePos, UICam, out Vector2 targetPosition);
                targetPosition += new Vector2(-mouseDragGridPosOffsetMenu.x, -mouseDragGridPosOffsetMenu.y);
                targetPosition /= 10f;
                targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
                targetPosition *= 10f;
                RectTransform rect = draggingPOWeaponMenu.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPosition, Time.deltaTime * dragSmoothness);
            }
            if (draggingPOSpellMenu != null && draggedSpellTile != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(draggedSpellTile.GetRectTransform(), mousePos, UICam, out Vector2 targetPosition);
                targetPosition += new Vector2(-mouseDragGridPosOffsetMenu.x, -mouseDragGridPosOffsetMenu.y);
                targetPosition /= 10f;
                targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
                targetPosition *= 10f;
                RectTransform rect = draggingPOSpellMenu.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPosition, Time.deltaTime * dragSmoothness);
            }
            if (draggedCraftTile != null && draggingPOCraft != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(draggedCraftTile.GetRectTransform(), mousePos, UICam, out Vector2 targetPosition);
                targetPosition += new Vector2(-mouseDragGridPosOffsetMenu.x, -mouseDragGridPosOffsetMenu.y);
                targetPosition /= 10f;
                targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
                targetPosition *= 10f;
                RectTransform rect = draggingPOCraft.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPosition, Time.deltaTime * dragSmoothness);
            }
        }              
    }

    public void StartedDragging(PlacedObject placedObject, Vector2 mousePos, bool isInventory)
    {
        if (placedObject != null)
        {           
            Cursor.visible = false;
            isDragging = true;
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
                    mouseStashDragGridPosOffset = Vector2Int.zero;
                    mouseStashDragGridPosOffset = OriginOnInventory - placedObject.GetGridPos();
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
            isDragging = true;
            draggingPOWeaponMenu = placedObject;
            draggedWeaponTile = menuTile;
            Cursor.visible = false;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(menuTile.GetRectTransform(), mousePos, UICam, out Vector2 anchoredPosition))
            {
                isDraggedFromEquipMenu = true;
                MenuDragging(placedObject, mousePos, anchoredPosition);
            }
        }
    }

    public void StartedDragging(PlacedObject placedObject, EquipMenuSpellTile menuTile, Vector2 mousePos)
    {
        if (placedObject != null && menuTile != null)
        {
            isDragging = true;
            draggingPOSpellMenu = placedObject;
            draggedSpellTile = menuTile;
            Cursor.visible = false;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(menuTile.GetRectTransform(), mousePos, UICam, out Vector2 anchoredPosition))
            {
                isDraggedFromEquipMenu = true;
                MenuDragging(placedObject, mousePos, anchoredPosition);
            }
        }
    }

    public void StartedDragging(PlacedObject placedObject, CraftMenuTile menuTile, Vector2 mousePos)
    {
        if (placedObject != null && menuTile != null)
        {
            isDragging = true;
            draggingPOCraft = placedObject;
            draggedCraftTile = menuTile;
            Cursor.visible = false;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(menuTile.GetRectTransform(), mousePos, UICam, out Vector2 anchoredPosition))
            {
                isDraggedFromCrafting = true;
                MenuDragging(placedObject, mousePos, anchoredPosition);
            }
        }
    }

    private void MenuDragging(PlacedObject placedObject, Vector2 mousePos, Vector2 anchoredPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySystem.GetItemContainer(), mousePos, UICam, out Vector2 tp);
        Vector2Int OriginOnInventory = inventorySystem.GetGridPos(tp);
        mouseDragGridPosOffset = Vector2Int.zero;
        mouseDragGridPosOffset = OriginOnInventory - placedObject.GetGridPos();
        mouseDragGridPosOffsetMenu = Vector2.zero;
        mouseDragGridPosOffsetMenu = anchoredPosition - placedObject.GetComponent<RectTransform>().anchoredPosition;
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
            draggedCraftTile = null;
            bool tryPlaceItem = false;
            bool tryRemoveItem = false;
            Cursor.visible = true;
            Vector2Int Origin = new Vector2Int();    
            InventoryItemSO POinventorySO = placedObject.GetInventoryItemSO();
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
                else if (placedObject.GetCraftTile() != null)
                {
                    tryRemoveItem = placedObject.GetCraftTile().TryRemoveItem();
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
                    if (isDraggedFromInventory)
                    {
                        Origin -= mouseDragGridPosOffset;
                    }
                    if (isDraggedFromEquipMenu || isDraggedFromCrafting)
                    {
                        Origin -= mouseDragGridPosOffset;             
                    }
                    if (isDraggedFromStash)
                    {
                        Origin -= mouseStashDragGridPosOffset;
                        Origin += new Vector2Int(XTransitionOffset, 0);                       
                    }
                    if (inventorySystem.IsValidGridPos(Origin))
                    {   
                        if (isDraggedFromStash)
                        {
                            if (inventorySystem.CheckStackable(placedObject))
                            {
                                inventorySystem.AddStackable(placedObject);
                            }
                            else
                            {
                                tryPlaceItem = inventorySystem.TryPlaceItem(POinventorySO, Origin, dir, out PlacedObject PO);
                            }
                        }
                        else
                        {
                            tryPlaceItem = inventorySystem.TryPlaceItem(POinventorySO, Origin, dir, out PlacedObject PO);
                        }                                               
                    }
                }
                if (itemStash.gameObject.activeSelf)
                {                    
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(itemStash.GetItemContainer(), mousePos, UICam, out Vector2 anchorPos))
                    {
                        Origin = itemStash.GetGridPos(anchorPos);                        
                        if (isDraggedFromInventory)
                        {
                            Origin -= mouseDragGridPosOffset;
                            Origin += new Vector2Int(-XTransitionOffset, 0);
                        }
                        else
                        {
                            Origin -= mouseStashDragGridPosOffset;
                        }
                        if (itemStash.IsValidGridPos(Origin))
                        {
                            tryPlaceItem = itemStash.TryPlaceItem(POinventorySO, Origin, dir, out PlacedObject PO);
                            if (tryPlaceItem)
                            {
                                inventorySystem.RemoveStackable(placedObject);
                                if (inventorySystem.CheckStackable(placedObject))
                                {
                                    inventorySystem.TryPlaceItem(POinventorySO, placedObject.GetGridPos(), placedObject.GetDir(), out PlacedObject obj);
                                }
                            }                           
                        }
                    }
                }
                else
                {
                    if (IsOnMenuTile(mousePos, out MenuTile menuTile))
                    {
                        if (menuTile.GetType() == typeof(EquipMenuWeaponTile) || menuTile.GetType() == typeof(EquipMenuSpellTile))
                        {
                            if (CheckStatRequirement(POinventorySO))
                            {
                                tryPlaceItem = menuTile.TryPlaceItem(POinventorySO, Origin, out PlacedObject PO);
                            }
                        }
                        else if (menuTile.GetType() == typeof(CraftMenuTile))
                        {
                            if (POinventorySO.Craftable)
                            {
                                tryPlaceItem = menuTile.TryPlaceItem(POinventorySO, Origin, out PlacedObject PO);
                            }                      
                        }                                                                      
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
                            inventorySystem.TryPlaceItem(POinventorySO, placedObject.GetGridPos(), placedObject.GetDir(), out PlacedObject PO);
                        }
                        else
                        {
                            itemStash.TryPlaceItem(POinventorySO, placedObject.GetGridPos(), placedObject.GetDir(), out PlacedObject PObj);
                        }
                    }                   
                    else
                    {
                        if (placedObject.GetWeaponEquipTile() != null)
                        {
                            placedObject.GetWeaponEquipTile().TryPlaceItem(POinventorySO, placedObject.GetGridPos(), out PlacedObject PO);
                        }
                        if (placedObject.GetSpellEquipTile() != null)
                        {
                            placedObject.GetSpellEquipTile().TryPlaceItem(POinventorySO, placedObject.GetGridPos(), out PlacedObject PO);
                        }
                        if (placedObject.GetCraftTile() != null)
                        {
                            placedObject.GetCraftTile().TryPlaceItem(POinventorySO, placedObject.GetGridPos(), out PlacedObject PO);
                        }
                    }                   
                }
            }
            isDragging = false;
            isDraggedFromInventory = false;
            isDraggedFromEquipMenu = false;
            isDraggedFromStash = false;
            isDraggedFromCrafting = false;
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
                            if (!item.Stackable)
                            {
                                placedObject.DestroySelf();
                                TooltipSystem.Hide();
                                inventorySystem.RemoveFromInventoryList(placedObject);
                                List<Vector2Int> gridPosList = placedObject.GetGridPosList();
                                foreach (Vector2Int gridPos in gridPosList)
                                {
                                    inventorySystem.GetGrid().GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
                                }
                            }
                            else
                            {
                                inventorySystem.RemoveStackable(placedObject);
                                if (!inventorySystem.CheckStackable(placedObject))
                                {
                                    placedObject.DestroySelf();
                                    TooltipSystem.Hide();
                                    inventorySystem.RemoveFromInventoryList(placedObject);
                                    List<Vector2Int> gridPosList = placedObject.GetGridPosList();
                                    foreach (Vector2Int gridPos in gridPosList)
                                    {
                                        inventorySystem.GetGrid().GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
                                    }
                                }
                            }                                                        
                            if (IT == ItemTypes.Spell)
                            {
                                if (ST != SpellTypes.None)
                                {
                                    PickedObject pickedObject = PickedObject.SpawnSpellWorld(item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f));
                                }
                            }
                            else if (IT == ItemTypes.Weapon)
                            {
                                if (WT != WeaponTypes.None)
                                {
                                    PickedObject pickedObject = PickedObject.SpawnWeaponWorld(item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f));
                                }
                            }
                            else if (IT == ItemTypes.Armor)
                            {
                                if (AT != ArmorTypes.None)
                                {
                                    PickedObject pickedObject = PickedObject.SpawnArmorWorld(item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f));
                                }
                            }
                            else
                            {
                                if (WT != WeaponTypes.None)
                                {
                                    PickedObject pickedObject = PickedObject.SpawnItemsWorld(item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f), WT);
                                }
                                else
                                {
                                    PickedObject pickedObject = PickedObject.SpawnItemsWorld(item, playerController.transform.position + playerController.GetRandomPosWithoutY(15f, -15f));
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
                                    inventorySystem.ConsumeFromInventory(placedObject);
                                    if (!inventorySystem.CheckStackable(placedObject))
                                    {
                                        placedObject.DestroySelf();
                                        List<Vector2Int> gridPosList = placedObject.GetGridPosList();
                                        foreach (Vector2Int gridPos in gridPosList)
                                        {
                                            inventorySystem.GetGrid().GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
                                        }
                                    }                                                               
                                }
                            }
                            if (item.ItemType == ItemTypes.ManaPotion)
                            {
                                if (playerController.PlayerTarget.Resource.ManaGain(item.AttributeAmount))
                                {
                                    inventorySystem.ConsumeFromInventory(placedObject);
                                    if (!inventorySystem.CheckStackable(placedObject))
                                    {
                                        placedObject.DestroySelf();
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
    }

    #endregion

    #region UtilityFunctions

    private bool CheckStatRequirement(InventoryItemSO itemSO)
    {
        bool isCompleted = false;
        for (int i = 0; i < itemSO.RequirementList.Count; i++)
        {
            isCompleted = attributeManager.IsStatRequirementCompleted(itemSO.RequirementList[i].attributeType, itemSO.RequirementList[i].attributeValue);
            if (!isCompleted)
            {
                return false;
            }
        }
        return isCompleted;
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

    public bool IsOnMenuTile(Vector2 currentPosition, out MenuTile menuTile)
    {
        foreach (var tile in tileList)
        {
            if (tile.IsOnTile(currentPosition, UICam))
            {
                menuTile = tile;
                return true;
            }
        }
        menuTile = null;
        return false;
    }

    public bool IsOnCraftMenu(Vector2 currentPosition, out CraftMenuTile menuTile)
    {
        foreach(var tile in CraftMenuControl.CraftTileList)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(tile.GetRectTransform(), currentPosition, UICam))
            {
                menuTile = tile;
                return true;
            }
        }
        menuTile = null;
        return false;
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

    #endregion

    //~~~~~~~~~~~~~~~~~~~~ Event Callback ~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void Tile_OnObjectPlacedinEquipTile(PlacedObject e, int num)
    {
        
    }

    #endregion

}
