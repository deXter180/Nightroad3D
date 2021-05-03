using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable] public struct ObjectPoolItems
{
    public Projectile ProjectilePrefab;
    public int PoolSize;
    //public bool Expandable;
}

public class ObjectPooler : MonoBehaviour
{
    public List<ObjectPoolItems> itemsToPool = new List<ObjectPoolItems>();
    private List<Projectile> projectiles;
    

    #region Singleton
    public static ObjectPooler Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion


    private void Start()
    {
        projectiles = new List<Projectile>();
        foreach (ObjectPoolItems item in itemsToPool)
        {
            for (int i = 0; i < item.PoolSize; i++)
            {
                Projectile obj = Instantiate(item.ProjectilePrefab);
                obj.gameObject.SetActive(false);
                projectiles.Add(obj);
            }
        }
    }
    public void GetObjectCount()
    {
        Debug.Log(projectiles.Count);
    }
    public Projectile GetPooledObject(ProjectileTypes projectileType)
    {
        if (projectiles.Count == 0)
        {
            InitializePool(projectileType);
        }
        for (int i = 0; i < projectiles.Count; i++)
        {
            if (!projectiles[i].gameObject.activeInHierarchy && projectiles[i].GetProjectileType() == projectileType)
            {
                return projectiles[i];
            }
        }
        return null;
        //foreach (ObjectPoolItems item in itemsToPool)
        //{
        //    if (item.ProjectilePrefab.projectile == projectileType)
        //    {
        //        return projectiles.Dequeue();
        //    }
        //}


        //foreach (ObjectPoolItems item in itemsToPool)
        //{
        //    if (item.ProjectilePrefab.projectile == projectileType)
        //    {
        //        if (item.Expandable)
        //        {
        //            Projectile _projectile = Instantiate(item.ProjectilePrefab);
        //            _projectile.gameObject.SetActive(false);
        //            projectiles.Add(_projectile);
        //            return _projectile;
        //        }
        //    }
        //}

    }

    public void InitializePool(ProjectileTypes projectile)
    {
        foreach (ObjectPoolItems item in itemsToPool)
        {
            if (item.ProjectilePrefab.GetProjectileType() == projectile)
            {
                for (int i = 0; i < item.PoolSize; i++)
                {
                    Projectile _projectile = Instantiate(item.ProjectilePrefab);
                    _projectile.gameObject.SetActive(false);
                    projectiles.Add(_projectile);
                }
            }
        }

    }

    public void ReturnToPool(Projectile _projectile)
    {
        _projectile.gameObject.SetActive(false);
        projectiles.Add(_projectile);
    }

   
}
