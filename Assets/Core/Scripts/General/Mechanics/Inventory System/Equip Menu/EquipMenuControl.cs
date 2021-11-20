using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMenuControl : MonoBehaviour
{
    public static List<EquipMenuTile> EquipTileList = new List<EquipMenuTile>();
    public static EquipMenuControl Instance { get; private set; }
    public RectTransform menuContainer;
    public Camera UICam;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(Instance);
        EquipMenuTile[] equipArray = GetComponentsInChildren<EquipMenuTile>();
        foreach (var tile in equipArray)
        {
            tile.GetRectTransform().sizeDelta = new Vector2(100, 100);
            EquipTileList.Add(tile);
        }
    }

}
