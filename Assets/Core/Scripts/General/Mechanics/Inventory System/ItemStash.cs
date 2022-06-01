using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemStash : Singleton<ItemStash>
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    private float cellSize;
    private Grid<GridObject> grid;
    private bool isInitialized => StashList != null;
    private List<PlacedObject> StashList;
    private InventoryUIHandler inventoryUI;
    private InventoryContainor itemContainor;

    protected override void Awake()
    {
        base.Awake();
        itemContainor = GetComponentInChildren<InventoryContainor>();
        inventoryUI = GetComponentInParent<InventoryUIHandler>();
        cellSize = inventoryUI.GetCellSize();
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, transform.position, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        StashList = new List<PlacedObject>();
    }

    private void OnEnable()
    {
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
    }

    private void OnDisable()
    {
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
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

    #endregion

    public void ClearStash(List<SavedStashItems> savedItems)
    {
        foreach (var PO in StashList)
        {
            SavedStashItems SI = new SavedStashItems(PO.GetInventoryItemSO(), PO.GetGridPos(), PO.GetDir());
            savedItems.Add(SI);
        }
        ClearStash();
    }

    private void ClearStash()
    {
        foreach (var obj in grid.gridArray)
        {
            obj.ClearPlacedObject();
        }
        itemContainor.ContainorRect.DeleteChildren();
        StashList = new List<PlacedObject>();
    }

    public bool IsStashFull()
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

    public void AddToStash(List<StashItems> stashItems)
    {
        StashList = new List<PlacedObject>();
        foreach(var item in stashItems)
        {
            InventoryItemSO itemSO = null ;
            if (item.ItemType == ItemTypes.Weapon && item.WeaponType != WeaponTypes.None)
            {
                itemSO = GameController.GetWeaponInventorySO(item.WeaponType);
            }
            else if (item.ItemType == ItemTypes.Armor && item.ArmorType != ArmorTypes.None)
            {
                itemSO = GameController.GetArmorInventorySO(item.ArmorType);
            }
            else if (item.ItemType == ItemTypes.Spell && item.SpellType != SpellTypes.None)
            {
                itemSO = GameController.GetSpellInventorySO(item.SpellType);
            }
            else if (item.ItemType == ItemTypes.Ammo && item.WeaponType != WeaponTypes.None)
            {
                itemSO = GameController.GetInventoryItemSOFromList(item.ItemType, item.WeaponType);
            }
            else
            {
                itemSO = GameController.GetInventoryItemSOFromList(item.ItemType);
            }

            if (itemSO != null)
            {
                for (int i = 0; i < item.Amount; i++)
                {
                    if (TryAddingStashItem(itemSO))
                    {

                    }
                }
            }
        }
    }

    public void ReloadStash(List<SavedStashItems> savedItems)
    {
        foreach (var SI in savedItems)
        {
            if (grid.GetGridObject(SI.Position.x, SI.Position.y).CanBuild())
            {
                if (TryPlaceItem(SI.InventorySO, SI.Position, SI.Dir, out PlacedObject placedObject))
                {

                }
            }
        }
    }

    private bool TryAddingStashItem(InventoryItemSO inventoryItemSO)
    {
        if (grid != null && isInitialized)
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
                    placedObjectWorldPos = grid.GetWorldPosAtOrigin(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x + 0.07f, rotationOffset.y - 0.1f) * grid.GetCellSize();
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
            placedObject = PlacedObject.Create(itemContainor.ContainorRect, placedObjectWorldPos, placedObjectOrigin, dir, inventoryItemSO, false);
            foreach (Vector2Int gridPos in gridPosList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObject(placedObject);
            }
            StashList.Add(placedObject);
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
            List<Vector2Int> gridPosList = placedObject.GetGridPosList();
            foreach (Vector2Int gridPos in gridPosList)
            {
                grid.GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
            }
            StashList.Remove(placedObject);
            placedObject.DestroySelf();
            return true;
        }
        return false;
    }

    //~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~

    private void SceneLoader_OnNewGameStart()
    {
        ClearStash();
    }
}

[System.Serializable]
public class StashItems
{
    public int Amount;
    public ItemTypes ItemType;
    [Tooltip("Fill this for Weapon or Ammo Type")] public WeaponTypes WeaponType;
    [Tooltip("Fill this for Armor Type")] public ArmorTypes ArmorType;
    [Tooltip("Fill this for Spell Type")] public SpellTypes SpellType;
}

public class SavedStashItems
{
    public InventoryItemSO InventorySO;
    public Vector2Int Position;
    public InventoryItemSO.Dir Dir;

    public SavedStashItems (InventoryItemSO SO, Vector2Int Pos, InventoryItemSO.Dir dir)
    {
        InventorySO = SO;
        Position = Pos;
        Dir = dir;
    }
}
