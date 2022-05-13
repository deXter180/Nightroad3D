using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItemSO : ScriptableObject
{
    public enum TileTypes
    {
        SpellTile,
        WeaponTile,
        InventoryTile
    }
    public string ItemID;  
    public string ItemName;
    public string ItemDescription;
    public bool Usable;
    public int Width;
    public int Height;
    public ItemTypes ItemType;
    public WeaponTypes WeaponType;
    public ArmorTypes ArmorType;
    public SpellTypes SpellType;
    public SpellCategories SpellCategory;
    public ItemRarity Rarity;
    [SerializeField] public int AttributeAmount;
    [SerializeField] public Texture2D AttributeIcon;
    [SerializeField] private Transform inventoryPrefab;
    [SerializeField] private Transform worldPrefab;
    public Transform InventoryPrefab { get => inventoryPrefab; }
    public Transform WorldPrefab { get => worldPrefab; }

    #region
    public enum Dir
    {
        Down,
        Left,
        Up,
        Right,
    }

    public int GetAttributeAmount()
    {
        return AttributeAmount;

    }

    public Texture2D GetAttributeIcon()
    {
        return AttributeIcon;
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
        //rectTransform.sizeDelta = new Vector2(inventoryItemSO.Width, inventoryItemSO.Height) * cellSize;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.SetAsLastSibling();
        visualTransform.gameObject.SetActive(true);
    }

}


