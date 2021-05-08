using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;

public static class WeaponFactory
{
    private static Dictionary<WeaponTypes, Weapons> WeaponDictionary;
    private static Dictionary<WeaponTypes, MeleeWeapns> MeleeDictionary;
    private static Dictionary<WeaponTypes, RangedWeapons> RangedDictionary;
    private static bool IsInitialized => WeaponDictionary != null;

    private static void InitializeFactory()
    {
        if (IsInitialized)
            return;

        var allWeapons = Assembly.GetAssembly(typeof(Weapons)).GetTypes()
            .Where(myTypes => myTypes.IsClass && !myTypes.IsAbstract && myTypes.IsSubclassOf(typeof(Weapons)));
        var meleeWeapons = Assembly.GetAssembly(typeof(MeleeWeapns)).GetTypes()
            .Where(myTypes => myTypes.IsClass && !myTypes.IsAbstract && myTypes.IsSubclassOf(typeof(MeleeWeapns)));
        var rangedWeapons = Assembly.GetAssembly(typeof(RangedWeapons)).GetTypes()
            .Where(myTypes => myTypes.IsClass && !myTypes.IsAbstract && myTypes.IsSubclassOf(typeof(RangedWeapons)));

        WeaponDictionary = new Dictionary<WeaponTypes, Weapons>();
        MeleeDictionary = new Dictionary<WeaponTypes, MeleeWeapns>();
        RangedDictionary = new Dictionary<WeaponTypes, RangedWeapons>();

        foreach (var type in allWeapons)
        {
            var temp = Activator.CreateInstance(type) as Weapons;
            WeaponDictionary.Add(temp.weaponTypes, temp);
        }
        foreach (var type in meleeWeapons)
        {
            var temp = Activator.CreateInstance(type) as MeleeWeapns;
            MeleeDictionary.Add(temp.weaponTypes, temp);
        }
        foreach (var type in rangedWeapons)
        {
            var temp = Activator.CreateInstance(type) as RangedWeapons;
            RangedDictionary.Add(temp.weaponTypes, temp);
        }
    }

    public static Weapons GetWeapon(WeaponTypes weaponTypes, out MeleeWeapns melee, out RangedWeapons ranged)
    {
        InitializeFactory();
        if (WeaponDictionary.ContainsKey(weaponTypes))
        {
            var weapon = WeaponDictionary[weaponTypes];
            if (MeleeDictionary.ContainsKey(weaponTypes))
            {
                melee = MeleeDictionary[weaponTypes];
                ranged = null;
            }
            else if (RangedDictionary.ContainsKey(weaponTypes))
            {
                ranged = RangedDictionary[weaponTypes];
                melee = null;
            }
            else
            {
                melee = null;
                ranged = null;
            }
            return weapon;
        }
        melee = null;
        ranged = null;
        return default;
    }
}
