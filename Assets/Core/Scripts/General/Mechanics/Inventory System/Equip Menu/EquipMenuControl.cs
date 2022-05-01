using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMenuControl : Singleton<EquipMenuControl>
{
    public static List<EquipMenuWeaponTile> WeaponTileList = new List<EquipMenuWeaponTile>();
    public static List<EquipMenuSpellTile> SpellTileList = new List<EquipMenuSpellTile>();

    protected override void Awake()
    {
        base.Awake();
        InitializeEquipMenu();
    }

    private void OnEnable()
    {
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
    }

    private void OnDisable()
    {
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
    }

    private void InitializeEquipMenu()
    {
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

    private void ResetEquipMenu()
    {
        if (WeaponTileList != null && SpellTileList != null)
        {
            WeaponTileList.Clear();
            SpellTileList.Clear();
            InitializeEquipMenu();
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

    //~~~~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~~~

    private void SceneLoader_OnNewGameStart()
    {
        ResetEquipMenu();
    }
}
