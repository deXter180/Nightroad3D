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
    private static Dictionary<ProjectileTypes, GameObject> GODictProjectile = new Dictionary<ProjectileTypes, GameObject>();
    private static Dictionary<WeaponTypes, AudioClip> ACDictWeapon = new Dictionary<WeaponTypes, AudioClip>();
    private static Dictionary<string, QuestSO> QuestDict = new Dictionary<string, QuestSO>();
    private static Dictionary<string, Material> MaterailDict = new Dictionary<string, Material>();
    private static List<InventoryItemSO> InventorySOList = new List<InventoryItemSO>();
    private static List<InventoryItemSO> WeaponInventorySOList = new List<InventoryItemSO>();
    private static List<InventoryItemSO> SpellInventorySOList = new List<InventoryItemSO>();
    private static List<WeaponSO> WeaponSOList = new List<WeaponSO>();
    private static List<SpellBaseSO> spellSOList = new List<SpellBaseSO>();
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
        LoadAudioAssets("AudioFiles");
        LoadMaterialAssets("Materials");
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~ Utility Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~

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

    public static Material GetMaterail(string name)
    {
        if (MaterailDict.TryGetValue(name, out Material material))
            return material;
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

    public static QuestSO GetQeust(string name)
    {
        if (QuestDict.TryGetValue(name, out QuestSO quest))
            return quest;

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

    public static SpellBaseSO GetSpellSOFromList(SpellTypes spellType)
    {
        foreach(SpellBaseSO spellSO in spellSOList)
        {
            if (spellSO.SpellType == spellType)
            {
                return spellSO;
            }
        }
        return null;
    }

    public static InventoryItemSO GetInventoryItemSOFromList(ItemTypes itemType)
    {
        foreach (var invSO in InventorySOList)
        {
            if (invSO.ItemType == itemType)
            {
                return invSO;
            }
        }
        return null;
    }

    public static InventoryItemSO GetWeaponInventorySO(WeaponTypes WT)
    {
        foreach (var temp in WeaponInventorySOList)
        {
            if (temp.WeaponType == WT)
            {
                return temp;
            }
        }
        return null;
    }

    public static InventoryItemSO GetSpellInventorySO(SpellTypes ST)
    {
        foreach (var temp in SpellInventorySOList)
        {
            if (temp.SpellType == ST)
            {
                return temp;
            }
        }
        return null;
    }

    public static List<InventoryItemSO> GetInventoryItemsSOFromList(ItemTypes itemType)
    {
        List<InventoryItemSO> itemList = new List<InventoryItemSO>();
        foreach (var invSO in InventorySOList)
        {
            if (invSO.ItemType == itemType)
            {
                itemList.Add(invSO);
            }
        }
        return itemList;
    }

    public static GameObject GetProjectileObj(ProjectileTypes projectileType)
    {
        if (GODictProjectile.TryGetValue(projectileType, out GameObject GO))
            return GO;
        else return null;
    }

    public static AudioClip GetAudioClipByWeaponType(WeaponTypes weaponType)
    {
        if (ACDictWeapon.TryGetValue(weaponType, out AudioClip AC))
            return AC;
        else return null;
    }

    //~~~~~~~~~~~~~~~~~~~~~ Instantiate Methods ~~~~~~~~~~~~~~~~~~~~~

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
            var notify = GO.AddComponent<NotifyOnDestroyByAssetRef>();
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
            var notify = GO.AddComponent<NotifyOnDestroyByAssetRef>();
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
                var notify = GO.AddComponent<NotifyOnDestroyByAssetRef>();
                notify.Destroyed += Notify_Destroyed;
                notify.AssetReference = assetReference;
            }
            return GOList;
        }
        else return null;
    }

    //~~~~~~~~~~~~~~~~~~~~~~ Load Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private static async void LoadSOAssets(string label)
    {
        if (label == "ScriptableObject")
        {
            await AssetRefLoader.LoadedAssets<UnityEngine.Object>(label, SOLoadCallback);
        }      
    }

    private static async void LoadAudioAssets(string label)
    {
        if (label == "AudioFiles")
        {
            await AssetRefLoader.LoadedAssets<AudioClip>(label, ACLoadCallback);
        }
    }

    private static async void LoadMaterialAssets(string label)
    {
        if (label == "Materials")
        {
            await AssetRefLoader.LoadedAssets<Material>(label, MaterialLoadCallback);
        }
    }  

    //~~~~~~~~~~~~~~~~~~~~~~~ Release Methods ~~~~~~~~~~~~~~~~~~~~~~~~

    public static void ReleaseAssetInstance(GameObject obj)
    {
        if (!Addressables.ReleaseInstance(obj))
            Destroy(obj);
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
        if(obj.GetComponent<Projectile>() != null)
        {
            var pro = obj.GetComponent<Projectile>();
            GODictProjectile.Add(pro.GetProjectileType(), obj);
        }
    }

    private static void ACLoadCallback(AudioClip obj)
    {
        if (obj.name == "RifleAttackAudio")
        {
            ACDictWeapon.Add(WeaponTypes.Rifle, obj);
        }
    }

    private static void SOLoadCallback(UnityEngine.Object obj)
    {
        if (obj.GetType() == typeof(InventoryItemSO))
        {
            InventoryItemSO itemSO = (InventoryItemSO)obj;
            if (itemSO.WeaponType == WeaponTypes.None && itemSO.SpellType == SpellTypes.None)
            {
                InventorySOList.Add(itemSO);
            }
            else if (itemSO.WeaponType != WeaponTypes.None && itemSO.SpellType == SpellTypes.None)
            {
                WeaponInventorySOList.Add(itemSO);
            }
            else if (itemSO.SpellType != SpellTypes.None && itemSO.WeaponType == WeaponTypes.None)
            {
                SpellInventorySOList.Add(itemSO);
            }
            
        }
        else if (obj.GetType() == typeof(WeaponSO))
        {
            WeaponSOList.Add((WeaponSO)obj);
        }
        else if (obj.GetType() == typeof(EnemySO))
        {
            EnemySOList.Add((EnemySO)obj);
        }
        else if (obj.GetType() == typeof(SpellBaseSO))
        {
            spellSOList.Add((SpellBaseSO)obj);
        }
        else if (obj.GetType() == typeof(QuestSO))
        {
            QuestSO quest = (QuestSO)obj;
            QuestDict.Add(quest.name, quest);
        }
    }

    private static void MaterialLoadCallback(Material obj)
    {
        MaterailDict.Add(obj.name, obj);
    }

    private static void Notify_Destroyed(AssetReference assetReference, NotifyOnDestroyByAssetRef obj)
    {
        ReleaseAssetInstance(obj.gameObject, assetReference, false);
        InstantiatedGODictByAssetRef.Remove(assetReference);
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