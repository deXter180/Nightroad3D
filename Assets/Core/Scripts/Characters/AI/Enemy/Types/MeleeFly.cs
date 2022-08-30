using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeFly : MeleeEnemy
{
    #region Variables

    private Vector3 defaultPos;
    private float travelTime = 5f;
    private float attackHeight = 120f;

    #endregion

    #region General

    public MeleeFly(EnemyBrain EB) : base(EB)
    {

    }

    public override void PreAttack()
    {
        base.PreAttack();
        defaultPos = transform.position;
        Vector3 tempPos = new Vector3(transform.position.x, player.CameraTransform.position.y - attackHeight, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, tempPos, travelTime * Time.deltaTime);
    }

    public override void PostAttack()
    {
        enemyBrain.StartCoroutine(Delay()); 
        IEnumerator Delay()
        {
            yield return null;
            transform.position = Vector3.Lerp(transform.position, defaultPos, travelTime * Time.deltaTime);
            base.PostAttack();
        }
    }

    protected override void Prepare()
    {
        targetPosition = GetRandomPosition4(enemySO.PostAttackMoveRange);
        base.Prepare();
    }

    public override void HandleAttack(Target target, float dodgeChance)
    {
        DoAttack(target, dodgeChance);
        base.HandleAttack(target, dodgeChance);
    }

    #endregion
}
