using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }
    [HideInInspector] public bool IsAttacking = false;
    [SerializeField] private int WeaponCount = 4;
    private int SelectedWeapon = 0;
    private WeaponBrain[] weaponBrains;
    private bool isRemoved;
    private bool IsInitialized => weaponInventory != null;
    private PlayerInputAsset inputs;
    private PlayerController player;
    private Dictionary<WeaponTypes, WeaponBrain> weaponInventory;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else Instance = this;
        weaponBrains = GetComponentsInChildren<WeaponBrain>(true);
    }

    private void Start()
    {
        player = PlayerController.Instance;
        StartCoroutine(InputDone());
        foreach (WeaponBrain WB in weaponBrains)
        {
            WB.gameObject.SetActive(false);
        }
        foreach (var menuTile in EquipMenuControl.EquipTileList)
        {
            menuTile.OnPlacedOnMenu += MenuTile_OnPlacedOnMenu;
            menuTile.OnRemovedFromMenu += MenuTile_OnRemovedFromMenu;
        }
        InitializeInventory();
        isRemoved = false;
    }

    private void OnDestroy()
    {
        foreach (var menuTile in EquipMenuControl.EquipTileList)
        {
            menuTile.OnPlacedOnMenu -= MenuTile_OnPlacedOnMenu;
            menuTile.OnRemovedFromMenu -= MenuTile_OnRemovedFromMenu;
        }
    }

    private void Update()
    {       
        if (inputs != null && !player.IsPlayerDead)
        {
            if (!SpellManager.Instance.IsCastingSpell && !InputMenuUIHandler.Instance.IsMainMenuActive)
            {
                ControlInventory();
                WeaponSelect();
            }
        }           
    }

    private IEnumerator InputDone()
    {
        yield return new WaitUntil(() => InputManager.InputReady);
        inputs = InputManager.InputActions;
    }

    private void ControlInventory()
    {
        if (inputs.BasicControls.Inventory.triggered && !SpellManager.Instance.IsCastingSpell)
        {
            InventoryUIHandler.Instance.Control();
            if (InventoryUIHandler.Instance.IsInventoryActive)
            {
                InventoryUIHandler.Instance.IsInventoryActive = false;
            }
            else InventoryUIHandler.Instance.IsInventoryActive = true;
        }
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

    private void AddWeapon(WeaponTypes weaponType)
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
                        SelectedWeapon = 0;
                    }
                }
            }
        }
    }

    public void RemoveWeapon(WeaponTypes weaponType)
    {
        if (IsInitialized)
        {
            if (weaponInventory.Count > 0 && weaponInventory.ContainsKey(weaponType))
            {
                weaponInventory.TryGetValue(weaponType, out WeaponBrain weaponBrain);
                weaponBrain.gameObject.SetActive(false);
                weaponInventory.Remove(weaponType);
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
            }
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }

    private void DisableAllWeapons()
    {
        foreach (WeaponBrain WB in weaponBrains)
        {
            WB.gameObject.SetActive(false);
        }
    }   

    private void WeaponSelect()
    {
        int previousWeapon = SelectedWeapon;
        if (inputs.WeaponSelection.Weapon1.triggered && weaponInventory.Count >= 1)
        {
            SelectedWeapon = 0;
        }
        else if (inputs.WeaponSelection.Weapon2.triggered && weaponInventory.Count >= 2)
        {
            SelectedWeapon = 1;
        }
        else if (inputs.WeaponSelection.Weapon3.triggered && weaponInventory.Count >= 3)
        {
            SelectedWeapon = 2;
        }
        else if (inputs.WeaponSelection.Weapon4.triggered && weaponInventory.Count >= 4)
        {
            SelectedWeapon = 3;
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

    //~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~

    private void MenuTile_OnPlacedOnMenu(object sender, PlacedObject e)
    {
        InventoryItemSO itemSO = e.GetInventoryItemSO();
        if (itemSO.itemType == ItemTypes.Weapon && itemSO.weaponType != WeaponTypes.None)
        {
            AddWeapon(itemSO.weaponType);
        }       
    }

    private void MenuTile_OnRemovedFromMenu(object sender, PlacedObject e)
    {
        InventoryItemSO itemSO = e.GetInventoryItemSO();
        if (itemSO.itemType == ItemTypes.Weapon && itemSO.weaponType != WeaponTypes.None)
        {
            RemoveWeapon(itemSO.weaponType);
            isRemoved = true;
        }
    }

}
