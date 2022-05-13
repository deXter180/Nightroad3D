using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo", menuName = "Ammos/New")]
public class AmmoSO : ScriptableObject
{
    [SerializeField] private int ammoAmountInPack;
    [SerializeField] private WeaponTypes weaponType;

    public int AmmoAmountInPack => ammoAmountInPack;
    public WeaponTypes WeaponType => weaponType;
}
