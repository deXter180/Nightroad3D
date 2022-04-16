using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    private float throwDistance = 20f;
    private float cellSize;
    private Grid<GridObject> grid;
    private List<PlacedObject> InventoryList = new List<PlacedObject>();
    private RectTransform itemContainer;
    private InventoryUIHandler inventoryUI;
    public static event EventHandler<PlacedObject> OnPlacedOnInventory;
    public static event Action<ItemTypes> OnAddingInInventory;
    public static event Action<ItemTypes> OnRemovingFromInventory;
    public static InventorySystem Instance { get; private set; }
    public int PlacedObjectCount { get; private set; }

    private void Awake()
    {
        Instance = Instance == null ? this : null;
        inventoryUI = GetComponentInParent<InventoryUIHandler>();
        cellSize = inventoryUI.GetCellSize();
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, transform.position, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        itemContainer = transform.Find("ItemContainer").GetComponent<RectTransform>();
    }

    //~~~~~~~~~~~~~~~~~~ Utilities ~~~~~~~~~~~~~~~~~~

    public IEnumerator Test()
    {
        yield return new WaitForSeconds(2f);
        TryAddingItem(AssetCollections.GetInventoryItemSOFromList(ItemTypes.HealthPotion));
        TryAddingItem(AssetCollections.GetInventoryItemSOFromList(ItemTypes.Armor));
        TryAddingItem(AssetCollections.GetInventoryItemSOFromList(ItemTypes.Gloves));
        TryAddingItem(AssetCollections.GetInventoryItemSOFromList(ItemTypes.Boots));
        TryAddingItem(AssetCollections.GetInventoryItemSOFromList(ItemTypes.Shield));
        TryAddingItem(AssetCollections.GetInventoryItemSOFromList(ItemTypes.Helmet));
        TryAddingItem(AssetCollections.GetInventoryItemSOFromList(ItemTypes.ManaPotion));
        PickedObject.SpawnWeaponWorld(WeaponTypes.Axe, AssetCollections.GetWeaponInventorySO(WeaponTypes.Axe), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnWeaponWorld(WeaponTypes.Rifle, AssetCollections.GetWeaponInventorySO(WeaponTypes.Rifle), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnWeaponWorld(WeaponTypes.RocketLauncher, AssetCollections.GetWeaponInventorySO(WeaponTypes.RocketLauncher), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        PickedObject.SpawnWeaponWorld(WeaponTypes.Shotgun, AssetCollections.GetWeaponInventorySO(WeaponTypes.Shotgun), PlayerController.Instance.transform.position + PlayerController.Instance.GetRandomPosWithoutY(throwDistance, -throwDistance));
        //TryAddingItem(AssetCollections.GetWeaponInventorySO(WeaponTypes.Axe));
        //TryAddingItem(AssetCollections.GetWeaponInventorySO(WeaponTypes.Rifle));
        //TryAddingItem(AssetCollections.GetWeaponInventorySO(WeaponTypes.RocketLauncher));
        //TryAddingItem(AssetCollections.GetWeaponInventorySO(WeaponTypes.Shotgun));
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
        return itemContainer;
    }

    public void AddToInventoryList(PlacedObject placedObject)
    {
        InventoryList.Add(placedObject);
    }

    public void RemoveFromInventoryList(PlacedObject placedObject)
    {
        InventoryList.Remove(placedObject);
    }
    #endregion

    //~~~~~~~~~~~~~~~~~~ Main Functions ~~~~~~~~~~~~~~~~~~~

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

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                if (grid.GetGridObject(x, y).CanBuild())
                {
                    Vector2Int temp = new Vector2Int(x, y);
                    if (TryPlaceItem(inventoryItemSO, temp, InventoryItemSO.Dir.Down))
                    {
                        PlacedObject placedObject = inventoryItemSO.InventoryPrefab.GetComponent<PlacedObject>();
                        InventoryList.Add(placedObject);
                        OnAddingInInventory?.Invoke(inventoryItemSO.ItemType);
                        return true;
                    }
                }
            }
        }
        return false;
    }    

    public bool TryPlaceItem(InventoryItemSO inventoryItemSO, Vector2Int placedObjectOrigin, InventoryItemSO.Dir dir)
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
            PlacedObject placedObject = PlacedObject.Create(itemContainer, placedObjectWorldPos, placedObjectOrigin, dir, inventoryItemSO);
            placedObject.transform.rotation = Quaternion.Euler(0, 0, -inventoryItemSO.GetRotationAngle(dir));

            foreach(Vector2Int gridPos in gridPosList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObject(placedObject);
            }
            PlacedObjectCount = itemContainer.childCount;
            OnPlacedOnInventory?.Invoke(this, placedObject);
            return true;
        }
        else { return false; }
    }

    public bool TryRemoveItemAt(Vector2Int removeGridPos)
    {
        if (grid.GetGridObject(removeGridPos.x, removeGridPos.y).GetPlacedObject() != null)
        {
            PlacedObject placedObject = grid.GetGridObject(removeGridPos.x, removeGridPos.y).GetPlacedObject();
            placedObject.DestroySelf();
            InventoryList.Remove(placedObject);
            OnRemovingFromInventory(placedObject.GetItemType());
            List<Vector2Int> gridPosList = placedObject.GetGridPosList();
            foreach (Vector2Int gridPos in gridPosList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
            }
            return true;
        }
        else
        { 
            return false;
        }
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
            TryPlaceItem(AssetCollections.GetInventoryItemSOFromList(addedItem.itemType), addedItem.gridPos, addedItem.dir);
        }
    }
    #endregion
}
