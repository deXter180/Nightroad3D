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
        if (enemyBrain.FirepointTransform != null)
        {
            var shot = objectPooler.GetPooledObject(ProjectileTypes.EnemyFireBall);
            shot.transform.rotation = Quaternion.identity;
            shot.transform.position = enemyBrain.FirepointTransform.position;
            shot.SetEnemy(enemyBrain);
            shot.gameObject.SetActive(true);            
            base.HandleAttack(target, dodgeChance);
        }        
    }
}
