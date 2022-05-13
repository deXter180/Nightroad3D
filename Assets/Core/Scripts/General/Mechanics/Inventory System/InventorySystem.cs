using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventorySystem : Singleton<InventorySystem>
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    private float throwDistance = 20f;
    private float cellSize;
    private Grid<GridObject> grid;
    private bool isInitialized => InventoryList != null;
    private List<PlacedObject> InventoryList;
    private InventoryUIHandler inventoryUI;
    private InventoryContainor itemContainor;
    public static event EventHandler<PlacedObject> OnPlacedOnInventory;
    public static event Action<ItemTypes> OnAddingInInventory;
    public static event Action<ItemTypes> OnRemovingFromInventory;
    public static event Action<ItemTypes> OnConsumingFromInventory;

    protected override void Awake()
    {
        base.Awake();
        itemContainor = GetComponentInChildren<InventoryContainor>();
        inventoryUI = GetComponentInParent<InventoryUIHandler>();
        cellSize = inventoryUI.GetCellSize();
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, transform.position, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        InitializeInventory();
    }

    private void OnEnable()
    {
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
    }

    private void OnDisable()
    {
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
    }

    //~~~~~~~~~~~~~~~~~~ Utilities ~~~~~~~~~~~~~~~~~~

    private void Test()
    {
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.HealthPotion));
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.Armor));
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.Gloves));
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.Boots));
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.Shield));
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.Helmet));
        TryAddingItem(GameController.GetInventoryItemSOFromList(ItemTypes.ManaPotion));
        PickedObject.SpawnWeaponWorld(WeaponTypes.Axe, GameController.GetWeaponInventorySO(WeaponTypes.Axe), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnWeaponWorld(WeaponTypes.Rifle, GameController.GetWeaponInventorySO(WeaponTypes.Rifle), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnWeaponWorld(WeaponTypes.RocketLauncher, GameController.GetWeaponInventorySO(WeaponTypes.RocketLauncher), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnWeaponWorld(WeaponTypes.Shotgun, GameController.GetWeaponInventorySO(WeaponTypes.Shotgun), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnSpellWorld(SpellTypes.Dash, GameController.GetSpellInventorySO(SpellTypes.Dash), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnSpellWorld(SpellTypes.FireBall, GameController.GetSpellInventorySO(SpellTypes.FireBall), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnSpellWorld(SpellTypes.FreezeBlast, GameController.GetSpellInventorySO(SpellTypes.FreezeBlast), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnItemsWorld(ItemTypes.Ammo, GameController.GetInventoryItemSOFromList(ItemTypes.Ammo, WeaponTypes.Rifle), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance), WeaponTypes.Rifle);
        PickedObject.SpawnItemsWorld(ItemTypes.Ammo, GameController.GetInventoryItemSOFromList(ItemTypes.Ammo, WeaponTypes.RocketLauncher), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance), WeaponTypes.RocketLauncher);
        PickedObject.SpawnItemsWorld(ItemTypes.Ammo, GameController.GetInventoryItemSOFromList(ItemTypes.Ammo, WeaponTypes.Shotgun), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance), WeaponTypes.Shotgun);
    }

    #region
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

    public void RemoveFromInventoryList(PlacedObject placedObject)
    {
        InventoryList.Remove(placedObject);
        OnRemovingFromInventory?.Invoke((placedObject.GetItemType()));
    }

    public void ConsumeFromInventory(PlacedObject placedObject)
    {
        InventoryList.Remove(placedObject);
        OnConsumingFromInventory?.Invoke(placedObject.GetItemType());
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~ Main Functions ~~~~~~~~~~~~~~~~~~~

    public void InitializeInventory()
    {
        if (!isInitialized)
            InventoryList = new List<PlacedObject>();
        else
            return;
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

    public bool TryAddingItem(InventoryItemSO inventoryItemSO)
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
                        if (TryPlaceItem(inventoryItemSO, temp, InventoryItemSO.Dir.Down, out PlacedObject placedObject))
                        {
                            InventoryList.Add(placedObject);
                            OnAddingInInventory?.Invoke(inventoryItemSO.ItemType);
                            return true;
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
                    placedObjectWorldPos = grid.GetWorldPosAtOrigin(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y - 0.1f) * grid.GetCellSize();
                    break;
                case InventoryItemSO.Dir.Right:
                    placedObjectWorldPos = grid.GetWorldPosAtOrigin(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x - 0.2f, rotationOffset.y - 0.1f) * grid.GetCellSize();
                    break;
                case InventoryItemSO.Dir.Left:
                    placedObjectWorldPos = grid.GetWorldPosAtOrigin(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y - 0.2f) * grid.GetCellSize();
                    break;
                case InventoryItemSO.Dir.Up:
                    placedObjectWorldPos = grid.GetWorldPosAtOrigin(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x - 0.2f, rotationOffset.y - 0.2f) * grid.GetCellSize();
                    break;
            }
            placedObject = PlacedObject.Create(itemContainor.ContainorRect, placedObjectWorldPos, placedObjectOrigin, dir, inventoryItemSO);
            placedObject.transform.rotation = Quaternion.Euler(0, 0, -inventoryItemSO.GetRotationAngle(dir));
            foreach (Vector2Int gridPos in gridPosList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObject(placedObject);
            }
            OnPlacedOnInventory?.Invoke(this, placedObject);
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
            RemoveFromInventoryList(placedObject);          
            placedObject.DestroySelf();                      
            List<Vector2Int> gridPosList = placedObject.GetGridPosList();
            foreach (Vector2Int gridPos in gridPosList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
            }
            return true;
        }
        return false;
    }

    public bool RemoveItem(PlacedObject placedObject)
    {
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
        }
        return false;
    }

    public void ClearGrid()
    {
        foreach (var obj in grid.gridArray)
        {
            obj.ClearPlacedObject();
        }
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

    //~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~

    private void SceneLoader_OnNewGameStart()
    {
        ClearGrid();
        itemContainor.ContainorRect.DeleteChildren();
        InventoryList = new List<PlacedObject>();        
        Test(); // Remove this
    }

    //~~~~~~~~~~~~~~~~~ Save & Load ~~~~~~~~~~~~~~~~~~~~

    #region
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
