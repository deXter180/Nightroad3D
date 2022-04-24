using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMenuControl : MonoBehaviour
{
    public static List<EquipMenuWeaponTile> WeaponTileList = new List<EquipMenuWeaponTile>();
    public static List<EquipMenuSpellTile> SpellTileList = new List<EquipMenuSpellTile>();
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
        EquipMenuWeaponTile[] equipWeaponArray = GetComponentsInChildren<EquipMenuWeaponTile>();
        EquipMenuSpellTile[] equipSpellArray = GetComponentsInChildren<EquipMenuSpellTile>();
        foreach (var tile in equipWeaponArray)
        {
            tile.GetRectTransform().sizeDelta = new Vector2(100, 100);
            WeaponTileList.Add(tile);
        }

        foreach (var tile in equipSpellArray)
        {
            tile.GetRectTransform().sizeDelta = new Vector2(50, 50);
            SpellTileList.Add(tile);
        }


    }

}
