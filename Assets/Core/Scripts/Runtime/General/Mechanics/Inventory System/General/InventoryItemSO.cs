using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItemSO : ScriptableObject
{
    #region SerializedVariables

    [SerializeField] private string itemID;  
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private bool usable;
    [SerializeField] private bool craftable;
    [SerializeField] private bool stackable;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private ItemTypes itemType;
    [SerializeField] private WeaponTypes weaponType;
    [SerializeField] private ArmorTypes armorType;
    [SerializeField] private SpellTypes spellType;
    [SerializeField] private SpellCategories spellCategory;
    [SerializeField] private ItemRarity rarity;
    [SerializeField] private int attributeAmount;
    [SerializeField] private Sprite attributeIcon;
    [SerializeField] private Transform inventoryPrefab;
    [SerializeField] private Transform worldPrefab;
    [SerializeField] private List<RequiredStatForEquip> requirementList;

    #endregion

    #region Properties

    public string ItemID => itemID;
    public string ItemName => itemName;
    public string ItemDescription => itemDescription;
    public bool Usable => usable;
    public bool Craftable => craftable;
    public bool Stackable => stackable;
    public int Width => width;
    public int Height => height;
    public ItemTypes ItemType => itemType;
    public WeaponTypes WeaponType => weaponType;
    public ArmorTypes ArmorType => armorType;
    public SpellTypes SpellType => spellType;
    public SpellCategories SpellCategory => spellCategory;
    public ItemRarity Rarity => rarity;
    public int AttributeAmount => attributeAmount;
    public Sprite AttributeIcon => attributeIcon;
    public Transform InventoryPrefab { get => inventoryPrefab; }
    public Transform WorldPrefab { get => worldPrefab; }
    public List<RequiredStatForEquip> RequirementList => requirementList;

    #endregion

    #region Utilities

    public enum Dir
    {
        Down,
        Left,
        Up,
        Right,
    }

    public enum TileTypes
    {
        SpellTile,
        WeaponTile,
        CraftTile,
        InventoryTile
    }

    public virtual void UseItem()
    {
        Debug.Log("Using" + ItemName);
    }

    public static Dir GetNextDir(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, Width);
            case Dir.Up: return new Vector2Int(Width, Height);
            case Dir.Right: return new Vector2Int(Height, 0);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir)
        {
            default:
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }

    #endregion

    #region Mechanics

    public static void CreateGridVisual(Transform visualParentTransform, InventoryItemSO inventoryItemSO, float cellSize, TileTypes tileType)
    {
        Transform visualTransform = Instantiate(InventoryUIHandler.Instance.gridVisual, visualParentTransform);
        visualTransform.gameObject.SetActive(false);
        RectTransform rectTransform = visualTransform.GetComponent<RectTransform>();
        if (tileType == TileTypes.InventoryTile)
        {
            if (inventoryItemSO.Width == 1 && inventoryItemSO.Height == 1)
            {
                rectTransform.sizeDelta = new Vector2(inventoryItemSO.Width, inventoryItemSO.Height) * cellSize * 1.6f;
            }
            else if (inventoryItemSO.Width == 1)
            {
                rectTransform.sizeDelta = new Vector2(inventoryItemSO.Width * 1.6f, inventoryItemSO.Height) * cellSize;
            }
            else if (inventoryItemSO.Height == 1)
            {
                rectTransform.sizeDelta = new Vector2(inventoryItemSO.Width, inventoryItemSO.Height * 1.6f) * cellSize;
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(inventoryItemSO.Width, inventoryItemSO.Height) * cellSize;
            }
        }
        else if (tileType == TileTypes.WeaponTile)
        {
            rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
        }
        else if (tileType == TileTypes.SpellTile)
        {
            rectTransform.sizeDelta = new Vector2(cellSize * 3.2f, cellSize * 3.2f);
        }   
        else if (tileType == TileTypes.CraftTile)
        {
            rectTransform.sizeDelta = new Vector2(cellSize * 3.2f, cellSize * 3.2f);
        }
        //rectTransform.sizeDelta = new Vector2(inventoryItemSO.Width, inventoryItemSO.Height) * cellSize;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.SetAsLastSibling();
        visualTransform.gameObject.SetActive(true);
    }

    public string GetRequirementText()
    {
        string text = "";
        for (int i = 0; i < requirementList.Count; i++)
        {
            text = $"{text}  {requirementList[i].attributeType} : {requirementList[i].attributeValue} ";
        }
        return text;
    }

    #endregion
}


