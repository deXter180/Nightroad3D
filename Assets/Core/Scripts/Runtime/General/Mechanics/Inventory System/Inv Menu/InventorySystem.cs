using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventorySystem : Singleton<InventorySystem>
{

    #region Variables

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    private float throwDistance = 20f;
    private float cellSize;
    private Grid<GridObject> grid;
    private bool isInitialized => InventoryList != null;
    private List<PlacedObject> InventoryList;
    public Dictionary<ItemTypes, int> ItemDict;
    private InventoryUIHandler inventoryUI;
    private InventoryContainor itemContainor;
    public static event Action<ItemTypes, PlacedObject> OnAddingToInventory;
    public static event Action<ItemTypes, PlacedObject> OnRemovingFromInventory;
    public static event Action<ItemTypes> OnConsumingFromInventory;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        itemContainor = GetComponentInChildren<InventoryContainor>();
        inventoryUI = GetComponentInParent<InventoryUIHandler>();
        cellSize = inventoryUI.GetCellSize();
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize , transform.position, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        InitializeInventory();
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~ Utilities ~~~~~~~~~~~~~~~~~~

    private void Test()
    {
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.HealthPotion));
        TryAddingItem(GameController.GetArmorInventorySO(ArmorTypes.RoyalPlate));
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.Gloves));
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.Boots));
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.Shield));
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.Helmet));
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.ManaPotion));
        PickedObject.SpawnWeaponWorld(GameController.GetWeaponInventorySO(WeaponTypes.Axe), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnWeaponWorld(GameController.GetWeaponInventorySO(WeaponTypes.Rifle), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnWeaponWorld(GameController.GetWeaponInventorySO(WeaponTypes.RocketLauncher), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnWeaponWorld(GameController.GetWeaponInventorySO(WeaponTypes.Shotgun), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnSpellWorld(GameController.GetSpellInventorySO(SpellTypes.Dash), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnSpellWorld(GameController.GetSpellInventorySO(SpellTypes.ChainLightening), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnSpellWorld(GameController.GetSpellInventorySO(SpellTypes.FireBall), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnSpellWorld(GameController.GetSpellInventorySO(SpellTypes.FreezeBlast), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnItemsWorld(GameController.GetInventoryItemSOFromList(ItemTypes.Ammo, WeaponTypes.Rifle), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance), WeaponTypes.Rifle);
        PickedObject.SpawnItemsWorld(GameController.GetInventoryItemSOFromList(ItemTypes.Ammo, WeaponTypes.RocketLauncher), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance), WeaponTypes.RocketLauncher);
        PickedObject.SpawnItemsWorld(GameController.GetInventoryItemSOFromList(ItemTypes.Ammo, WeaponTypes.Shotgun), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance), WeaponTypes.Shotgun);
        PickedObject.SpawnWeaponWorld(GameController.GetInventoryItemSOFromList(ItemTypes.Herb), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnWeaponWorld(GameController.GetInventoryItemSOFromList(ItemTypes.Herb), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnWeaponWorld(GameController.GetInventoryItemSOFromList(ItemTypes.Herb), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedRecipe.SpawnItemsWorld(GameController.GetRecipeSOFromList(RecipeTypes.P3C1), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
    }

    #region Utility

    public Grid<GridObject> GetGrid()
    {
        return grid;
    }

    public int GetWidth()
    {
        return gridWidth;
    }

    public int GetHeight()
    {
        return gridHeight;
    }

    public Vector2Int GetGridPos(Vector3 worldPos)
    {
        grid.GetCellXYpos(worldPos, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public Vector2Int GetGridLocalPos(Vector3 worldPos)
    {
        grid.GetCellXYLoaclPos(worldPos, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public bool IsValidGridPos(Vector2Int gridPos)
    {
        return grid.IsValidGridPos(gridPos);
    }

    public RectTransform GetItemContainer()
    {
        return itemContainor.ContainorRect;
    }

    private void ClearGrid()
    {
        foreach (var obj in grid.gridArray)
        {
            obj.ClearPlacedObject();
        }
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~ Main Functions ~~~~~~~~~~~~~~~~~~~

    #region Mechanics

    public void InitializeInventory()
    {
        if (!isInitialized)
        {
            InventoryList = new List<PlacedObject>();
            ItemDict = new Dictionary<ItemTypes, int>();
        }           
        else
            return;
    }

    public void ChangeTooltip(bool isTrue)
    {
        for (int i = 0; i < InventoryList.Count; i++)
        {
            InventoryList[i].TooltipControl(isTrue);
            InventoryList[i].StopDragging(isTrue);
        }
    }

    public bool IsInventoryFull()
    {
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                if (grid.GetGridObject(x, y).CanBuild())
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool TryAddingItem(InventoryItemSO inventoryItemSO, int amount = 1)
    {
        InitializeInventory();
        if (grid != null)
        {
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    if (grid.GetGridObject(x, y).CanBuild())
                    {
                        Vector2Int temp = new Vector2Int(x, y);
                        if (inventoryItemSO.Stackable)
                        {
                            if (!ItemDict.ContainsKey(inventoryItemSO.ItemType))
                            {
                                if (TryPlaceItem(inventoryItemSO, temp, InventoryItemSO.Dir.Down, out PlacedObject placedObject))
                                {
                                    ItemDict.Add(inventoryItemSO.ItemType, amount);
                                    placedObject.SetupTooltip();
                                    return true;
                                }
                            }
                            else
                            {
                                ItemDict[inventoryItemSO.ItemType] += 1;
                                var PO = GetAvailableItem(inventoryItemSO.ItemType);
                                if (PO!= null)
                                {
                                    PO.SetupTooltip();
                                }
                                return true;
                            }
                        }
                        else
                        {
                            if (TryPlaceItem(inventoryItemSO, temp, InventoryItemSO.Dir.Down, out PlacedObject placedObject))
                            {
                                return true;
                            }
                        }                       
                    }
                }
            }
        }    
        return false;
    }    

    public bool TryPlaceItem(InventoryItemSO inventoryItemSO, Vector2Int placedObjectOrigin, InventoryItemSO.Dir dir, out PlacedObject placedObject)
    {
        List<Vector2Int> gridPosList = inventoryItemSO.GetGridPositionList(placedObjectOrigin, dir);
        bool canPlace = true;
        foreach (Vector2Int gridPos in gridPosList)
        {
            bool isValidPos = grid.IsValidGridPos(gridPos);
            if (!isValidPos)
            {
                canPlace = false;
                break;
            }
            if (!grid.GetGridObject(gridPos.x, gridPos.y).CanBuild())
            {
                canPlace = false;
                break;
            }
        }
        if (canPlace)
        {
            Vector2Int rotationOffset = inventoryItemSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPos = new Vector3();
            switch (dir)
            {
                case InventoryItemSO.Dir.Down:
                    placedObjectWorldPos = grid.GetWorldPosAtOrigin(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x + 0.1f, rotationOffset.y - 0.1f) * grid.GetCellSize();
                    break;
                case InventoryItemSO.Dir.Right:
                    placedObjectWorldPos = grid.GetWorldPosAtOrigin(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x - 0.2f, rotationOffset.y) * grid.GetCellSize();
                    break;
                case InventoryItemSO.Dir.Left:
                    placedObjectWorldPos = grid.GetWorldPosAtOrigin(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y - 0.25f) * grid.GetCellSize();
                    break;
                case InventoryItemSO.Dir.Up:
                    placedObjectWorldPos = grid.GetWorldPosAtOrigin(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x - 0.2f, rotationOffset.y - 0.2f) * grid.GetCellSize();
                    break;
            }
            placedObject = PlacedObject.Create(itemContainor.ContainorRect, placedObjectWorldPos, placedObjectOrigin, dir, inventoryItemSO, true);
            foreach (Vector2Int gridPos in gridPosList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObject(placedObject);
            }
            InventoryList.Add(placedObject);
            OnAddingToInventory?.Invoke(placedObject.GetItemType(), placedObject);
            return true;
        }
        placedObject = null;
        return false;
    }

    public bool TryRemoveItemAt(Vector2Int removeGridPos)
    {
        if (grid.GetGridObject(removeGridPos.x, removeGridPos.y).GetPlacedObject() != null)
        {
            PlacedObject placedObject = grid.GetGridObject(removeGridPos.x, removeGridPos.y).GetPlacedObject();
            var type = placedObject.GetInventoryItemSO().ItemType;
            RemoveFromInventoryList(placedObject);                                            
            List<Vector2Int> gridPosList = placedObject.GetGridPosList();
            foreach (Vector2Int gridPos in gridPosList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
            }           
            placedObject.DestroySelf();
            return true;
        }
        return false;
    }

    public bool RemoveItem(PlacedObject placedObject)
    {
        var type = placedObject.GetInventoryItemSO().ItemType;
        if (InventoryList.Contains(placedObject))
        {
            foreach (var obj in grid.gridArray)
            {
                if (obj.GetPlacedObject() == placedObject)
                {
                    RemoveFromInventoryList(placedObject);

                    placedObject.DestroySelf();
                    List<Vector2Int> gridPosList = placedObject.GetGridPosList();
                    foreach (Vector2Int gridPos in gridPosList)
                    {
                        grid.GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
                    }
                    return true;
                }
            }
            RemoveStackable(placedObject);
        }
        return false;
    }

    public void RemoveFromInventoryList(PlacedObject placedObject)
    {
        InventoryList.Remove(placedObject);
        OnRemovingFromInventory?.Invoke(placedObject.GetItemType(), placedObject);
    }

    public void RemoveFromInvDict(PlacedObject placedObject)
    {
        if (ItemDict.ContainsKey(placedObject.GetItemType()))
        {
            ItemDict.Remove(placedObject.GetItemType());
        }
    }

    public void AddStackable(PlacedObject placedObject)
    {
        var type = placedObject.GetInventoryItemSO().ItemType;
        if (ItemDict.ContainsKey(type))
        {
            ItemDict[type] += 1;
        }
    }

    public void RemoveStackable(PlacedObject placedObject)
    {
        if (placedObject != null && placedObject.GetInventoryItemSO().Stackable)
        {
            var type = placedObject.GetInventoryItemSO().ItemType;
            if (ItemDict.ContainsKey(type))
            {
                if (ItemDict[type] >= 1)
                {
                    ItemDict[type] -= 1;
                    if (ItemDict[type] == 0)
                    {
                        ItemDict.Remove(type);
                        RemoveItem(placedObject);
                    }
                    else
                    {
                        placedObject.SetupTooltip();
                        placedObject.UpdateTooltip();
                    }
                }
                else
                {
                    ItemDict.Remove(type);
                    RemoveItem(placedObject);
                }
            }
        }
    }

    public bool CheckStackable(PlacedObject placedObject)
    {
        if (placedObject.GetInventoryItemSO().Stackable)
        {
            var type = placedObject.GetInventoryItemSO().ItemType;
            if (ItemDict.ContainsKey(type))
            {
                if (ItemDict[type] >= 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void ConsumeFromInventory(PlacedObject placedObject)
    {
        InventoryList.Remove(placedObject);
        RemoveStackable(placedObject);
        OnConsumingFromInventory?.Invoke(placedObject.GetItemType());
    }

    private bool IsAmmoAvailable()
    {
        foreach (var item in InventoryList)
        {
            if (item.GetItemType() == ItemTypes.Ammo)
            {
                return true;
            }
        }
        return false;
    }

    public PlacedObject GetAvailableAmmo(WeaponTypes weaponType)
    {
        if (IsAmmoAvailable())
        {
            foreach (var item in InventoryList)
            {
                if (item.GetItemType() == ItemTypes.Ammo)
                {
                    if (item.GetInventoryItemSO().WeaponType == weaponType)
                    {
                        return item;
                    }
                }
            }
        }
        return null;
    }

    public void ResetInventory()
    {
        ClearGrid();
        itemContainor.ContainorRect.DeleteChildren();
        InventoryList.Clear();
        ItemDict.Clear();
        Test(); // Remove this
    }

    public int GetItemAmount(ItemTypes itemType)
    {
        if (ItemDict.TryGetValue(itemType, out var amount))
        {
            return amount;
        }
        return 0;
    }

    public PlacedObject GetAvailableItem(ItemTypes itemType)
    {
        for (int i = 0; i < InventoryList.Count; i++)
        {
            if (InventoryList[i].GetItemType() == itemType)
            {
                return InventoryList[i];
            }
        }
        return null;
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~



    //~~~~~~~~~~~~~~~~~ Save & Load ~~~~~~~~~~~~~~~~~~~~

    #region SaveLoad

    [Serializable]
    public struct AddedItem
    {
        public ItemTypes itemType;
        public Vector2Int gridPos;
        public InventoryItemSO.Dir dir;
    }

    [Serializable]
    public struct ListAddedItem
    {
        public List<AddedItem> addedItemList;
    }

    public string Save()
    {
        List<PlacedObject> placedObjectList = new List<PlacedObject>();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                if (grid.GetGridObject(x, y).HasPlacedObject())
                {
                    placedObjectList.Remove(grid.GetGridObject(x, y).GetPlacedObject());
                    placedObjectList.Add(grid.GetGridObject(x, y).GetPlacedObject());
                }
            }
        }

        List<AddedItem> addedItemList = new List<AddedItem>();
        foreach(PlacedObject placedObject in placedObjectList)
        {
            addedItemList.Add(new AddedItem
            {
                dir = placedObject.GetDir(),
                gridPos = placedObject.GetGridPos(),
                itemType = placedObject.GetItemType()
            });
        }

        return JsonUtility.ToJson(new ListAddedItem { addedItemList = addedItemList });               
    }

    public void Load(string loadString)
    {
        ListAddedItem listAddedItem = JsonUtility.FromJson<ListAddedItem>(loadString);

        foreach(AddedItem addedItem in listAddedItem.addedItemList)
        {
            TryPlaceItem(GameController.GetInventoryItemSOFromList(addedItem.itemType), addedItem.gridPos, addedItem.dir, out PlacedObject placedObject);
        }
    }

    #endregion
}
