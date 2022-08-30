using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Armors/New")]
public class ArmorSO : ScriptableObject
{
    #region SerializedVariables

    [SerializeField] [Range(0, 100)] private float blockPercentage;
    [SerializeField] private ArmorTypes armorType;

    #endregion

    #region Properties

    public float BlockPercentage => blockPercentage;
    public ArmorTypes ArmorType => armorType;

    #endregion
}
