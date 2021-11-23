using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class ObjectPooler : MonoBehaviour
{
    private List<Projectile> projectiles = new List<Projectile>();  
    [SerializeField] private int PoolSize;
    private List<AssetReference> RefList = new List<AssetReference>();

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
        var assetRef = AssetCollections.Instance.GetProjectileAssetRef(projectileType);
        if (assetRef != null)
        {
            RefList.Add(assetRef);
            for (int i = 0; i < PoolSize; i++)
            {
                AssetRefLoader.InstantiateGO(assetRef).Completed += ObjectPooler_Completed;
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

    //~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~

    private void ObjectPooler_Completed(AsyncOperationHandle obj)
    {
        GameObject GO = obj.Result as GameObject;
        var notify = GO.GetComponent<NotifyOnDestroyByAssetRef>();
        Projectile projectile = GO.GetComponent<Projectile>();
        if (notify == null)
        {
            notify = GO.AddComponent<NotifyOnDestroyByAssetRef>();
        }
        if (projectile != null)
        {
            projectiles.Add(projectile);
        }
        notify.Destroyed += Notify_Destroyed;
        var assetRef = AssetCollections.Instance.GetProjectileAssetRef(projectile.GetProjectileType());
        notify.AssetReference = assetRef;
        GO.SetActive(false);
    }

    private void Notify_Destroyed(UnityEngine.AddressableAssets.AssetReference assetRef, NotifyOnDestroyByAssetRef notify)
    {
        notify.Destroyed -= Notify_Destroyed;
        AssetCollections.ReleaseAssetInstance(notify.gameObject, assetRef, false);
    }

}
