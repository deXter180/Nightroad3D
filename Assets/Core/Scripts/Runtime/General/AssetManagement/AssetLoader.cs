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
    #region Variables

    private static Dictionary<string, QuestSO> QuestDict = new Dictionary<string, QuestSO>();
    private static Dictionary<string, Material> MaterailDict = new Dictionary<string, Material>();
    private static Dictionary<AudioTypes, AudioSO> AudioSODict = new Dictionary<AudioTypes, AudioSO>();
    private static List<InventoryItemSO> InventorySOList = new List<InventoryItemSO>();
    private static List<InventoryItemSO> WeaponInventorySOList = new List<InventoryItemSO>();
    private static List<InventoryItemSO> SpellInventorySOList = new List<InventoryItemSO>();
    private static List<InventoryItemSO> ArmorInventorySOList = new List<InventoryItemSO>();
    private static List<WeaponSO> WeaponSOList = new List<WeaponSO>();
    private static List<AmmoSO> AmmoSOList = new List<AmmoSO>();
    private static List<ArmorSO> ArmorSOList = new List<ArmorSO>();
    private static List<SpellBaseSO> spellSOList = new List<SpellBaseSO>();
    private static List<EnemySO> EnemySOList = new List<EnemySO>();
    private static List<RecipeSO> RecipeSOList = new List<RecipeSO>();

    #endregion

    #region Events

    public static event Action<GameObject> OnGOCreated;
    public static event Action<GameObject, AssetReference> OnGOCreatedWithAssetRef;
    public static event Action<IList<ScriptableObject>> OnSOsLoaded;
    public static event Action<ScriptableObject> OnSOLoaded;
    public static event Action<IList<UnityEngine.Object>> OnAnyAssetLoad;
    public static event Action<SceneInstance> OnSingleSceneLoad;
    public static event Action<SceneInstance> OnLoadingNewGameInstance;
    public static event Action<SceneInstance> OnAdditiveSceneLoad;
    public static Dictionary<GameObject, AsyncOperationHandle> AsyncHandleDict = new Dictionary<GameObject, AsyncOperationHandle>();

    #endregion

    #region MechanicsFunctions

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

    public static bool CreateGOAsset(string nameKey, Transform parent = null)
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
        if (!Addressables.ReleaseInstance(gameObject))
        {
            MonoBehaviour.Destroy(gameObject);
        }            
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
            OnAdditiveSceneLoad?.Invoke(asyncHandle.Result);
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
            OnSingleSceneLoad?.Invoke(handle.Result);
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

    #endregion

    #region AssetLoadFunctions

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadDependencies()
    {
        LoadSOAssets("ScriptableObject", SOLoadCallback);
        LoadAnyAssets<Material>("Materials", MaterialLoadCallback);
    }

    public static AudioClip GetAudioClip(AudioTypes audioType, MusicTypes musicType, int index = 0, bool randomize = false)
    {
        List<AudioClip> clipList = new List<AudioClip>();
        if (AudioSODict.TryGetValue(audioType, out AudioSO audioSO))
        {
            clipList = audioSO.GetMusicAudio(musicType);
            if (clipList != null)
            {
                if (index >= 0 && index < clipList.Count)
                {
                    if (randomize)
                    {
                        index = UnityEngine.Random.Range(0, clipList.Count - 1);
                    }
                    return clipList[index];
                }
            }
        }
        return null;
    }

    public static AudioClip GetAudioClip(AudioTypes audioType, EnvironmentTypes environmentType, int index = 0, bool randomize = false)
    {
        List<AudioClip> clipList = new List<AudioClip>();
        if (AudioSODict.TryGetValue(audioType, out AudioSO audioSO))
        {
            clipList = audioSO.GetEnvironmentAudio(environmentType);
            if (clipList != null)
            {
                if (index >= 0 && index < clipList.Count)
                {
                    if (randomize)
                    {
                        index = UnityEngine.Random.Range(0, clipList.Count - 1);
                    }
                    return clipList[index];
                }
            }
        }
        return null;
    }

    public static AudioClip GetAudioClip(AudioTypes audioType, WeaponTypes weaponType, int index = 0, bool randomize = false)
    {
        List<AudioClip> clipList = new List<AudioClip>();
        if (AudioSODict.TryGetValue(audioType, out AudioSO audioSO))
        {
            clipList = audioSO.GetWeaponAudio(weaponType);
            if (clipList != null)
            {
                if (index >= 0 && index < clipList.Count)
                {
                    if (randomize)
                    {
                        index = UnityEngine.Random.Range(0, clipList.Count - 1);
                    }
                    return clipList[index];
                }
            }
        }
        return null;
    }

    public static AudioClip GetAudioClip(AudioTypes audioType, SpellTypes spellType, int index = 0, bool randomize = false)
    {
        List<AudioClip> clipList = new List<AudioClip>();
        if (AudioSODict.TryGetValue(audioType, out AudioSO audioSO))
        {
            clipList = audioSO.GetSpellAudio(spellType);
            if (clipList != null)
            {
                if (index >= 0 && index < clipList.Count)
                {
                    if (randomize)
                    {
                        index = UnityEngine.Random.Range(0, clipList.Count - 1);
                    }
                    return clipList[index];
                }
            }
        }
        return null;
    }

    public static AudioClip GetAudioClip(AudioTypes audioType, EnemyTypes enemyType, int index = 0, bool randomize = false)
    {
        List<AudioClip> clipList = new List<AudioClip>();
        if (AudioSODict.TryGetValue(audioType, out AudioSO audioSO))
        {
            clipList = audioSO.GetEnemyAudio(enemyType);
            if (clipList != null)
            {
                if (index >= 0 && index < clipList.Count)
                {
                    if (randomize)
                    {
                        index = UnityEngine.Random.Range(0, clipList.Count - 1);
                    }
                    return clipList[index];
                }
            }
        }
        return null;
    }

    public static Material GetMaterail(string name)
    {
        if (MaterailDict.TryGetValue(name, out Material material))
            return material;
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

    public static RecipeSO GetRecipeSOFromList(RecipeTypes recipeType)
    {
        foreach (RecipeSO recipe in RecipeSOList)
        {
            if (recipe.RecipeType == recipeType)
            {
                return recipe;
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

    public static AmmoSO GetAmmoSOFromList(WeaponTypes weaponType)
    {
        foreach (AmmoSO ammoSO in AmmoSOList)
        {
            if (ammoSO.WeaponType == weaponType)
            {
                return ammoSO;
            }
        }
        return null;
    }

    public static ArmorSO GetArmorSOFromList(ArmorTypes armorType)
    {
        foreach (ArmorSO armorSO in ArmorSOList)
        {
            if (armorSO.ArmorType == armorType)
            {
                return armorSO;
            }
        }
        return null;
    }

    public static SpellBaseSO GetSpellSOFromList(SpellTypes spellType)
    {
        foreach (SpellBaseSO spellSO in spellSOList)
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

    public static InventoryItemSO GetInventoryItemSOFromList(ItemTypes itemType, WeaponTypes weaponType)
    {
        foreach (var invSO in InventorySOList)
        {
            if (invSO.ItemType == itemType && invSO.WeaponType == weaponType)
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

    public static InventoryItemSO GetArmorInventorySO(ArmorTypes AT)
    {
        foreach (var temp in ArmorInventorySOList)
        {
            if (temp.ArmorType == AT)
            {
                return temp;
            }
        }
        return null;
    }

    #endregion

    #region Callbacks

    private static void SOLoadCallback(ScriptableObject SO)
    {
        if (SO.GetType() == typeof(InventoryItemSO))
        {
            InventoryItemSO itemSO = (InventoryItemSO)SO;
            if (itemSO.ItemType == ItemTypes.Spell)
            {
                if (itemSO.SpellType != SpellTypes.None && !SpellInventorySOList.Contains(itemSO))
                {
                    SpellInventorySOList.Add(itemSO);
                }
            }
            else if (itemSO.ItemType == ItemTypes.Weapon)
            {
                if (itemSO.WeaponType != WeaponTypes.None && !WeaponInventorySOList.Contains(itemSO))
                {
                    WeaponInventorySOList.Add(itemSO);
                }
            }
            else if (itemSO.ItemType == ItemTypes.Armor)
            {
                if (itemSO.ArmorType != ArmorTypes.None && !ArmorInventorySOList.Contains(itemSO))
                {
                    ArmorInventorySOList.Add(itemSO);
                }
            }
            else
            {
                if (!InventorySOList.Contains(itemSO))
                {
                    InventorySOList.Add(itemSO);
                }
            }
        }
        else if (SO.GetType() == typeof(WeaponSO))
        {
            if (!WeaponSOList.Contains((WeaponSO)SO))
            {
                WeaponSOList.Add((WeaponSO)SO);
            }
        }
        else if (SO.GetType() == typeof(AmmoSO))
        {
            if (!AmmoSOList.Contains((AmmoSO)SO))
            {
                AmmoSOList.Add((AmmoSO)SO);
            }
        }
        else if (SO.GetType() == typeof(ArmorSO))
        {
            if (!ArmorSOList.Contains((ArmorSO)SO))
            {
                ArmorSOList.Add((ArmorSO)SO);
            }
        }
        else if (SO.GetType() == typeof(EnemySO))
        {
            if (!EnemySOList.Contains((EnemySO)SO))
            {
                EnemySOList.Add((EnemySO)SO);
            }
        }
        else if (SO.GetType() == typeof(SpellBaseSO))
        {
            if (!spellSOList.Contains((SpellBaseSO)SO))
            {
                spellSOList.Add((SpellBaseSO)SO);
            }
        }
        else if (SO.GetType() == typeof(QuestSO))
        {
            QuestSO quest = (QuestSO)SO;
            if (!QuestDict.ContainsValue(quest))
            {
                QuestDict.Add(quest.QuestID, quest);
            }
        }
        else if (SO.GetType() == typeof(AudioSO))
        {
            AudioSO audio = (AudioSO)SO;
            if (!AudioSODict.ContainsKey(audio.AudioType))
            {
                AudioSODict.Add(audio.AudioType, audio);
            }
        }
        else if (SO.GetType() == typeof(RecipeSO))
        {
            RecipeSO recipe = (RecipeSO)SO;
            if (!RecipeSOList.Contains((RecipeSO)SO))
            {
                RecipeSOList.Add((RecipeSO)SO);
            }
        }
    }

    private static void MaterialLoadCallback(Material obj)
    {
        if (!MaterailDict.ContainsValue(obj))
        {
            MaterailDict.Add(obj.name, obj);
        }

    }

    #endregion
}

//~~~~~~~~~~~~~~~~~~~ Essential Enums ~~~~~~~~~~~~~~~~~~~~~~

#region Enums

[Serializable]
public class AssetPackages
{
    public AssetReference Reference;
    public ProjectileTypes projectileTypes;
    public ImpactTypes impactTypes;
}

[Serializable]
public class RequiredStatForEquip
{
    public AttributeTypes attributeType;
    public int attributeValue;
}

public enum AIStates
{
    Stop,
    Roam,
    Chase,
    Prepare,
    Attack
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

public enum ArmorTypes
{
    None,
    RoyalPlate
}

public enum SpellTypes
{
    None,
    FireBlast,
    FreezeBlast,
    ChainLightening,
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
    Fighter,
    Bat,
    Shooter,
    ShootingBat
}

public enum ProjectileTypes
{
    None,
    Bullet,
    FireBall,
    FireBallSpell,
    IceSpike,
    EnemyFireBall
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
    Spell,
    Recipe,
    Herb
}

public enum RecipeTypes
{
    None,
    P3C1
}

public enum GoalTypes
{
    Assassination,
    MassKillType,
    MassKillAny,
    Collect,
    Delivery
}

public enum OverlayTypes
{
    Posterize,
    Damage,
    Shield,
    Fire,
    Poison
}

public enum AudioTypes
{
    Music,
    Environment,
    Weapon,
    Spell,
    Enemy
}

public enum MusicTypes
{
    None,
    Intro,
    Normal,
    Action,
}

public enum EnvironmentTypes
{
    None,
    Insect,
    Bird,
    Wind,
    Water,
    Thunder,
    Rain
}

public enum AttributeTypes
{
    Strength,
    Dexterity,
    Vitality,
    Spirit,
    Intelligence
}

public enum AttributeModType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300,
}

#endregion