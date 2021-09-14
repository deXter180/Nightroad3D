using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AssetCollections : MonoBehaviour
{
    public static AssetCollections Instance { get; private set; }
    [SerializeField] private List<AssetPackages> AssetRefList = new List<AssetPackages>();
    private static Dictionary<string, List<GameObject>> InstantiatedGODictByName = new Dictionary<string, List<GameObject>>();
    private static Dictionary<AssetReference, List<GameObject>> InstantiatedGODictByAssetRef = new Dictionary<AssetReference, List<GameObject>>();
    private static Dictionary<ImpactTypes, GameObject> GODictImpact = new Dictionary<ImpactTypes, GameObject>();
    private static Dictionary<ProjectileTypes, GameObject> GODictProjectile = new Dictionary<ProjectileTypes, GameObject>();
    private static List<InventoryItemSO> InventorySOList = new List<InventoryItemSO>();
    private static List<WeaponSO> WeaponSOList = new List<WeaponSO>();
    private static List<EnemySO> EnemySOList = new List<EnemySO>();

    public IList<IResourceLocation> AssetLocationList { get; } = new List<IResourceLocation>();
    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(Instance);
            Instance = this;
        }
        LoadSOAssets("ScriptableObject");
        LoadGOAssets("ImpactEffect");
        LoadGOAssets("Weapon");
        StartCoroutine(FindObjectOfType<InventorySystem>().Test());
    }

    public AssetReference GetProjectileAssetRef(ProjectileTypes projectileType)
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
            return null;
        }
        else return null;
    }

    public AssetReference GetImpactAssetRef(ImpactTypes impactType)
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

    public static List<GameObject> GetGOListFromDictByName(string name)
    {
        if (InstantiatedGODictByName.TryGetValue(name, out List<GameObject> value))
            return value;
        else return null;
    }

    public static List<GameObject> GetGOListFromDictByName(AssetReference assetReference)
    {
        if (InstantiatedGODictByAssetRef.TryGetValue(assetReference, out List<GameObject> value))
            return value;
        else return null;
    }

    public static EnemySO GetEnemySOFromList(EnemyTypes enemyType)
    {
        foreach (EnemySO enemySO in EnemySOList)
        {
            if (enemySO.EnemyType == enemyType)
            {
                return enemySO;
            }
        }
        return null;
    }

    public static WeaponSO GetWeaponSOFromList(WeaponTypes weaponType)
    {
        foreach (WeaponSO weaponSO in WeaponSOList)
        {
            if (weaponSO.WeaponType == weaponType)
            {
                return weaponSO;
            }
        }
        return null;
    }

    public static InventoryItemSO GetInventoryItemSOFromList(ItemTypes itemType)
    {
        foreach (var invSO in InventorySOList)
        {
            if (invSO.itemType == itemType)
            {
                return invSO;
            }
        }
        return null;
    }

    public static GameObject GetImpactObj(ImpactTypes impactType)
    {
        if (GODictImpact.TryGetValue(impactType, out GameObject GO))
            return GO;
        else return null;
    }

    public static GameObject GetProjectileObj(ProjectileTypes projectileType)
    {
        if (GODictProjectile.TryGetValue(projectileType, out GameObject GO))
            return GO;
        else return null;
    }

    public static async void InstantiateAssetsByName(string name)
    {
        List<GameObject> GOList = new List<GameObject>();
        await AssetRefLoader.CreatedAssetsAddToList(name, GOList);
        if (!InstantiatedGODictByName.ContainsKey(name))
            InstantiatedGODictByName.Add(name, GOList);
    }

    public static async void InstantiateAssetsByName(string name, Transform Parent)
    {
        List<GameObject> GOList = new List<GameObject>();
        await AssetRefLoader.CreatedAssetsAddToList(name, GOList, Parent);
        if (!InstantiatedGODictByName.ContainsKey(name))
            InstantiatedGODictByName.Add(name, GOList);
    }

    public static async void InstantiateAssetsByAssetRef(AssetReference assetReference, Transform parent)
    {
        List<GameObject> GOList = new List<GameObject>();
        await AssetRefLoader.CreatedAssetsAddToList(assetReference, GOList, parent);
        if (!InstantiatedGODictByAssetRef.ContainsKey(assetReference))
            InstantiatedGODictByAssetRef.Add(assetReference, GOList);
        foreach (var GO in GOList)
        {
            var notify = GO.AddComponent<NotifyOnDestroy>();
            notify.Destroyed += Notify_Destroyed;
            notify.AssetReference = assetReference;
        }
    }

    public static async void InstantiateAssetsByAssetRef(AssetReference assetReference)
    {
        List<GameObject> GOList = new List<GameObject>();
        await AssetRefLoader.CreatedAssetsAddToList(assetReference, GOList);
        if (!InstantiatedGODictByAssetRef.ContainsKey(assetReference))
            InstantiatedGODictByAssetRef.Add(assetReference, GOList);
        foreach (var GO in GOList)
        {
            var notify = GO.AddComponent<NotifyOnDestroy>();
            notify.Destroyed += Notify_Destroyed;
            notify.AssetReference = assetReference;
        }
    }

    public static List<GameObject> InstantiateAndGetAssetsByAssetRef(AssetReference assetReference)
    {
        List<GameObject> GOList = new List<GameObject>();
        if (AssetRefLoader.CreatedAssetsAddToList(assetReference, GOList).IsCompleted)
        {
            if (!InstantiatedGODictByAssetRef.ContainsKey(assetReference))
                InstantiatedGODictByAssetRef.Add(assetReference, GOList);
            foreach (var GO in GOList)
            {
                var notify = GO.AddComponent<NotifyOnDestroy>();
                notify.Destroyed += Notify_Destroyed;
                notify.AssetReference = assetReference;
            }
            return GOList;
        }
        else return null;
    }

    public static async void LoadSOAssets(string label)
    {
        if (label == "ScriptableObject")
        {
            await AssetRefLoader.LoadedAssets<UnityEngine.Object>(label, SOLoadCallback);
        }      
    }

    public static async void LoadGOAssets(string label)
    {
        if (label != "ScriptableObject")
        {
            await AssetRefLoader.LoadedAssets<GameObject>(label, GOLoadCallback);
        }
    }

    public static void ReleaseAssetInstance(GameObject obj, string label, bool IsReleaseCompletely)
    {
        Addressables.ReleaseInstance(obj);
        if (InstantiatedGODictByName.ContainsKey(label))
            InstantiatedGODictByName.Remove(label);
        if (IsReleaseCompletely)
            Addressables.Release(obj);
    }

    public static void ReleaseAssetInstance(GameObject obj, AssetReference assetReference, bool IsReleaseCompletely)
    {
        Addressables.ReleaseInstance(obj);
        if (InstantiatedGODictByAssetRef.ContainsKey(assetReference))
            InstantiatedGODictByAssetRef.Remove(assetReference);
        if (IsReleaseCompletely)
            Addressables.Release(obj);
    }

    public static void ReleaseAsset(GameObject obj, ImpactTypes impactType)
    {
        Addressables.Release(obj);
        if (GODictImpact.ContainsKey(impactType))
            GODictImpact.Remove(impactType);
    }

    public static void ReleaseAsset(GameObject obj, ProjectileTypes projectileType)
    {
        Addressables.Release(obj);
        if (GODictProjectile.ContainsKey(projectileType))
            GODictProjectile.Remove(projectileType);
    }

    public static void ReleaseAnyAsset(UnityEngine.Object obj, string label)
    {
        Addressables.Release(obj);
        if (InstantiatedGODictByName.ContainsKey(label))
            InstantiatedGODictByName.Remove(label);
        else if (obj.GetType() == typeof(InventoryItemSO) && InventorySOList.Contains((InventoryItemSO)obj))
            InventorySOList.Remove((InventoryItemSO)obj);
        else if (obj.GetType() == typeof(WeaponSO) && WeaponSOList.Contains((WeaponSO)obj))
            WeaponSOList.Remove((WeaponSO)obj);
        else if (obj.GetType() == typeof(EnemySO) && EnemySOList.Contains((EnemySO)obj))
            EnemySOList.Remove((EnemySO)obj);
    }

    //~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~

    private static void GOLoadCallback(GameObject obj)
    {
        if (obj.GetComponent<ImpactEffect>() != null)
        {
            var IE = obj.GetComponent<ImpactEffect>();
            GODictImpact.Add(IE.ImpactType, obj);
        }
        else if(obj.GetComponent<Projectile>() != null)
        {
            var pro = obj.GetComponent<Projectile>();
            GODictProjectile.Add(pro.GetProjectileType(), obj);
        }
    }

    private static void SOLoadCallback(UnityEngine.Object obj)
    {
        if (obj.GetType() == typeof(InventoryItemSO))
        {
            InventorySOList.Add((InventoryItemSO)obj);
        }
        else if (obj.GetType() == typeof(WeaponSO))
        {
            WeaponSOList.Add((WeaponSO)obj);
        }
        else if (obj.GetType() == typeof(EnemySO))
        {
            EnemySOList.Add((EnemySO)obj);
        }
    }

    private static void Notify_Destroyed(AssetReference assetReference, NotifyOnDestroy obj)
    {
        ReleaseAssetInstance(obj.gameObject, assetReference, false);
        InstantiatedGODictByAssetRef.Remove(assetReference);
    }
}


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
    RocketLauncher
}
public enum WeaponCategories
{
    Melee,
    RaycastShoot,
    ProjectileShoot
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
    FireBall
}
public enum ImpactTypes
{
    None,
    BulletHole,
    FireMark
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
    ManaPotion
}
