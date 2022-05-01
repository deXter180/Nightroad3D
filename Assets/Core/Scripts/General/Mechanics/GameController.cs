using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : PersistentSingleton<GameController>
{
    private bool isInventoryActive;
    private bool isMainMenuActive;
    private bool isDialogueActive;
    private InventoryUIHandler inventoryUI;
    private InGameMainMenuUIHandler mainMenu;
    private HeadUpDisplayHandler crosshair;
    private LoadUIHandler loadUI;
    private DialogueManager dialogueManager;
    private SpellManager spellManager;
    private PlayerInputAsset inputs;
    private static Dictionary<string, QuestSO> QuestDict = new Dictionary<string, QuestSO>();
    private static Dictionary<WeaponTypes, AudioClip> ACDictWeapon = new Dictionary<WeaponTypes, AudioClip>();
    private static Dictionary<string, Material> MaterailDict = new Dictionary<string, Material>();
    private static List<InventoryItemSO> InventorySOList = new List<InventoryItemSO>();
    private static List<InventoryItemSO> WeaponInventorySOList = new List<InventoryItemSO>();
    private static List<InventoryItemSO> SpellInventorySOList = new List<InventoryItemSO>();
    private static List<WeaponSO> WeaponSOList = new List<WeaponSO>();
    private static List<SpellBaseSO> spellSOList = new List<SpellBaseSO>();
    private static List<EnemySO> EnemySOList = new List<EnemySO>();

    public bool IsInventoryActive => isInventoryActive;
    public bool IsMainMenuActive => isMainMenuActive;
    public bool IsDialogueActive => isDialogueActive;
    public bool IsCastingSpell => spellManager.IsInSpellCastMode;

    protected override void Awake()
    {
        base.Awake();
        AssetLoader.OnSOsLoaded += AssetRefLoader_OnSOsLoaded;
        AssetLoader.OnSingleSceneLoad += AssetLoader_OnSingleSceneLoad;
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        SceneLoader.OnMainMenuSceneLoad += SceneLoader_OnMainMenuSceneLoad;
        AssetLoader.LoadSOAssets("ScriptableObject", SOLoadCallback);
        AssetLoader.LoadAnyAssets<AudioClip>("AudioFiles", ACLoadCallback);
        AssetLoader.LoadAnyAssets<Material>("Materials", MaterialLoadCallback);
        loadUI = GetComponentInChildren<LoadUIHandler>();
    }

    private void OnEnable()
    {
        loadUI.Control(false);
        isInventoryActive = false;
        isMainMenuActive = false;
        isDialogueActive = false;
        StartCoroutine(InputDone());
    }

    private void OnDisable()
    {
        AssetLoader.OnSOsLoaded -= AssetRefLoader_OnSOsLoaded;
        AssetLoader.OnSingleSceneLoad -= AssetLoader_OnSingleSceneLoad;
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        SceneLoader.OnMainMenuSceneLoad -= SceneLoader_OnMainMenuSceneLoad;
    }

    private void Update()
    {
        if (inputs != null)
        {
            ControlUI();
        }
    }

    private IEnumerator InputDone()
    {
        if (inputs == null)
        {
            yield return new WaitUntil(() => InputManager.InputReady);
            inputs = InputManager.InputActions;
            AssignMenuInstance();
            AssignInvInstance();
            AssignDialogueInstance();
            AssignCrossInstance();
            spellManager = SpellManager.Instance;
        }       
    }

    public void SetDialogueActive(bool isActive)
    {
        isDialogueActive = isActive;
    }

    private void ControlUI()
    {
        ControlMainMenu();
        ControlInventory();
        ControlDialogue();
        ControlCrosshair();
    }

    private void AssignMenuInstance()
    {
        if (mainMenu == null)
        {
            mainMenu = InGameMainMenuUIHandler.Instance;
        }
    }

    private void AssignInvInstance()
    {
        if (inventoryUI == null)
        {
            inventoryUI = InventoryUIHandler.Instance;
        }
    }

    private void AssignCrossInstance()
    {
        if (crosshair == null)
        {
            crosshair = HeadUpDisplayHandler.Instance;
        }
    }

    private void AssignDialogueInstance()
    {
        if (dialogueManager == null)
        {
            dialogueManager = DialogueManager.Instance;
        }
    }

    private void ControlMainMenu()
    {
        if (inputs.UI.Menu.triggered)
        {
            mainMenu.Control(isMainMenuActive);
            if (isMainMenuActive)
            {
                isMainMenuActive = false;
            }
            else
            {
                isMainMenuActive = true;
            }
        }
    }

    private void ControlInventory()
    {
        if (inputs.BasicControls.Inventory.triggered && !isMainMenuActive)
        {
            inventoryUI.Control(isInventoryActive);
            if (isInventoryActive)
            {
                isInventoryActive = false;
            }
            else isInventoryActive = true;
        }
    }

    private void ControlDialogue()
    {
        if (isDialogueActive)
        {
            if (isMainMenuActive || isInventoryActive)
            {
                dialogueManager.EndConversation();
            }
        }
        
    }

    private void ControlCrosshair()
    {
        if (isMainMenuActive || isInventoryActive)
        {
            crosshair.Control(false);
        }
        else
        {
            crosshair.Control(true);
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~ Utility Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~

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

    public static AudioClip GetAudioClipByWeaponType(WeaponTypes weaponType)
    {
        if (ACDictWeapon.TryGetValue(weaponType, out AudioClip AC))
            return AC;
        else return null;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void AssetRefLoader_OnSOsLoaded(IList<ScriptableObject> obj)
    {
        foreach(var SO in obj)
        {
            if (SO.GetType() == typeof(InventoryItemSO))
            {
                InventoryItemSO itemSO = (InventoryItemSO)SO;
                if (itemSO.WeaponType == WeaponTypes.None && itemSO.SpellType == SpellTypes.None)
                {
                    if (!InventorySOList.Contains(itemSO))
                    {
                        InventorySOList.Add(itemSO);
                        InventorySOList.Add(itemSO);
                    }                    
                }
                else if (itemSO.WeaponType != WeaponTypes.None && itemSO.SpellType == SpellTypes.None)
                {
                    if (!WeaponInventorySOList.Contains(itemSO))
                    {
                        WeaponInventorySOList.Add(itemSO);
                    }                   
                }
                else if (itemSO.SpellType != SpellTypes.None && itemSO.WeaponType == WeaponTypes.None)
                {
                    if (!SpellInventorySOList.Contains(itemSO))
                    {
                        SpellInventorySOList.Add(itemSO);
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
                    QuestDict.Add(quest.name, quest);
                }                
            }
        }
    }

    private void SOLoadCallback(ScriptableObject SO)
    {
        
    }

    private void MaterialLoadCallback(Material obj)
    {
        if (!MaterailDict.ContainsValue(obj))
        {
            MaterailDict.Add(obj.name, obj);
        }
        
    }

    private void ACLoadCallback(AudioClip obj)
    {
        if (obj.name == "RifleAttackAudio" && !ACDictWeapon.ContainsValue(obj))
        {
            ACDictWeapon.Add(WeaponTypes.Rifle, obj);
        }
    }

    private void SceneLoader_OnMainMenuSceneLoad()
    {
        isInventoryActive = true;
        isMainMenuActive = true;
        mainMenu.Control(isMainMenuActive);
        inventoryUI.Control(isInventoryActive);
        crosshair.Control(false);
        dialogueManager.EndConversation();
    }

    private void SceneLoader_OnNewGameStart()
    {
        AssignMenuInstance();
        AssignInvInstance();
        AssignDialogueInstance();
        AssignCrossInstance();
        isInventoryActive = false;
        isMainMenuActive = false;
    }

    private void AssetLoader_OnSingleSceneLoad(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance obj)
    {
        isInventoryActive = false;
        isMainMenuActive = false;
        //loadUI.Control(false);
    }
}
