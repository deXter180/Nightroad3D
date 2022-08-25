using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/New")]
public class WeaponSO : ScriptableObject, IAttack
{
    [SerializeField] private int damageAmount;
    [SerializeField] private int ammoPerMagazine;
    [SerializeField] private int totalAmmo;   
    [SerializeField] [Tooltip("Not for Automatic weapons")] private float attackDelay;
    [SerializeField] private float impactForce;
    [SerializeField] private float attackRange;
    [SerializeField] private float critChance;
    [SerializeField] private float critBonus;
    [SerializeField] private float bloom;
    [SerializeField] private float statMultiplier;
    [SerializeField] private WeaponTypes weaponType;
    [SerializeField] private ImpactTypes impactType;
    [SerializeField] private bool isRanged;
    [SerializeField] private Transform worldPrefab;


    public int DamageAmount => damageAmount;
    public int AmmoPerMagazine => ammoPerMagazine;
    public int TotalAmmo => totalAmmo;
    public float AttackDelay => attackDelay;
    public float AttackRange => attackRange;
    public float ImpactForce => impactForce;
    public float CritChance => critChance;
    public float CritBonus => critBonus;
    public float Bloom => bloom;
    public bool IsRanged => isRanged;
    public float StatMultiplier => statMultiplier;
    public WeaponTypes WeaponType => weaponType;
    public ImpactTypes ImpactType => impactType;
    public Transform WorldPrefab => worldPrefab;
}
