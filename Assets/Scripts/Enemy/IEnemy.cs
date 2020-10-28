using System.Collections;
using System.Collections.Generic;


public interface IEnemyStat
{
    int id { get; set; }
    string name { get; set; }
}
public interface IDamage
{
    void DoDamage(int dmg);
}