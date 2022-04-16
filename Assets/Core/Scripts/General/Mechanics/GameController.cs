using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    private bool isInventoryActive;
    private bool isMainMenuActive;
    private bool isDialogueActive;
    private InventoryUIHandler inventoryUI;
    private MainMenuUIHandler mainMenu;
    private CrosshairUIHandler crosshair;
    private DialogueManager dialogueManager;
    private SpellManager spellManager;
    private PlayerInputAsset inputs;

    public bool IsInventoryActive => isInventoryActive;
    public bool IsMainMenuActive => isMainMenuActive;
    public bool IsDialogueActive => isDialogueActive;
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else Instance = this;
    }

    private void Start()
    {
        isInventoryActive = false;
        isMainMenuActive = false;
        isDialogueActive = false;
        StartCoroutine(InputDone());
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
        yield return new WaitUntil(() => InputManager.InputReady);
        inputs = InputManager.InputActions;
        inventoryUI = InventoryUIHandler.Instance;
        mainMenu = MainMenuUIHandler.Instance;
        crosshair = CrosshairUIHandler.Instance;
        dialogueManager = DialogueManager.Instance;
        spellManager = SpellManager.Instance;
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

}
