﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public interface IAttack
{
    int DamageAmount { get; }
    float AttackSpeed { get; }
    float AttackRange { get; }
    float CritChance { get; }
    float CritBonus { get; }
}

public interface Input
{
    float GetAttackHold();
    Vector2 GetMousePos();
    Vector2 GetMovement();
    float GetJump();
    float GetWeapon1();
    float GetWeapon2();
    float GetWeapon3();
    bool GetInventory { get; set; }
    bool RotateItems { get; set; }
}

public interface IHeapItem<HeapObject> : IComparable<HeapObject>
{
    int HeapIndex { get; set; }
    bool IsEqual(HeapObject other);
}

public interface IState
{
    void Tick();
    void OnEnter();
    void OnExit();
}