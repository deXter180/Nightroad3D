using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMenuControl : Singleton<EquipMenuControl>
{
    private bool isArmorAdded;
    private ArmorTypes equippedArmorType;
    public bool IsArmorAdded => isArmorAdded;
    public static List<EquipMenuWeaponTile> WeaponTileList = new List<EquipMenuWeaponTile>();
    public static List<EquipMenuSpellTile> SpellTileList = new List<EquipMenuSpellTile>();

    protected override void Awake()
    {
        base.Awake();
        InitializeEquipMenu();
    }

    private void OnEnable()
    {
        isArmorAdded = false;
    }

    private void OnDisable()
    {
        foreach (var tile in WeaponTileList)
        {
            tile.OnPlacedOnWeaponMenu -= Tile_OnPlacedOnWeaponMenu;
            tile.OnRemovedFromWeaponMenu -= Tile_OnRemovedFromWeaponMenu;
        }
        foreach (var tile in SpellTileList)
        {
            tile.OnPlacedOnSpellMenu -= Tile_OnPlacedOnSpellMenu;
            tile.OnRemovedFromSpellMenu -= Tile_OnRemovedFromSpellMenu;
        }
    }

    private void InitializeEquipMenu()
    {
        EquipMenuWeaponTile[] equipWeaponArray = GetComponentsInChildren<EquipMenuWeaponTile>();
        EquipMenuSpellTile[] equipSpellArray = GetComponentsInChildren<EquipMenuSpellTile>();
        foreach (var tile in equipWeaponArray)
        {
            tile.OnPlacedOnWeaponMenu += Tile_OnPlacedOnWeaponMenu;
            tile.OnRemovedFromWeaponMenu += Tile_OnRemovedFromWeaponMenu;
            tile.GetRectTransform().sizeDelta = new Vector2(100, 100);
            WeaponTileList.Add(tile);
        }
        foreach (var tile in equipSpellArray)
        {
            tile.OnPlacedOnSpellMenu += Tile_OnPlacedOnSpellMenu;
            tile.OnRemovedFromSpellMenu += Tile_OnRemovedFromSpellMenu;
            tile.GetRectTransform().sizeDelta = new Vector2(50, 50);
            SpellTileList.Add(tile);
        }
    }
    
    public void ResetEquipMenu()
    {
        
        if (WeaponTileList != null && SpellTileList != null)
        {            
            foreach(var tile in WeaponTileList)
            {
                if (tile != null)
                {
                    tile.ResetTile();
                }               
            }
            foreach(var tile in SpellTileList)
            {
                if (tile != null)
                {
                    tile.ResetTile();
                }                
            }
        }
    }

    public float GetArmorBlockPct()
    {
        if (isArmorAdded && equippedArmorType != ArmorTypes.None)
        {
            return GameController.GetArmorSOFromList(equippedArmorType).BlockPercentage;
        }
        else return 0;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~~~

    private void Tile_OnPlacedOnWeaponMenu(PlacedObject e, int num)
    {
        if (e.GetItemType() == ItemTypes.Armor)
        {
            isArmorAdded = true;
            equippedArmorType = e.GetInventoryItemSO().ArmorType;
        }
    }

    private void Tile_OnRemovedFromWeaponMenu(PlacedObject e, int num)
    {
        if (e.GetItemType() == ItemTypes.Armor)
        {
            isArmorAdded = false;
            equippedArmorType = ArmorTypes.None;
        }
    }

    private void Tile_OnPlacedOnSpellMenu(PlacedObject e, int num)
    {

    }

    private void Tile_OnRemovedFromSpellMenu(PlacedObject e, int num)
    {

    }
}
