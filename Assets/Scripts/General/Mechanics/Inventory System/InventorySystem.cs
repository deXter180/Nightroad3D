using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    private Grid<GridObject> grid;
    private List<InventoryItemSO> InventoryList;
    private RectTransform itemContainer;
    [SerializeField] private Transform InventoryGridStart;
    public static event EventHandler<PlacedObject> OnObjectPlaced;          

    private void Awake()
    {
        Instance = this;

        int gridWidth = 7;
        int gridHeight = 7;
        float cellSize = 127f;
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, InventoryGridStart.position, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        itemContainer = transform.Find("ItemContainer").GetComponent<RectTransform>();
        InventoryList = new List<InventoryItemSO>();
    }

    private void Start()
    {
        StartCoroutine(Test());
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(1f);
        TryAddingItem(InventoryAssets.Instance.GetInventoryItemSO(ItemTypes.Armor));
    }

    //~~~~~~~~~~~~~~~~~~ Utilities ~~~~~~~~~~~~~~~~~~

    public Grid<GridObject> GetGrid()
    {
        return grid;
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
                        Debug.Log("" + temp.x + "" + temp.y);
                        InventoryList.Add(inventoryItemSO);
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
            Vector3 placedObjectWorldPos = grid.GetWorldPosAtOrigin(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();
            PlacedObject placedObject = PlacedObject.Create(itemContainer, placedObjectWorldPos, placedObjectOrigin, dir, inventoryItemSO);
            placedObject.transform.rotation = Quaternion.Euler(0, 0, -inventoryItemSO.GetRotationAngle(dir));

            foreach(Vector2Int gridPos in gridPosList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObject(placedObject);
            }

            OnObjectPlaced?.Invoke(this, placedObject);
            return true;
        }
        else { return false; }
    }

    public void RemoveItemAt(Vector2Int removeGridPos)
    {
        PlacedObject placedObject = grid.GetGridObject(removeGridPos.x, removeGridPos.y).GetPlacedObject();

        if(placedObject != null)
        {
            placedObject.DestroySelf();
            List<Vector2Int> gridPosList = placedObject.GetGridPosList();
            foreach (Vector2Int gridPos in gridPosList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
            }
        }
    }

    //~~~~~~~~~~~~~~~~~ Save & Load ~~~~~~~~~~~~~~~~~~~~

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
            TryPlaceItem(InventoryAssets.Instance.GetInventoryItemSO(addedItem.itemType), addedItem.gridPos, addedItem.dir);
        }
    }

    //~~~~~~~~~~~~~~~ Grid Object Class ~~~~~~~~~~~~~~~~

    public class GridObject 
    {
        private Grid<GridObject> grid;
        private int x;
        private int y;
        public PlacedObject placedObject;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            placedObject = null;
        }

        public override string ToString()
        {
            return x + ", " + y + "\n" + placedObject;
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObject GetPlacedObject()
        {
            return placedObject;
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }

        public bool HasPlacedObject()
        {
            return placedObject != null;
        }
    }


}
