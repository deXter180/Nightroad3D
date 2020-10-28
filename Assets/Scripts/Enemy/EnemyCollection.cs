using System.Collections;
using System.Collections.Generic;

public class Giant : EnemyManager
{
    EnemyTypes enemy = EnemyTypes.Giant;
    private int ID;
    private string Name;
    private int HP;
    public override int id { get => ID; set => ID = value; }
    public override string name { get => Name; set => Name = value; }

    public Giant (int ID, string Name)
    {
        id = ID;
        name = Name;
    }
}

public class Fighter : EnemyManager
{
    EnemyTypes enemy = EnemyTypes.Fighter;
    private int ID;
    private string Name;
    private int HP;
    public override int id { get => ID; set => ID = value; }
    public override string name { get => Name; set => Name = value; }

    public Fighter(int ID, string Name)
    {
        id = ID;
        name = Name;
    }
}
