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
    [SerializeField] private Image SelectionImage;
    [SerializeField] private RectTransform NPCSpeechBubble;
    [SerializeField] private Canvas speechBubbleCanvas;
    [SerializeField] private Color FogColor;
    [SerializeField] private List<SpawnLocations> spawnLocations;

    #endregion

    #region Variables

    private Vector3 screenPos;
    private bool isInventoryActive;
    private bool isStashActive;
    private bool isMainMenuActive;
    private bool isDialogueActive;
    private bool isCraftingActive;
    private bool isUIActive;
    private GameObject UICam;
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
    private CraftMenuControl craftMenu;
    public static event Action OnStashClose;
    public static event Action OnCraftingClose;

    #endregion

    #region Properties

    public bool IsInventoryActive => isInventoryActive;
    public bool IsStashActive => isStashActive;
    public bool IsMainMenuActive => isMainMenuActive;
    public bool IsDialogueActive => isDialogueActive;
    public bool IsCraftingActive => isCraftingActive;
    public bool IsUIActive => isUIActive;
    public bool IsCastingSpell => spellManager.IsInSpellCastMode;

    #endregion

    #region GeneralFuctions

    protected override void Awake()
    {
        base.Awake();
        AssetLoader.OnSingleSceneLoad += AssetLoader_OnSingleSceneLoad;
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        SceneLoader.OnMainMenuSceneLoad += SceneLoader_OnMainMenuSceneLoad;       
        loadUI = GetComponentInChildren<LoadUIHandler>();
        returnButton = GetComponentInChildren<ReturnToMenuButton>();
        npcSpeechText = NPCSpeechBubble.GetComponentInChildren<TextMeshProUGUI>();
        UICam = GetComponentInChildren<Camera>().gameObject;
        NPCSpeechBubble.gameObject.SetActive(false);
        OpenStashImage.gameObject.SetActive(false);
        SelectionImage.gameObject.SetActive(false);
        TalkUIimage.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        loadUI.Control(false);
        isInventoryActive = false;
        isUIActive = false;
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
        AssetLoader.OnSingleSceneLoad -= AssetLoader_OnSingleSceneLoad;
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        SceneLoader.OnMainMenuSceneLoad -= SceneLoader_OnMainMenuSceneLoad;
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

    public Vector3 GetSpawnLocation(string levelName)
    {
        for (int i = 0; i < spawnLocations.Count; i++)
        {
            if (spawnLocations[i].LevelName == levelName)
            {
                return spawnLocations[i].SpawnPosition;
            }
        }
        return Vector3.zero;
    }

    public void DisableLightningFog()
    {       
        RenderSettings.fog = false;
    }

    public void DisableFPSCam()
    {
        FPSCam.gameObject.SetActive(false);
    }

    public void ControlCursor(bool isActive)
    {
        if (!isActive)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;            
        }
        Cursor.visible = !isActive;
    }

    public void SetDialogueActive(bool isActive)
    {
        isDialogueActive = isActive;
        isUIActive = isActive;
        ControlCursor(isActive);
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
        if (craftMenu == null)
        {
            craftMenu = CraftMenuControl.Instance;
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
                if (!isInventoryActive && !isStashActive && !isCraftingActive)
                {
                    isUIActive = false;
                }               
                ControlCursor(false);
            }
            else
            {
                isMainMenuActive = true;
                isUIActive = true;
                ControlCursor(true);
            }
        }
    }

    private void ControlInventory()
    {
        if (!isMainMenuActive && !isStashActive && !isCraftingActive)
        {
            if (inputs.BasicControls.Inventory.triggered)
            {
                inventoryUI.ControlInv(isInventoryActive);
                if (isInventoryActive)
                {
                    isInventoryActive = false;
                    isUIActive = false;
                    ControlCursor(false);
                }
                else
                {
                    isInventoryActive = true;
                    isUIActive = true;
                    ControlCursor(true);
                }
            }
        }
    }

    public void OpenStash()
    {
        if (!isMainMenuActive && !isInventoryActive && !isStashActive && !isCraftingActive)
        {
            inventoryUI.ControlSth(isStashActive);
            isStashActive = true;
            isUIActive = true;
            ControlCursor(true);
        }
    }

    public void CloseStash() //Calling from UI
    {
        if (isStashActive)
        {
            inventoryUI.ControlSth(isStashActive);
            isStashActive = false;
            isUIActive = false;
            ControlCursor(false);
            OnStashClose?.Invoke();
        }
    }

    public void OpenCrafting()
    {
        if (!isMainMenuActive && !isInventoryActive && !isStashActive && !isCraftingActive)
        {
            inventoryUI.ControlCft(isCraftingActive);
            isCraftingActive = true;
            isUIActive = true;
            ControlCursor(true);
        }
    }

    public void CloseCrafting()
    {
        if (isCraftingActive)
        {
            inventoryUI.ControlCft(IsCraftingActive);
            isCraftingActive = false;
            isUIActive = false;
            ControlCursor(false);
            OnCraftingClose?.Invoke();
        }
    }

    private void ControlDialogue()
    {
        if (isDialogueActive)
        {
            if (isMainMenuActive || isInventoryActive || isStashActive || isCraftingActive)
            {
                dialogueManager.EndConversation();
                ControlCursor(false);
            }
        }
        
    }

    private void ControlHUD()
    {
        if (isMainMenuActive || isInventoryActive || isStashActive || isDialogueActive || isCraftingActive)
        {
            HUDHandler.Control(false);
            UnHighlightInteract();
            UnHighlightSelection();
            UnHighlightStash();
            ControlCursor(false);
        }
        else
        {
            HUDHandler.Control(true);
            ControlCursor(true);
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

    public void HighlightInteract(Vector3 position)
    {
        TalkUIimage.gameObject.SetActive(true);
        screenPos = Helpers.MainCam.WorldToScreenPoint(position);
        TalkUIimage.transform.position = screenPos;
    }
    public void UnHighlightInteract()
    {
        TalkUIimage.gameObject.SetActive(false);
    }

    public void HighlightSelection(Bounds bound, float distance, bool isRecipe = false)
    {
        SelectionImage.gameObject.SetActive(true);
        SelectionImage.transform.position = Helpers.MainCam.WorldToScreenPoint(new Vector3 (
            bound.center.x, 
            bound.center.y, 
            bound.center.z));
        Vector3 newScale = new Vector3();
        if (isRecipe)
        {
            newScale = new Vector3(
            bound.size.z * (1 / (float)Math.Round(distance, 1)) * 300f,
            bound.size.y * (1 / (float)Math.Round(distance, 1)) * 15f,
            bound.size.x);
        }
        else
        {
            newScale = new Vector3(
            bound.size.x * (1 / (float)Math.Round(distance, 1)) * 15f,
            bound.size.y * (1 / (float)Math.Round(distance, 1)) * 15f,
            bound.size.z);
        }
        if (float.IsFinite(newScale.x) && float.IsFinite(newScale.y))
        {
            SelectionImage.transform.localScale = newScale;
        }
    }

    public void UnHighlightSelection()
    {
        SelectionImage.gameObject.SetActive(false);
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

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void SceneLoader_OnMainMenuSceneLoad()
    {
        ControlCursor(true);
        DisableLightningFog();
        isInventoryActive = true;
        isMainMenuActive = true;
        isStashActive = true;
        mainMenu.Control(isMainMenuActive);
        inventoryUI.ControlInv(isInventoryActive);
        inventoryUI.ControlSth(isStashActive);
        HUDHandler.Control(false);
        dialogueManager.EndConversation();
        AudioManager.StopMusicSound();
        AudioManager.StopWeaponSound();
        AudioManager.StopEnvironmentSound();
        AudioManager.StopRainAudio();
        AudioManager.StopWindAudio();
    }

    private void SceneLoader_OnNewGameStart()
    {
        ControlCursor(false);
        AssignInstances();
        InputManager.InputActionSetup();
        isInventoryActive = false;
        isMainMenuActive = false;
        isStashActive = false;
        dialogueManager.ResetDialogues();
        inventorySystem.ResetInventory();
        equipMenu.ResetEquipMenu();
        craftMenu.ResetMenu();
        itemStash.ResetStash();
        StartCoroutine(DelayAudio());
        //var LOSManager = FindObjectOfType<EnemyLOSManager>();
        //if (LOSManager != null)
        //{
        //    LOSManager.CheckForEnemy();
        //}
        if (FPSCam == null)
        {
            FPSCam = FPSCamControl.Instance;
        }
        if (FPSCam != null && !FPSCam.gameObject.activeInHierarchy)
        {
            FPSCam.EnableFPSCamera();
        }

        //Remove this
        StartCoroutine(inventorySystem.Test());

        IEnumerator DelayAudio()
        {
            yield return Helpers.GetWait(0.5f);
            AudioManager.PlayMusicSound(MusicTypes.Normal, 0, false, true);
        }

    }

    private void AssetLoader_OnSingleSceneLoad(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance obj)
    {
        isInventoryActive = false;
        isMainMenuActive = false;
        //var LOSManager = FindObjectOfType<EnemyLOSManager>();
        //if (LOSManager != null)
        //{
        //    LOSManager.CheckForEnemy();
        //}
    }

    #endregion
}

[Serializable]
public struct SpawnLocations
{
    public string LevelName;
    public Vector3 SpawnPosition;
}
