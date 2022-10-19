using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    protected ObjectPooler objectPooler;

    public RangedEnemy(EnemyBrain EB) : base(EB)
    {
        objectPooler = ObjectPooler.Instance;
    }

    public override void HandleAttack(Target target, float dodgeChance)
    {
        if (enemyBrain.Firepoint != null)
        {
            var shot = objectPooler.GetPooledObject(ProjectileTypes.EnemyFireBall);
            shot.transform.rotation = Quaternion.identity;
            shot.transform.position = enemyBrain.Firepoint.position;
            shot.SetEnemy(enemyBrain);
            shot.gameObject.SetActive(true);            
            base.HandleAttack(target, dodgeChance);
        }        
    }
}
