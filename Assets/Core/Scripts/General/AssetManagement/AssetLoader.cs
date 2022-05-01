using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetLoader
{
    public static event Action<GameObject> OnGOCreated;
    public static event Action<GameObject, AssetReference> OnGOCreatedWithAssetRef;
    public static event Action<IList<ScriptableObject>> OnSOsLoaded;
    public static event Action<ScriptableObject> OnSOLoaded;
    public static event Action<IList<UnityEngine.Object>> OnAnyAssetLoad;
    public static event Action<SceneInstance> OnSingleSceneLoad;
    public static event Action<SceneInstance> OnLoadingNewGameInstance;
    public static event Action<SceneInstance> OnAdditiveSceneLoad;
    public static Dictionary<GameObject, AsyncOperationHandle> AsyncHandleDict = new Dictionary<GameObject, AsyncOperationHandle>();

    //~~~~~~~~~~~~~~~~~~~~~~~~ Load Assets ~~~~~~~~~~~~~~~~~~

    public static void LoadSOAssets(string nameKey, Action<ScriptableObject> callback)
    {
        Addressables.LoadAssetsAsync(nameKey, callback).Completed += (handle) => 
        {
            OnSOsLoaded?.Invoke(handle.Result);
        };
    }

    public static void LoadAnyAssets<T>(string nameKey, Action<T> callback) where T : UnityEngine.Object
    {
        Addressables.LoadAssetsAsync(nameKey, callback).Completed += (handle) =>
        {
            OnAnyAssetLoad?.Invoke(handle.Result as IList<UnityEngine.Object>);
        };
    }

    public static void LoadSOAsset(string nameKey)
    {
        Addressables.LoadAssetAsync<ScriptableObject>(nameKey).Completed += (handle) => 
        {
            OnSOLoaded?.Invoke(handle.Result);
        };
    }

    //~~~~~~~~~~~~~~~~~~~ Instantiate Gameobjects Release After Delay ~~~~~~~~~~~~~~~~~

    public static async void CreateAndReleaseAsset(string nameKey, Vector3 Position, float delayTime)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(nameKey, Position, Quaternion.identity);
        await handle.Task;
        if (handle.IsDone)
        {
            AsyncHandleDict.Add(handle.Result, handle);
            int delayInMilli = Mathf.FloorToInt(delayTime * 1000);
            await Task.Delay(delayInMilli);
            if (handle.IsValid())
            {
                AsyncHandleDict.Remove(handle.Result);
                Addressables.ReleaseInstance(handle);               
            }
        }
    }

    public static async void CreateAndReleaseAsset(string nameKey, Transform parent, float delayTime)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(nameKey, parent);
        await handle.Task;
        if (handle.IsDone)
        {
            AsyncHandleDict.Add(handle.Result, handle);
            int delayInMilli = Mathf.FloorToInt(delayTime * 1000);
            await Task.Delay(delayInMilli);
            if (handle.IsValid())
            {
                AsyncHandleDict.Remove(handle.Result);
                Addressables.ReleaseInstance(handle);
            }
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~ Instantiate Gameobjects ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public static void CreateGOAssets(string nameKey, Transform parent, List<GameObject> createdObjs)
    {
        Addressables.LoadResourceLocationsAsync(nameKey).Completed += (handle) => 
        { 
            foreach(var location in handle.Result)
            {
                Addressables.InstantiateAsync(location, parent).Completed += (opHandle) =>
                {
                    AsyncHandleDict.Add(opHandle.Result, opHandle);
                    createdObjs.Add(opHandle.Result as GameObject);
                };
            }               
        };
    }

    public static bool CreateGOAsset(string nameKey, Transform parent)
    {
        bool isCompleted = false;
        Addressables.InstantiateAsync(nameKey, parent).Completed += (handle) =>
        {
            AsyncHandleDict.Add(handle.Result, handle);
            OnGOCreated?.Invoke(handle.Result as GameObject);
            isCompleted = true;
        };
        return isCompleted;
    }

    public static bool CreateGOAsset(string nameKey, Vector3 position, Quaternion rotation)
    {
        bool isCompleted = false;
        Addressables.InstantiateAsync(nameKey, position, rotation).Completed += (handle) =>
        {
            AsyncHandleDict.Add(handle.Result, handle);
            OnGOCreated?.Invoke(handle.Result as GameObject);
            isCompleted = true;
        };
        return isCompleted;
    }

    public static bool CreateGOAsset(AssetReference reference)
    {
        bool isCompleted = false;
        reference.InstantiateAsync().Completed += (handle) => 
        {
            AsyncHandleDict.Add(handle.Result, handle);
            OnGOCreatedWithAssetRef?.Invoke(handle.Result as GameObject, reference);
            isCompleted = true;
        };
        return isCompleted;
    }

    //~~~~~~~~~~~~~~~ Release Asset ~~~~~~~~~~~~~~

    public static void ReleaseAssetInstance(GameObject gameObject)
    {
        if (AsyncHandleDict.ContainsKey(gameObject))
        {
            AsyncHandleDict.Remove(gameObject);
        }
        Addressables.ReleaseInstance(gameObject);
    }

    //~~~~~~~~~~~~~~~~~~~~~ Load Scene ~~~~~~~~~~~~~~~~~~~~

    public static void LoadAddressableLevelAdditive(string nameKey, SceneInstance previousScene, bool clearPreviousScene)
    {
        if (clearPreviousScene)
        { 
            Addressables.UnloadSceneAsync(previousScene).Completed += (asyncHandle) =>
            {
                clearPreviousScene = false;
                previousScene = new SceneInstance();
            };
        }
        Addressables.LoadSceneAsync(nameKey, UnityEngine.SceneManagement.LoadSceneMode.Additive).Completed += (asyncHandle) =>
        {
            clearPreviousScene = true;
            previousScene = asyncHandle.Result;
            OnAdditiveSceneLoad?.Invoke(previousScene);
        };
    }

    public static void LoadAddressableLevelSingle(string nameKey, SceneInstance previousScene, bool clearPreviousScene)
    {
        if (clearPreviousScene)
        {
            Addressables.UnloadSceneAsync(previousScene).Completed += (asyncHandle) =>
            {
                clearPreviousScene = false;
                previousScene = new SceneInstance();
            };
        }
        Addressables.LoadSceneAsync(nameKey, UnityEngine.SceneManagement.LoadSceneMode.Single, false).Completed += (handle) =>
        {
            clearPreviousScene = true;
            previousScene = handle.Result;
            OnSingleSceneLoad?.Invoke(previousScene);
        };
    }

    public static void LoadFreshGameInstance(string nameKey)
    {
        Addressables.LoadSceneAsync(nameKey, UnityEngine.SceneManagement.LoadSceneMode.Single, false).Completed += (handle) =>
        {
            OnLoadingNewGameInstance?.Invoke(handle.Result);
        };
    }

    public static void UnloadAddressableLevel(SceneInstance previousScene, bool clearPreviousScene)
    {
        Addressables.UnloadSceneAsync(previousScene).Completed += (asyncHandle) => 
        {
            clearPreviousScene = false;
            previousScene = new SceneInstance();
        };
    }
}

//~~~~~~~~~~~~~~~~~~~ Essential Enums ~~~~~~~~~~~~~~~~~~~~~~

#region

[Serializable]
public class AssetPackages
{
    public AssetReference Reference;
    public ProjectileTypes projectileTypes;
    public ImpactTypes impactTypes;
}

public enum WeaponTypes
{
    None,
    Axe,
    Rifle,
    RocketLauncher,
    Shotgun
}

public enum WeaponCategories
{
    Melee,
    RaycastShoot,
    ProjectileShoot
}

public enum SpellTypes
{
    None,
    FireBlast,
    FreezeBlast,
    ElectricBlast,
    Dash,
    FireBall,
    IceSpike,
    ElectricStorm
}

public enum SpellCastTypes
{
    None,
    Basic
}

public enum SpellCategories
{
    None,
    SingleTargetedProjectile,
    SingleTargetedRaycast,
    AOETargeted,
    SelfTargeted
}

public enum EnemyTypes
{
    None,
    Giant,
    Fighter
}

public enum ProjectileTypes
{
    None,
    Bullet,
    FireBall,
    FireBallSpell,
    IceSpike
}

public enum ImpactTypes
{
    None,
    BulletHole,
    FireMark,
    ShotgunHit
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic
}

public enum ItemTypes
{
    None,
    Ammo,
    HealthPotion,
    Grenade,
    Armor,
    Gloves,
    Shield,
    Boots,
    Helmet,
    ManaPotion,
    Weapon,
    Spell
}

public enum GoalTypes
{
    Assassination,
    MassKillType,
    MassKillAny,
    Collect,
    Delivery
}

#endregion