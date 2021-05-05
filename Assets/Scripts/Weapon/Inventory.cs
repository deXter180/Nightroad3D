using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    [HideInInspector] public bool IsAttacking = false;
    private int WeaponCount => 3;
    private int SelectedWeapon = 0;
    private WeaponBrain[] weaponBrains;
    private bool IsInitialized => WeaponInventory != null;
    private Input input;
    private Dictionary<WeaponTypes, WeaponBrain> WeaponInventory;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else Instance = this;
        weaponBrains = GetComponentsInChildren<WeaponBrain>(true);
        input = GetComponentInParent<InputControl>();
    }
    private void Start()
    {
        foreach (WeaponBrain WB in weaponBrains)
        {
            WB.gameObject.SetActive(false);
        }
        InitializeInventory();
        AddWeapon(WeaponTypes.RocketLauncher);
        AddWeapon(WeaponTypes.Rifle);
        AddWeapon(WeaponTypes.Axe);
    }
    private void Update()
    {
        WeaponSelect();
    }
    private void InitializeInventory()
    {
        if (IsInitialized)
            return;
        else
        WeaponInventory = new Dictionary<WeaponTypes, WeaponBrain>();
        
    }

    public bool InInventory(WeaponTypes weaponType)
    {
        if (IsInitialized)
        {
            if (WeaponInventory.ContainsKey(weaponType))
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
            if (WeaponInventory.ContainsKey(weaponTypes))
            {
                WeaponInventory.TryGetValue(weaponTypes, out WeaponBrain WB);
                return WB;
            }
            else return null;
        }
        else return null;
    }

    public void AddWeapon(WeaponTypes weaponType)
    {
        InitializeInventory();
        if (WeaponInventory.Count < WeaponCount && !WeaponInventory.ContainsKey(weaponType))
        {
            
            for (int i = 0; i < weaponBrains.Length; i++)
            {
                if (weaponBrains[i].GetWeaponTypes() == weaponType)
                {
                    WeaponInventory.Add(weaponType, weaponBrains[i]);
                    if (WeaponInventory.Count == 1)
                    {
                        weaponBrains[i].gameObject.SetActive(true);
                        SelectedWeapon = 0;
                    }
                }
                //if (WeaponInventory.Count == 1)
                //{
                //    WeaponInventory.TryGetValue(weaponType, out WeaponBrain weaponBrain);
                //    weaponBrain.gameObject.SetActive(true);
                //    SelectedWeapon = 0;

                //}
            }
        }
    }

    public void RemoveWeapon(WeaponTypes weaponType)
    {
        if (IsInitialized)
        {
            if (WeaponInventory.Count != 0 && WeaponInventory.ContainsKey(weaponType))
            {
                WeaponInventory.Remove(weaponType);
            }
        }
    }

    private void SelectWeapon()
    {
        int i = 0;
        foreach(WeaponBrain weapon in WeaponInventory.Values)
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
    private void WeaponSelect()
    {
        int previousWeapon = SelectedWeapon;
        if (input.GetWeapon1() == 1 && WeaponInventory.Count >= 1)
        {
            SelectedWeapon = 0;
        }
        else if (input.GetWeapon2() == 1 && WeaponInventory.Count >= 2)
        {
            SelectedWeapon = 1;
        }
        else if (input.GetWeapon3() == 1 && WeaponInventory.Count >= 3)
        {
            SelectedWeapon = 2;
        }
        if (previousWeapon != SelectedWeapon)
        {
            SelectWeapon();
            IsAttacking = false;
        }
    }

}
