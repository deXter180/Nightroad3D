﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WeaponManager : Singleton<WeaponManager>
{
    #region Variables

    [HideInInspector] public bool IsAttacking = false;
    [SerializeField] private int WeaponCount = 4;
    private int SelectedWeapon = 0;
    private WeaponBrain[] weaponBrains;
    private WeaponBrain selectedWeaponBrain;
    private bool isRemoved;
    private bool IsInitialized => weaponInventory != null;
    private PlayerInputAsset inputs;
    private PlayerController player;
    private InventorySystem inventorySystem;
    public static Dictionary<WeaponTypes, WeaponBrain> weaponInventory;
    private Dictionary<WeaponBrain, RangedWeapon> rangedWeaponDict = new Dictionary<WeaponBrain, RangedWeapon>();
    private GameController gameController;   
    public static event Action<RangedWeapon> OnRangeWeaponEquip;
    public static event Action OnMeleeWeaponEquip;
    public static event Action<WeaponTypes, int> OnAddingWeapon;
    public static event Action<WeaponTypes, int> OnRemovingWeapon;
    public static event Action OnEmptyWeapon;
    public WeaponBrain SelectedWeaponBrain => selectedWeaponBrain;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        weaponBrains = GetComponentsInChildren<WeaponBrain>(true);
    }

    private void Start()
    {
        player = PlayerController.Instance;
        gameController = GameController.Instance;
        inventorySystem = InventorySystem.Instance;
        StartCoroutine(InputDone());
        foreach (WeaponBrain WB in weaponBrains)
        {
            WB.gameObject.SetActive(false);
        }
        foreach (var menuTile in EquipMenuControl.WeaponTileList)
        {
            menuTile.OnPlacedOnWeaponMenu += MenuTile_OnPlacedOnMenu;
            menuTile.OnRemovedFromWeaponMenu += MenuTile_OnRemovedFromMenu;
        }
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        Weapons.OnPlayerReload += Weapons_OnPlayerReload;
        InitializeInventory();
        isRemoved = false;
    }

    private void OnDestroy()
    {
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        Weapons.OnPlayerReload += Weapons_OnPlayerReload;
        foreach (var menuTile in EquipMenuControl.WeaponTileList)
        {
            menuTile.OnPlacedOnWeaponMenu -= MenuTile_OnPlacedOnMenu;
            menuTile.OnRemovedFromWeaponMenu -= MenuTile_OnRemovedFromMenu;
        }
    }

    private void Update()
    {
        if (inputs != null && !player.IsPlayerDead && gameController != null)
        {
            if (!gameController.IsInventoryActive && !gameController.IsStashActive && !gameController.IsMainMenuActive)
            {
                WeaponSelect();
            }
        }           
    }

    private IEnumerator InputDone()
    {
        yield return new WaitUntil(() => InputManager.InputReady);
        inputs = InputManager.InputActions;
    }

    private void InitializeInventory()
    {
        if (IsInitialized)
            return;
        else
        weaponInventory = new Dictionary<WeaponTypes, WeaponBrain>();     
    }

    public bool InInventory(WeaponTypes weaponType)
    {
        if (IsInitialized)
        {
            if (weaponInventory.ContainsKey(weaponType))
            {
                return true;
            }
            else return false;
        }
        else return false;
    }

    public WeaponBrain GetWeaponFromInventory(WeaponTypes weaponTypes)
    {
        if (IsInitialized)
        {
            if (weaponInventory.ContainsKey(weaponTypes))
            {
                weaponInventory.TryGetValue(weaponTypes, out WeaponBrain WB);
                return WB;
            }
            else return null;
        }
        else return null;
    }

    #endregion

    #region Mechanics

    private void AddWeapon(WeaponTypes weaponType, int num)
    {
        InitializeInventory();
        if (weaponInventory.Count < WeaponCount && !weaponInventory.ContainsKey(weaponType))
        {            
            for (int i = 0; i < weaponBrains.Length; i++)
            {
                if (weaponBrains[i].GetWeaponTypes() == weaponType)
                {
                    weaponInventory.Add(weaponType, weaponBrains[i]);
                    if (weaponInventory.Count == 1)
                    {
                        weaponBrains[i].gameObject.SetActive(true);
                        selectedWeaponBrain = weaponBrains[i];
                        VerifyIfRanged(weaponBrains[i].GetThisWeapon().ThisWeaponSO, weaponBrains[i]);
                        SelectedWeapon = 0;
                    }
                    OnAddingWeapon?.Invoke(weaponType, num);
                }
            }
        }
    }

    public void RemoveWeapon(WeaponTypes weaponType, int num)
    {
        if (IsInitialized)
        {
            if (weaponInventory.Count > 0 && weaponInventory.ContainsKey(weaponType))
            {
                weaponInventory.TryGetValue(weaponType, out WeaponBrain weaponBrain);
                weaponBrain.gameObject.SetActive(false);
                selectedWeaponBrain = null;
                weaponInventory.Remove(weaponType);
                OnRemovingWeapon?.Invoke(weaponType, num);
            }
        }
    }

    private void SelectWeapon()
    {
        int i = 0;
        foreach(WeaponBrain weapon in weaponInventory.Values)
        {
            if (i == SelectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                weapon.EquipAnim();
                selectedWeaponBrain = weapon;
                VerifyIfRanged(weapon.GetThisWeapon().ThisWeaponSO, weapon);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }               
            i++;
        }
    }

    public void DisableAllWeapons()
    {
        foreach (WeaponBrain WB in weaponBrains)
        {
            WB.gameObject.SetActive(false);
        }
    }   

    public void DisableSelectedWeapon()
    {
        if (selectedWeaponBrain != null)
        {
            selectedWeaponBrain.gameObject.SetActive(false);
        }
    }

    public IEnumerator EnableSelectedWeapon(Action action)
    {
        if (selectedWeaponBrain != null)
        {
            yield return Helpers.GetWait(0.5f);
            selectedWeaponBrain.gameObject.SetActive(true);
            selectedWeaponBrain.EquipAnim();
            action.Invoke();
        }
    }

    private void WeaponSelect()
    {
        int previousWeapon = SelectedWeapon;
        if (inputs.WeaponSelection.Weapon1.triggered)
        {
            if (weaponInventory.Count >= 1)
            {
                SelectedWeapon = 0;
            }
            
        }
        if (inputs.WeaponSelection.Weapon2.triggered)
        {
            if (weaponInventory.Count >= 2)
            {
                SelectedWeapon = 1;
            }
            
        }
        if (inputs.WeaponSelection.Weapon3.triggered)
        {
            if (weaponInventory.Count >= 3)
            {
                SelectedWeapon = 2;
            }
            
        }
        if (inputs.WeaponSelection.Weapon4.triggered)
        {
            if (weaponInventory.Count >= 4)
            {
                SelectedWeapon = 3;
            }            
        }
        if (previousWeapon != SelectedWeapon)
        {
            SelectWeapon();
            IsAttacking = false;
        }
        if (isRemoved)
        {
            SelectedWeapon = 0;
            SelectWeapon();
            isRemoved = false;
        }
    }

    private void VerifyIfRanged(WeaponSO weaponSO, WeaponBrain weapon)
    {
        if (weaponSO != null && weapon != null)
        {
            if (weaponSO.IsRanged)
            {
                if (rangedWeaponDict.ContainsKey(weapon))
                {
                    OnRangeWeaponEquip?.Invoke(rangedWeaponDict[weapon]);
                }
                else
                {
                    if (weapon.TryGetComponent<RangedWeapon>(out RangedWeapon rangedWeapon))
                    {
                        rangedWeaponDict.Add(weapon, rangedWeapon);
                        OnRangeWeaponEquip?.Invoke(rangedWeapon);
                    }
                }
            }
            else
            {
                OnMeleeWeaponEquip?.Invoke();
            }
        }
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void Weapons_OnPlayerReload(object sender, OnPlayerReloadEventArg e)
    {
        if (inventorySystem == null)
        {
            inventorySystem = InventorySystem.Instance;
        }
        if (inventorySystem.GetAvailableAmmo(e.weaponType) != null)
        {
            PlacedObject placedObject = inventorySystem.GetAvailableAmmo(e.weaponType);
            int ammoAmount = AssetLoader.GetAmmoSOFromList(e.weaponType).AmmoAmountInPack;
            if (e.rangedWeapon.AddAmmo(ammoAmount))
            {
                inventorySystem.RemoveItem(placedObject);
            }
        }
    }

    private void MenuTile_OnPlacedOnMenu(PlacedObject e, int num)
    {
        if (e != null)
        {
            InventoryItemSO itemSO = e.GetInventoryItemSO();
            if (itemSO.ItemType == ItemTypes.Weapon && itemSO.WeaponType != WeaponTypes.None)
            {
                AddWeapon(itemSO.WeaponType, num);
            }
        }             
    }

    private void MenuTile_OnRemovedFromMenu(PlacedObject e, int num)
    {
        if (e != null)
        {
            InventoryItemSO itemSO = e.GetInventoryItemSO();
            if (itemSO.ItemType == ItemTypes.Weapon && itemSO.WeaponType != WeaponTypes.None)
            {                
                RemoveWeapon(itemSO.WeaponType, num);
                isRemoved = true;
                if (weaponInventory.Count < 1)
                {
                    OnEmptyWeapon?.Invoke();
                }
            }
        }       
    }

    private void SceneLoader_OnNewGameStart()
    {
        weaponInventory.Clear();
    }

    #endregion
}
