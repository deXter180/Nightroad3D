using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class InputManager : MonoBehaviour
{
    #region Variables

    public static PlayerInputAsset InputActions;
    public static bool InputReady { get => inputReady; }
    private static bool inputReady = false;
    public static List<InputMenuTab> MenuTabsList = new List<InputMenuTab>();

    #endregion

    #region Events

    public static event Action RebindComplete;
    public static event Action RebindCancelled;
    public static event Action<InputAction, int> RebindStarted;

    #endregion

    #region General

    private void Awake()
    {
        if (InputActions == null)
        {
            InputActions = new PlayerInputAsset();
            InputActions.Enable();
        }
        inputReady = true;
        InputMenuTab[] tempArray = GetComponentsInChildren<InputMenuTab>();
        foreach (var menuTab in tempArray)
        {
            MenuTabsList.Add(menuTab);
        }
    }

    private void OnDisable()
    {
        InputActions.Disable();
        InputActions.Dispose();
    }

    #endregion

    #region Mechanics

    public static void StartRebind(string actionName, int bindingIndex, TMP_Text statusText)
    {
        InputAction action = InputActions.asset.FindAction(actionName);
        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Couldn't find action or binding");
            return;
        }
        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
            {
                DoRebind(action, i, statusText, true);
            }          
        }
        else
        {
            DoRebind(action, bindingIndex, statusText, false);
        }
    }

    private static void DoRebind(InputAction actionToRebind, int bindingIndex, TMP_Text statusText, bool allComposite)
    {
        if (actionToRebind == null || bindingIndex < 0)
            return;
        statusText.text = $"Press Any";
        actionToRebind.Disable();
        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Pointer>/position")
            .WithControlsExcluding("<Pointer>/delta")
            .WithCancelingThrough("<Keyboard>/escape");
        rebind.OnComplete(operation => {
            actionToRebind.Enable();
            if (CheckDuplicateBinding(actionToRebind, bindingIndex, allComposite))
            {
                actionToRebind.RemoveBindingOverride(bindingIndex);
                operation.Dispose();
                DoRebind(actionToRebind, bindingIndex, statusText, allComposite);
                return;
            } 
            operation.Dispose();
            SaveBindingOverride(actionToRebind);
            RebindComplete?.Invoke();
        });
        rebind.OnCancel(operation => {
            actionToRebind.Enable();
            operation.Dispose();
            RebindCancelled?.Invoke();
        });
        RebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start(); 
    }

    public static string GetBindingName(string actionName, int bindingIndex)
    {
        if (InputActions == null)
        {
            InputActions = new PlayerInputAsset();
            InputActions.Enable();
            inputReady = true;
        }       
        InputAction action = InputActions.asset.FindAction(actionName);
        if (action != null)
        {
            return action.GetBindingDisplayString(bindingIndex);
        }
        else
        {
            return "";
        }
    }

    private static bool CheckDuplicateBinding(InputAction action, int bindingIndex, bool allComposite)
    {
        InputBinding newBinding = action.bindings[bindingIndex];
        foreach (var binding in action.actionMap.asset.bindings)
        {
            if (binding.action == newBinding.action)
            {
                continue;
            }
            if (binding.effectivePath == newBinding.effectivePath)
            {
                Debug.Log("Duplicate binding found: " + newBinding.effectivePath);
                return true;
            }
            if (allComposite)
            {
                for (int i = 1; i < bindingIndex; i++)
                {
                    if (action.bindings[i].effectivePath == newBinding.effectivePath)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private static void SaveBindingOverride(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static void LoadBindingOverride(string actionName)
    {
        if (InputActions == null)
        {
            InputActions = new PlayerInputAsset();
            InputActions.Enable();
            inputReady = true;
        }
        InputAction action = InputActions.asset.FindAction(actionName);
        if (action != null)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
                    action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
            }
        }        
    }

    public static void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = InputActions.asset.FindAction(actionName);
        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Could not find action");
            return;
        }
        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }
        SaveBindingOverride(action);
    }

    #endregion
}
