using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo", menuName = "Ammos/New")]
public class AmmoSO : ScriptableObject
{
    #region SerializedVariables

    [SerializeField] private int ammoAmountInPack;
    [SerializeField] private WeaponTypes weaponType;

    #endregion

    #region Properties

    public int AmmoAmountInPack => ammoAmountInPack;
    public WeaponTypes WeaponType => weaponType;

    #endregion
}
