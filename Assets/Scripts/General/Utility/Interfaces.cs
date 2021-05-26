using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public interface IAttack
{
    int DamageAmount { get; }
    float AttackSpeed { get; }
    int AttackRange { get; }
    float CritChance { get; }
    float CritBonus { get; }
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

public interface IHeapItem<HeapObject> : IComparable<HeapObject>
{
    int HeapIndex { get; set; }
}

public interface IState
{
    void Tick();
    void OnEnter();
    void OnExit();
}