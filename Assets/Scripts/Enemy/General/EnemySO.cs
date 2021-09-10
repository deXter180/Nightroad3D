using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/New")]
public class EnemySO : ScriptableObject, IAttack
{
    [SerializeField] private int maxHP;
    [SerializeField] private int damageAmount;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private float critChance;
    [SerializeField] private float critBonus;
    [SerializeField] private EnemyTypes enemyType;
    [SerializeField] private float dodgeChance;
    [SerializeField] private bool isGroundUnit;
    [SerializeField] private float avoidanceRadius;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float speedMultiplier;
    public Transform WorldPrefab;

    public int MaxHP => maxHP;
    public int DamageAmount => damageAmount;
    public float AttackSpeed => attackSpeed;
    public float AttackRange => attackRange;
    public float CritChance => critChance;
    public float CritBonus => critBonus;
    public EnemyTypes EnemyType => enemyType;
    public float DodgeChance => dodgeChance;
    public bool IsGroundUnit => isGroundUnit;
    public float AvoidanceRadius => avoidanceRadius;
    public float MoveSpeed => moveSpeed;
    public float SpeedMultiplier => speedMultiplier;
}
