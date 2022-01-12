using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Spell", menuName = "Spells/New")]
public class SpellBaseSO : ScriptableObject
{
    [SerializeField] private SpellTypes spellType;
    [SerializeField] private SpellCategories spellCategory;
    [SerializeField] private ProjectileTypes projectileType;
    [SerializeField] private float duration;
    [SerializeField] private float range;
    [SerializeField] private float castDelay;
    [SerializeField] private bool isQuickCast;
    [SerializeField] private int effectAmount;
    public Transform SpellPrefabs;

    public int EffectAmount => effectAmount;
    public float Duration => duration;
    public float Range => range;
    public float CastDelay => castDelay;
    public bool IsQuickCast => isQuickCast;
    public SpellTypes SpellType => spellType;
    public SpellCategories SpellCategory => spellCategory;
    public ProjectileTypes ProjectileType => projectileType;
}