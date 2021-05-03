using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//~~~~~~~~~~~~~~~~~~~~~~ Base Type ~~~~~~~~~~~~~~~~~~~~~~~~~~
public abstract class MeleeWeapns : Weapons
{
   
}

public abstract class RangedWeapons : Weapons
{
}

//~~~~~~~~~~~~~~~~~~~~~~~ Individual Type ~~~~~~~~~~~~~~~~~~~~~~~

public class Axe : MeleeWeapns
{
    public override int DamageAmount => 100;
    public override float AttackSpeed => 1.5f;
    public override int AttackRange => 100;
    public override float CritChance => 0.7f;
    public override float CritBonus => 2f;
    public override WeaponTypes weaponTypes => WeaponTypes.Axe;
}
public class Rifle : RangedWeapons
{
    public override int DamageAmount => 150;
    public override float AttackSpeed => 0.2f;
    public override int AttackRange => 500;
    public override float CritChance => 0.1f;
    public override float CritBonus => 2f;
    public override WeaponTypes weaponTypes => WeaponTypes.Rifle;
}
public class RPG : RangedWeapons
{
    public override int DamageAmount => 500;
    public override float AttackSpeed => 0.8f;
    public override int AttackRange => 800;
    public override float CritChance => 0.6f;
    public override float CritBonus => 1.5f;
    public override WeaponTypes weaponTypes => WeaponTypes.RocketLauncher;
}
