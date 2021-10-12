using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/New")]
public class WeaponSO : ScriptableObject, IAttack
{
    [SerializeField] private int damageAmount;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private float critChance;
    [SerializeField] private float critBonus;
    [SerializeField] private float bloom;
    [SerializeField] private WeaponTypes weaponType;
    [SerializeField] private ImpactTypes impactType;
    [SerializeField] private bool isRanged;
    public Transform WorldPrefab;

    public int DamageAmount => damageAmount;
    public float AttackSpeed => attackSpeed;
    public float AttackRange => attackRange;
    public float CritChance => critChance;
    public float CritBonus => critBonus;
    public float Bloom => bloom;
    public bool IsRanged => isRanged;
    public WeaponTypes WeaponType => weaponType;
    public ImpactTypes ImpactType => impactType;
}
