using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;

public static class WeaponFactory
{
    private static Dictionary<WeaponTypes, Type> WeaponDictionary;
    private static bool IsInitialized => WeaponDictionary != null;

    private static void InitializeFactory()
    {
        if (IsInitialized)
            return;

        var allWeapons = Assembly.GetAssembly(typeof(Weapons)).GetTypes()
            .Where(myTypes => myTypes.IsClass && !myTypes.IsAbstract && myTypes.IsSubclassOf(typeof(Weapons)));

        WeaponDictionary = new Dictionary<WeaponTypes, Type>();

        foreach (var type in allWeapons)
        {
            var temp = Activator.CreateInstance(type) as Weapons;
            WeaponDictionary.Add(temp.weaponTypes, type);
        }
    }

    public static Weapons GetWeapon(WeaponTypes weaponTypes)
    {
        InitializeFactory();
        if (WeaponDictionary.ContainsKey(weaponTypes))
        {
            Type type = WeaponDictionary[weaponTypes];
            var weapon = Activator.CreateInstance(type) as Weapons;
            return weapon;
        }
        return default;
    }

}
