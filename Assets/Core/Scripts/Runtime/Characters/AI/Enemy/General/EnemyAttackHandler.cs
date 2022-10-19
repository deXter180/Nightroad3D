using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHandler : MonoBehaviour
{
    private PlayerController player;
    private EnemyBrain enemyBrain;

    void Start()
    {
        enemyBrain = GetComponentInParent<EnemyBrain>();
        player = PlayerController.Instance;
    }

    public void PerformAttack()
    {
        enemyBrain.GetThisEnemy().HandleAttack(player.PlayerTarget, player.DodgeChace);
    }
}
