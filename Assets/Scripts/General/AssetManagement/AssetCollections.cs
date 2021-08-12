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
    private static Dictionary<string, List<GameObject>> InstantiatedGODictByName = new Dictionary<string, List<GameObject>>();
    private static Dictionary<ImpactTypes, GameObject> GODictImpact = new Dictionary<ImpactTypes, GameObject>();
    private static Dictionary<ProjectileTypes, GameObject> GODictProjectile = new Dictionary<ProjectileTypes, GameObject>();
    private static List<InventoryItemSO> InventorySOList = new List<InventoryItemSO>();
    private static List<WeaponSO> WeaponSOList = new List<WeaponSO>();
    private static List<EnemySO> EnemySOList = new List<EnemySO>();

    public IList<IResourceLocation> AssetLocationList { get; } = new List<IResourceLocation>();
    

    private void Awake()
    {       
        LoadSOAssets("ScriptableObject");
        LoadGOAssets("ImpactEffect");
        LoadGOAssets("Weapon");
        StartCoroutine(FindObjectOfType<InventorySystem>().Test());
    }

    public static List<GameObject> GetGOListFromDictByName(string name)
    {
        if (InstantiatedGODictByName.TryGetValue(name, out List<GameObject> value))
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

    public static async void LoadSOAssets(string label)
    {
        if (label == "ScriptableObject")
        {
            await AssetRefLoader.LoadedAssets<UnityEngine.Object>(label, LOLoadCallback);
        }      
    }

    public static async void LoadGOAssets(string label)
    {
        if (label != "ScriptableObject")
        {
            await AssetRefLoader.LoadedAssets<GameObject>(label, GOLoadCallback);
        }
    }

    public static void ReleaseAssetInstance(GameObject obj, string label)
    {
        Addressables.ReleaseInstance(obj);
        if (InstantiatedGODictByName.ContainsKey(label))
            InstantiatedGODictByName.Remove(label);
    }

    public static void ReleaseAssetInstance(GameObject obj, ImpactTypes impactType)
    {
        Addressables.Release(obj);
        if (GODictImpact.ContainsKey(impactType))
            GODictImpact.Remove(impactType);
    }

    public static void ReleaseAssetInstance(GameObject obj, ProjectileTypes projectileType)
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

    private static void LOLoadCallback(UnityEngine.Object obj)
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
}


[Serializable]
public class AssetPackages
{
    public AssetLabelReference Label;
    public List<Vector3> PositionsList;
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
