using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;


public class Enemy
{
    #region Variables

    protected Transform transform;
    private Animator animator;
    private Target target;
    private bool IsPreping;
    protected bool IsResetAfterAttack;
    protected bool IsPlayerApproaching;
    protected EnemyTypes enemyType;
    protected AttackTypes attackType;
    protected float defaultSpeed;
    protected int pathIndex = 0;
    protected List<Vector3> path;
    protected EnemyBrain enemyBrain;
    protected EnemySO enemySO;
    protected NavMeshAgent navAgent;
    protected Rigidbody RBody;
    protected PlayerController player;
    protected EquipMenuControl menuControl;
    protected float attackTimer;
    protected Vector3 targetPosition;
    public bool IsTargetInRange { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsPrepDone { get; set; }
    public event EventHandler<OnEnemyAttackEventArg> OnEnemyAttack;
    public event EventHandler<OnEnemyDamageEventArg> OnEnemyDamage;

    #endregion

    #region General

    public Enemy(EnemyBrain EB)
    {
        enemySO = EB.ThisEnemySO;
        this.enemyBrain = EB;
        this.navAgent = EB.navMeshAgent;
        this.transform = EB.transform;
        this.target = EB.EnemyTarget;
        this.animator = EB.EnemyAnimator;
        this.enemyType = EB.EnemyType;
        player = PlayerController.Instance;
        menuControl = EquipMenuControl.Instance;
        IsPreping = false;
        IsTargetInRange = false;
        IsAttacking = false;
        IsPrepDone = false;
        IsResetAfterAttack = false;
        IsPlayerApproaching = false;
        attackTimer = 0;
        defaultSpeed = navAgent.speed;
    }

    #endregion

    #region Roam

    public virtual void Roam()
    {
        if (navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
        {
            if (navAgent.remainingDistance <= 5f)
            {
                pathIndex++;
                if (pathIndex >= path.Count)
                {
                    pathIndex = 0;
                }
                navAgent.SetDestination(path[pathIndex]);
            }
        }
    }

    public virtual void SetupRoam(List<Vector3> newPath)
    {
        pathIndex = 0;
        path = newPath;
    }

    #endregion

    #region Chase

    public virtual void Chase()
    {
        if (navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
        {
            if (targetPosition != player.PlayerTransform.position)
            {
                targetPosition = player.PlayerTransform.position;
                Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
                if (navAgent.SetDestination(targetPosition))
                {
                    navAgent.isStopped = false;
                }
                else
                {
                    navAgent.isStopped = true;
                }
            }           
        }
    }

    #endregion

    #region Prepare

    public void DoPreparation()
    {
        transform.LookAt(player.PlayerTransform.position);
        if (navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
        {
            if (!IsPreping)
            {
                Prepare();
            }
            else
            {
                CheckPrep();
            }
        }
    }

    protected virtual void Prepare()
    {        
        //transform.LookAt(player.PlayerTransform.position);
        if (Vector3.Distance(transform.position, targetPosition) > navAgent.stoppingDistance)
        {
            if (navAgent.SetDestination(targetPosition))
            {
                IsPreping = true;
                enemyBrain.CalculateAngle(targetPosition);
                enemyBrain.SetInBattle(true);
            }
        }
    }

    protected virtual void CheckPrep()
    {
        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            IsPreping = false;
            IsPrepDone = true;
        }
    }

    #endregion

    #region Attack

    public void InitializeAttack(Action action)
    {
        Vector3.Lerp(transform.position, player.PlayerTransform.position, Time.deltaTime);
        if (player.PlayerTarget != null && !player.PlayerTarget.IsDead && !player.PlayerTarget.GetEnemy())
        {
            SetAttackType();
            action.Invoke();           
        }
    }

    protected virtual void SetAttackType()
    {
        
    }

    public void ResetAttackAnim()
    {
        attackType = AttackTypes.None;
        enemyBrain.SetAttack1(false);
        enemyBrain.SetAttack2(false);
        enemyBrain.SetAttack3(false);
        enemyBrain.SetAttack4(false);
    }

    public void SetAttacking(bool isTrue)
    {
        IsAttacking = isTrue;
    }

    public virtual void PreAttack()
    {
        ResetAttackAnim();
        IsAttacking = false;
    }

    public virtual void PostAttack()
    {
        IsAttacking = false;
        ResetAttackAnim();
    }

    public virtual void HandleAttack(Target target, float dodgeChance)
    {
        OnEnemyAttack?.Invoke(this, new OnEnemyAttackEventArg(enemyType, attackType));
    }

    public void DoAttack(Target enemyTarget, float enemyDodgeChance)
    {
        int damageAmount = enemySO.DamageAmount - Mathf.RoundToInt((enemySO.DamageAmount * menuControl.GetArmorBlockPct()) / 100);
        if (UnityEngine.Random.value <= enemySO.CritChance)
        {
            enemyTarget.DoCritDamage(enemySO.CritBonus, damageAmount, enemyDodgeChance);
            OnEnemyDamage?.Invoke(this, new OnEnemyDamageEventArg(true));
        }
        else
        {
            enemyTarget.DoDamage(damageAmount, enemyDodgeChance);
            OnEnemyDamage?.Invoke(this, new OnEnemyDamageEventArg(false));
        }
    }

    #endregion

    #region Utilities

    public void CheckDistance()
    {
        float dist = Vector3.Distance(transform.position, player.PlayerTransform.position);
        if (dist <= enemySO.AttackRange)
        {
            IsTargetInRange = true;
            if (enemySO.IsRanged)
            {
                if (dist <= enemySO.AttackRange * 2 / 3)
                {
                    IsTargetInRange = false;
                    IsPlayerApproaching = true;
                }
                else
                {
                    IsPlayerApproaching = false;
                }
            }
        }
        else
        {
            IsTargetInRange = false;
        }
    }

    public float GetDistanceFromPlayer()
    {
        return Vector3.Distance(transform.position, player.PlayerTransform.position);
    }

    public Vector3 GetRandomPosition(float range1, float range2)
    {
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        return transform.position + randomDir * UnityEngine.Random.Range(range1, range2);
    }

    protected Vector3 GetRandomPosition1(float range)
    {
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        return transform.position + randomDir * UnityEngine.Random.Range(range, 2f * range);
    }

    protected Vector3 GetRandomPosition2(float range)
    {
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        return new Vector3(player.PlayerTransform.position.x, transform.position.y, player.PlayerTransform.position.z) + randomDir * UnityEngine.Random.Range(range, 2f * range);
    }

    protected Vector3 GetRandomPosition3(float range)
    {
        Vector3 dir = (transform.position - player.PlayerTransform.position).normalized;
        dir.y = 0;
        float dis = range - Vector3.Distance(transform.position, player.PlayerTransform.position);
        return transform.position + dir * dis;
    }

    protected Vector3 GetRandomPosition4(float range)
    {
        Vector3 dir = (-Vector3.forward + new Vector3(UnityEngine.Random.Range(-1, 1), 0, 0)).normalized;
        dir.y = 0;
        return transform.position + dir * UnityEngine.Random.Range(range, 2f * range);
    }

    #endregion 
}

#region EventArgs

public class OnEnemyDamageEventArg : EventArgs
{
    public bool IsCrit;

    public OnEnemyDamageEventArg(bool crit)
    {
        IsCrit = crit;
    }
}

public class OnEnemyAttackEventArg : EventArgs
{
    public EnemyTypes enemyType;
    public AttackTypes attackType;

    public OnEnemyAttackEventArg(EnemyTypes ET, AttackTypes AT)
    {
        enemyType = ET;
        attackType = AT;
    }
}

#endregion

