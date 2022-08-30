using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameController : PersistentSingleton<GameController>
{

    #region SerializedVariables

    [SerializeField] private Image TalkUIimage;
    [SerializeField] private Image OpenStashImage;
    [SerializeField] private RectTransform NPCSpeechBubble;
    [SerializeField] private Canvas speechBubbleCanvas;

    #endregion

    #region Variables

    private Vector3 screenPos;
    private bool isInventoryActive;
    private bool isStashActive;
    private bool isMainMenuActive;
    private bool isDialogueActive;
    private InventoryUIHandler inventoryUI;
    private InGameMainMenuUIHandler mainMenu;
    private HeadUpDisplayHandler HUDHandler;
    private LoadUIHandler loadUI;
    private DialogueManager dialogueManager;
    private ReturnToMenuButton returnButton;
    private TextMeshProUGUI npcSpeechText;
    private SpellManager spellManager;
    private PlayerInputAsset inputs;
    private PlayerController player;
    private FPSCamControl FPSCam;
    private EquipMenuControl equipMenu;
    private InventorySystem inventorySystem;
    private ItemStash itemStash;
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
    public static event Action OnStashClose;

    #endregion

    #region Properties

    public bool IsInventoryActive => isInventoryActive;
    public bool IsStashActive => isStashActive;
    public bool IsMainMenuActive => isMainMenuActive;
    public bool IsDialogueActive => isDialogueActive;
    public bool IsCastingSpell => spellManager.IsInSpellCastMode;

    #endregion

    #region GeneralFuctions

    protected override void Awake()
    {
        base.Awake();
        AssetLoader.OnSOsLoaded += AssetLoader_OnSOsLoaded;
        AssetLoader.OnSingleSceneLoad += AssetLoader_OnSingleSceneLoad;
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        SceneLoader.OnMainMenuSceneLoad += SceneLoader_OnMainMenuSceneLoad;
        AssetLoader.LoadSOAssets("ScriptableObject", SOLoadCallback);
        AssetLoader.LoadAnyAssets<Material>("Materials", MaterialLoadCallback);
        loadUI = GetComponentInChildren<LoadUIHandler>();
        returnButton = GetComponentInChildren<ReturnToMenuButton>();
        npcSpeechText = NPCSpeechBubble.GetComponentInChildren<TextMeshProUGUI>();
        NPCSpeechBubble.gameObject.SetActive(false);
        OpenStashImage.gameObject.SetActive(false);
        TalkUIimage.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        loadUI.Control(false);
        isInventoryActive = false;
        isStashActive = false;
        isMainMenuActive = false;
        isDialogueActive = false;
        player = PlayerController.Instance;
        StartCoroutine(InputDone());
        IEnumerator InputDone()
        {
            if (inputs == null)
            {
                yield return new WaitUntil(() => InputManager.InputReady);
                inputs = InputManager.InputActions;
                AssignInstances();
            }
        }
    }

    private void OnDisable()
    {
        AssetLoader.OnSOsLoaded -= AssetLoader_OnSOsLoaded;
        AssetLoader.OnSingleSceneLoad -= AssetLoader_OnSingleSceneLoad;
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        SceneLoader.OnMainMenuSceneLoad -= SceneLoader_OnMainMenuSceneLoad;
    }

    private void Start()
    {
        RainSystem.Instance.ControlRain();
    }

    private void Update()
    {        
        if (inputs != null && player != null)
        {
            if (!player.IsPlayerDead)
            {
                ControlUI();                
            }
            else
            {
                returnButton.AffectReturnButton(inputs.BasicControls.MousePosition.ReadValue<Vector2>(), inputs.BasicControls.Shoot.triggered);
            }  
        }
    }

    #endregion

    #region MechanicsFunctions

    public void SetDialogueActive(bool isActive)
    {
        isDialogueActive = isActive;
    }

    private void ControlUI()
    {
        ControlMainMenu();
        ControlInventory();
        ControlDialogue();
        ControlHUD();
    }

    private void AssignInstances()
    {
        if (mainMenu == null)
        {
            mainMenu = InGameMainMenuUIHandler.Instance;
        }
        if (inventoryUI == null)
        {
            inventoryUI = InventoryUIHandler.Instance;
        }
        if (HUDHandler == null)
        {
            HUDHandler = HeadUpDisplayHandler.Instance;
        }
        if (dialogueManager == null)
        {
            dialogueManager = DialogueManager.Instance;
        }
        if (equipMenu == null)
        {
            equipMenu = EquipMenuControl.Instance;
        }
        if (spellManager == null)
        {
            spellManager = SpellManager.Instance;
        }
        if (inventorySystem == null)
        {
            inventorySystem = InventorySystem.Instance;
        }
        if (itemStash == null)
        {
            itemStash = ItemStash.Instance;
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
        if (!isMainMenuActive && !isStashActive)
        {
            if (inputs.BasicControls.Inventory.triggered)
            {
                inventoryUI.ControlInv(isInventoryActive);
                if (isInventoryActive)
                {
                    isInventoryActive = false;
                }
                else isInventoryActive = true;
            }
        }      
    }

    public void OpenStash()
    {
        if (!isMainMenuActive && !isInventoryActive && !isStashActive)
        {
            inventoryUI.ControlSth(isStashActive);
            isStashActive = true;
        }
    }

    public void CloseStash() //Calling from UI
    {
        if (isStashActive)
        {
            inventoryUI.ControlSth(isStashActive);
            isStashActive = false;
            OnStashClose?.Invoke();
        }
    }

    private void ControlDialogue()
    {
        if (isDialogueActive)
        {
            if (isMainMenuActive || isInventoryActive || isStashActive)
            {
                dialogueManager.EndConversation();
            }
        }
        
    }

    private void ControlHUD()
    {
        if (isMainMenuActive || isInventoryActive || isStashActive || isDialogueActive)
        {
            HUDHandler.Control(false);
        }
        else
        {
            HUDHandler.Control(true);
        }
    }

    public IEnumerator HighlightNPCSpeech(Vector3 position, string textToShow)
    {
        if (!NPCSpeechBubble.gameObject.activeSelf)
        {
            speechBubbleCanvas.transform.position = position;
            npcSpeechText.text = textToShow;
            NPCSpeechBubble.gameObject.SetActive(true);           
            yield return Helpers.GetWait(3f);
            NPCSpeechBubble.gameObject.SetActive(false);
        }       
    }

    public void HighlightDialogue(Vector3 position)
    {
        TalkUIimage.gameObject.SetActive(true);
        screenPos = Helpers.MainCam.WorldToScreenPoint(position);
        TalkUIimage.transform.position = screenPos;
    }

    public void UnHighlightDialogue()
    {
        TalkUIimage.gameObject.SetActive(false);
    }

    public void HighlightStash(Vector3 position)
    {
        OpenStashImage.gameObject.SetActive(true);
        screenPos = Helpers.MainCam.WorldToScreenPoint(position);
        OpenStashImage.transform.position = screenPos;
    }

    public void UnHighlightStash()
    {
        OpenStashImage.gameObject.SetActive(false);
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~ Utility Methods ~~~~~~~~~~~~~~~~~~~~~~~~~~~

    #region UtilityFunctions

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

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void AssetLoader_OnSOsLoaded(IList<ScriptableObject> obj)
    {
        foreach(var SO in obj)
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

    private void SceneLoader_OnMainMenuSceneLoad()
    {
        isInventoryActive = true;
        isMainMenuActive = true;
        isStashActive = true;
        mainMenu.Control(isMainMenuActive);
        inventoryUI.ControlInv(isInventoryActive);
        inventoryUI.ControlSth(isStashActive);
        HUDHandler.Control(false);
        dialogueManager.EndConversation();
    }

    private void SceneLoader_OnNewGameStart()
    {
        AssignInstances();
        isInventoryActive = false;
        isMainMenuActive = false;
        isStashActive = false;
        dialogueManager.ResetDialogues();
        inventorySystem.ResetInventory();
        equipMenu.ResetEquipMenu();
        itemStash.ResetStash();
        if (FPSCam == null)
        {
            FPSCam = FPSCamControl.Instance;
        }
        if (FPSCam != null && !FPSCam.gameObject.activeInHierarchy)
        {
            FPSCam.EnableFPSCamera();
        }
    }

    private void AssetLoader_OnSingleSceneLoad(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance obj)
    {
        isInventoryActive = false;
        isMainMenuActive = false;
    }

    #endregion
}
