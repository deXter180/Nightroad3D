using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WeaponInventory : MonoBehaviour
{
    public static WeaponInventory Instance { get; private set; }
    [HideInInspector] public bool IsAttacking = false;
    private int WeaponCount => 3;
    private int SelectedWeapon = 0;
    private WeaponBrain[] weaponBrains;
    private bool IsInitialized => weaponInventory != null;
    private Input input;
    private Dictionary<WeaponTypes, WeaponBrain> weaponInventory;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else Instance = this;
        weaponBrains = GetComponentsInChildren<WeaponBrain>(true);
        input = FindObjectOfType<InputControl>();
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

    public void AddWeapon(WeaponTypes weaponType)
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
            if (weaponInventory.Count != 0 && weaponInventory.ContainsKey(weaponType))
            {
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
    private void WeaponSelect()
    {
        int previousWeapon = SelectedWeapon;
        if (input.GetWeapon1() == 1 && weaponInventory.Count >= 1)
        {
            SelectedWeapon = 0;
        }
        else if (input.GetWeapon2() == 1 && weaponInventory.Count >= 2)
        {
            SelectedWeapon = 1;
        }
        else if (input.GetWeapon3() == 1 && weaponInventory.Count >= 3)
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
