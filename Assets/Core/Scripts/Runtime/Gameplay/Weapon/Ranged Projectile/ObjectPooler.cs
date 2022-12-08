using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class ObjectPooler : Singleton<ObjectPooler>
{
    #region Variables

    [SerializeField] private int PoolSize;
    [SerializeField] private List<AssetPackages> AssetRefList = new List<AssetPackages>();
    private List<Projectile> projectiles;
    private bool isInitialized;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
    }


    private void Start()
    {
        InistializePoolSystem();
        AssetLoader.OnGOCreatedWithAssetRef += AssetRefLoader_OnGOCreated;
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        AssetLoader.OnSceneUnload += AssetLoader_OnSceneUnload;
        AssetLoader.OnSingleSceneLoad += AssetLoader_OnSingleSceneLoad;
    }

    private void OnDisable()
    {
        AssetLoader.OnGOCreatedWithAssetRef -= AssetRefLoader_OnGOCreated;
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        AssetLoader.OnSceneUnload += AssetLoader_OnSceneUnload;
        AssetLoader.OnSingleSceneLoad -= AssetLoader_OnSingleSceneLoad;
    }

    #endregion

    #region Mechanics

    private void InistializePoolSystem()
    {
        if (!isInitialized)
        {
            projectiles = new List<Projectile>();
            InitializePool(ProjectileTypes.FireBall);
            InitializePool(ProjectileTypes.FireBallSpell);
            InitializePool(ProjectileTypes.EnemyFireBall);
            isInitialized = true;
        }        
    }

    private AssetReference GetProjectileAssetRef(ProjectileTypes projectileType)
    {
        if (AssetRefList.Count > 0)
        {
            foreach (var asset in AssetRefList)
            {
                if (asset.impactTypes == ImpactTypes.None && asset.projectileTypes != ProjectileTypes.None)
                {
                    if (asset.projectileTypes == projectileType)
                    {
                        return asset.Reference;
                    }
                }
            }
        }
        return null;
    }

    private AssetReference GetImpactAssetRef(ImpactTypes impactType)
    {
        if (AssetRefList.Count > 0)
        {
            foreach (var asset in AssetRefList)
            {
                if (asset.projectileTypes == ProjectileTypes.None && asset.impactTypes != ImpactTypes.None)
                {
                    if (asset.impactTypes == impactType)
                    {
                        return asset.Reference;
                    }
                }
            }
            return null;
        }
        else return null;
    }

    public void InitializePool(ProjectileTypes projectileType)
    {
        var assetRef = GetProjectileAssetRef(projectileType);
        if (assetRef != null)
        {
            for (int i = 0; i < PoolSize; i++)
            {
                AssetLoader.CreateGOAsset(assetRef);              
            }
        }      
    }

    public void ReturnToPool(Projectile _projectile)
    {
        _projectile.gameObject.SetActive(false);
        projectiles.Add(_projectile);
    }

    public int GetObjectCount()
    {
        return projectiles.Count;
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
                projectiles[i].transform.SetParent(null);
                return projectiles[i];
            }
        }
        return null;
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void SceneLoader_OnNewGameStart()
    {
        InistializePoolSystem();        
    }

    private void AssetLoader_OnSceneUnload(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance obj)
    {
        isInitialized = false;
    }

    private void AssetLoader_OnSingleSceneLoad(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance obj)
    {
        InistializePoolSystem();        
    }

    private void AssetRefLoader_OnGOCreated(GameObject GO, AssetReference reference)
    {
        if (GO != null)
        {
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
            notify.AssetReference = reference;
            GO.SetActive(false);
        }
    }

    private void Notify_Destroyed(UnityEngine.AddressableAssets.AssetReference assetRef, NotifyOnDestroyByAssetRef notify)
    {
        notify.Destroyed -= Notify_Destroyed;
        AssetLoader.ReleaseAssetInstance(notify.gameObject);
    }

    #endregion
}
