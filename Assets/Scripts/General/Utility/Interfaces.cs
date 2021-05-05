using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyHP
{
    int MaxHP { get; }
}

public interface IAttack
{
    int DamageAmount { get; }
    float AttackSpeed { get; }
    int AttackRange { get; }
    float CritChance { get; }
    float CritBonus { get; }
}

public interface IMove
{
    int MoveSpeed { get; }
}

public interface Input
{
    float GetAttack();
    Vector2 GetMousePos();
    Vector2 GetMovement();
    float GetJump();
    float GetWeapon1();
    float GetWeapon2();
    float GetWeapon3();
}