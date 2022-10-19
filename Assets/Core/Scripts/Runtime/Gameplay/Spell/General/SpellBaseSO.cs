using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


[CreateAssetMenu(fileName = "Spell", menuName = "Spells/New")]
public class SpellBaseSO : ScriptableObject
{
    #region SerializedVariables

    [SerializeField] private SpellTypes spellType;
    [SerializeField] private SpellCategories spellCategory;
    [SerializeField] private ProjectileTypes projectileType;
    [SerializeField] private SpellCastTypes castVfxType;
    [SerializeField] private float duration;
    [SerializeField] private float range;
    [SerializeField] private float castDelay;
    [SerializeField] private bool isQuickCast;
    [SerializeField] private int effectAmount;
    [SerializeField] private int manaCost;
    [SerializeField] private float statMultiplier;

    #endregion

    #region Variables

    private float modifiedValue = 0;

    #endregion

    #region Properties

    public int ManaCost => manaCost;
    public int EffectAmount => effectAmount;
    public float Duration => duration;
    public float Range => range;
    public float CastDelay => castDelay;
    public bool IsQuickCast => isQuickCast;
    public float StatMultiplier => statMultiplier;
    public SpellTypes SpellType => spellType;
    public SpellCategories SpellCategory => spellCategory;
    public ProjectileTypes ProjectileType => projectileType;
    public SpellCastTypes CastVfxType => castVfxType;

    #endregion

    #region Mechanics

    public void DoAttack(Target enemyTarget, float enemyDodgeChance)
    {
        enemyTarget.DoDamage(Mathf.FloorToInt(modifiedValue), enemyDodgeChance);
    }

    public void UpdateModifiedValue(float value)
    {
        modifiedValue = value;
    }

    #endregion
}
