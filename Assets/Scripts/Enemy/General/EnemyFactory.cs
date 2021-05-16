using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public static class EnemyFactory
{
    private static Dictionary<EnemyTypes, Enemy> EnemyDictionary;
    private static bool IsInitialized => EnemyDictionary != null;
    private static void InitializeFactory(EnemyBrain enemyBrain)
    {
        if (IsInitialized)
            return;

        var allEnemy = Assembly.GetAssembly(typeof(Enemy)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Enemy)));

        EnemyDictionary = new Dictionary<EnemyTypes, Enemy>();
        Type[] types = { typeof(EnemyBrain) };
        foreach (var type in allEnemy)
        {
            ConstructorInfo ctor = type.GetConstructor(types);
            ObjectCreator.ObjectActivator<Enemy> temp = ObjectCreator.GetActivator<Enemy>(ctor);
            Enemy instance = temp(enemyBrain);
            EnemyDictionary.Add(instance.enemyTypes, instance);
        }
    }

    public static Enemy GetEnemy(EnemyTypes enemyType, EnemyBrain enemyBrain)
    {
        InitializeFactory(enemyBrain);

        if (EnemyDictionary.ContainsKey(enemyType))
        {
            var enemy = EnemyDictionary[enemyType];
            return enemy;
        }
        else return null;
    }
}
