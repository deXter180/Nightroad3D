using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPooler : MonoBehaviour
{
    private List<Projectile> projectiles = new List<Projectile>();  
    [SerializeField] private int PoolSize;
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
        StartCoroutine(InitializePool(ProjectileTypes.FireBall));      
    }

    public IEnumerator InitializePool(ProjectileTypes projectileType)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < PoolSize; i++)
        {
            if (AssetCollections.Instance.GetProjectileAssetRef(projectileType) != null)
            {
                List<GameObject> _projectiles = AssetCollections.InstantiateAndGetAssetsByAssetRef(AssetCollections.Instance.GetProjectileAssetRef(projectileType));     //Instantiate(AssetCollections.GetProjectileObj(projectileType));
                if (_projectiles != null && _projectiles.Count > 0)
                {
                    GameObject _projectile = _projectiles[0];
                    _projectile.gameObject.SetActive(false);
                    if (_projectile.GetComponent<Projectile>() != null)
                        projectiles.Add(_projectile.GetComponent<Projectile>());
                }                
            }            
        }
    }

    public void ReturnToPool(Projectile _projectile)
    {
        _projectile.gameObject.SetActive(false);
        projectiles.Add(_projectile);
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
    }

}
