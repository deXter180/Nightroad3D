using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItemSO : ScriptableObject
{
    public string ItemID;
    public Texture2D ItemIcon;
    public string ItemName;
    public string ItemDescription;
    public bool Usable;
    public int Width;
    public int Height;
    public ItemRarity Rarity;
    [SerializeField] protected int AttributeAmount;
    [SerializeField] protected Texture2D AttributeIcon;

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
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic
}