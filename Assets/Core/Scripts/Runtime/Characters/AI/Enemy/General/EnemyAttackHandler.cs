using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHandler : MonoBehaviour
{
    private string playerName = "Player";
    private PlayerController player;
    private EnemyBrain enemyBrain;

    void Start()
    {
        enemyBrain = GetComponentInParent<EnemyBrain>();
        player = PlayerController.Instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enemyBrain.GetThisEnemy().IsAttacking)
        {
            if (collision.collider.CompareTag(playerName))
            {
                PerformAttack();
            }
        }       
    }

    private void PerformAttack()
    {
        enemyBrain.GetThisEnemy().HandleAttack(player.PlayerTarget, player.DodgeChace);
        if (enemyBrain.GetThisEnemy().GetType() == typeof(MeleeGround))
        {
            player.ApplyKnockback();            
        }
    }
}
