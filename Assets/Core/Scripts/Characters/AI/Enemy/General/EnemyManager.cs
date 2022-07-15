using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;


public class Enemy
{
    protected Transform transform;
    private Animator animator;
    private Target target;
    private bool IsPreping;
    protected bool IsResetAfterAttack;
    protected bool IsPlayerApproaching;
    protected EnemyTypes enemyType;
    protected float defaultSpeed;
    protected int pathIndex = 0;
    protected List<Vector3> path;
    protected EnemyBrain enemyBrain;
    protected EnemySO enemySO;
    protected NavMeshAgent navAgent;
    protected Rigidbody RBody;
    protected PlayerController player;
    protected float attackTimer;
    protected Vector3 targetPosition;
    public bool IsTargetInRange { get; private set; }
    public bool IsAttacking { get; private set; }   
    public bool IsPrepDone { get; set; }
    public event EventHandler<OnEnemyAttackEventArg> OnEnemyAttack;
    public event EventHandler<OnEnemyDamageEventArg> OnEnemyDamage;

    public Enemy(EnemyBrain EB)
    {
        enemySO = EB.ThisEnemySO;
        this.enemyBrain = EB;
        this.RBody = EB.EnemyRigidbody;
        this.navAgent = EB.navMeshAgent;
        this.transform = EB.transform;
        this.target = EB.EnemyTarget;
        this.animator = EB.EnemyAnimator;
        this.enemyType = EB.GetEnemyType();
        player = PlayerController.Instance;
        IsPreping = false;
        IsTargetInRange = false;
        IsAttacking = false;
        IsPrepDone = false;
        IsResetAfterAttack = false;
        IsPlayerApproaching = false;
        attackTimer = 0;
        defaultSpeed = navAgent.speed;
    }

    public virtual void PreAttack()
    {
        navAgent.enabled = false;
        IsAttacking = false;
    }

    public virtual void PostAttack()
    {
        navAgent.enabled = true;
        IsAttacking = false;
    }

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

    public virtual void Roam()
    {
        if (navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
        {
            navAgent.speed = defaultSpeed;
            if (navAgent.remainingDistance <= 5f)
            {
                pathIndex++;
                if (pathIndex >= path.Count)
                {
                    pathIndex = 0;
                }
                if (navAgent.SetDestination(path[pathIndex]))
                {

                }
                else
                {
                    
                }
            }
        }       
    }

    public virtual void Chase()
    {
        if (navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
        {
            navAgent.speed *= enemySO.SpeedMultiplier;
            if (!navAgent.SetDestination(player.PlayerTransform.position))
            {
                navAgent.isStopped = true;
            }
            else
            {
                navAgent.isStopped = false;
            }
        }         
    }

    public virtual void SetupRoam(List<Vector3> newPath)
    {
        pathIndex = 0;
        path = newPath;
    }

    public void InitializeAttack(Action action)
    {
        if (player.PlayerTarget != null && !player.PlayerTarget.IsDead && !player.PlayerTarget.GetEnemy())
        {
            if (Time.time - attackTimer >= enemySO.AttackDelay)
            {               
                if (!IsAttacking)
                {   
                    if (enemySO.IsRanged)
                    {
                        enemyBrain.SetAnimIndex(0);
                    }
                    attackTimer = Time.time;
                    IsAttacking = true;
                    action.Invoke();
                }
            }
            else
            {
                IsAttacking = false;
            }
        }  
    }

    public virtual void HandleAttack(Target target, float dodgeChance)
    {
        OnEnemyAttack?.Invoke(this, new OnEnemyAttackEventArg(enemyType));
    }

    public void DoAttack(Target enemyTarget, float enemyDodgeChance)
    {
        int damageAmount = enemySO.DamageAmount - Mathf.RoundToInt((enemySO.DamageAmount * enemyBrain.MenuControl.GetArmorBlockPct()) / 100);
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

    protected Vector3 GetRandomPosition1(float range)
    {      
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1)).normalized;
        return transform.position + randomDir * UnityEngine.Random.Range(range, 2f * range);
    }

    protected Vector3 GetRandomPosition2(float range)
    {
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1)).normalized;
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

    public void DoPreparation()
    {
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
        if (Vector3.Distance(transform.position, targetPosition) > navAgent.stoppingDistance)
        {
            if (navAgent.SetDestination(targetPosition))
            {
                IsPreping = true;
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
}

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

    public OnEnemyAttackEventArg(EnemyTypes ET)
    {
        enemyType = ET;
    }
}


