using System.Collections;
using System.Collections.Generic;


public abstract class EnemyManager : IEnemyStat
{
    public abstract int id { get; set; }
    public abstract string name { get; set; } 
}

public enum EnemyTypes
{
    Giant,
    Fighter
}
