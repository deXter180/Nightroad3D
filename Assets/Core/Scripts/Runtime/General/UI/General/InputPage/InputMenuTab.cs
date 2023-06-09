using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class InputMenuTab : MonoBehaviour
{
    #region SerializedVariables

    [SerializeField] private InputActionReference inputReference;
    [Range(0,10)][SerializeField] private int selectedBinding;
    [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions;
    [Header("Binding Info - DO NOT EDIT")]
    [SerializeField] private InputBinding inputBinding;
    [Header("UI Fields")]
    [SerializeField] private TMP_Text actionText;
    [SerializeField] private TMP_Text bindText;
    [SerializeField] private Button rebindButton;

    #endregion

    #region Variables

    private int bindingIndex;
    private string actionName;
    private InputAction defaultAction;

    #endregion

    #region Properties

    public TMP_Text ActionText { get => actionText; }
    public TMP_Text BindText{ get => bindText; }
    public Button RebindButton { get => rebindButton; }

    #endregion

    #region General 

    private void OnEnable()
    {
        if (inputReference != null)
        {
            GetBindingInfo();
            InputManager.LoadBindingOverride(actionName);
            defaultAction = inputReference.action;           
            UpdateUI();
        }
        rebindButton.onClick.AddListener(() => DoRebind());
        InputManager.RebindComplete += UpdateUI;
        InputManager.RebindCancelled += UpdateUI;
    }

    private void OnDisable()
    {
        rebindButton.onClick.RemoveListener(() => DoRebind());
        InputManager.RebindComplete -= UpdateUI;
        InputManager.RebindCancelled -= UpdateUI;
    }

    private void OnValidate()
    {
        if (inputReference == null)
            return;
        GetBindingInfo();
        UpdateUI();
    }

    #endregion

    #region Mechanics

    private void GetBindingInfo()
    {
        if (inputReference.action != null)
        {
            actionName = inputReference.action.name;
        }            
        if (inputReference.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputReference.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }
    }

    private void UpdateUI()
    {
        if (actionText != null && actionName != null)
        {
            actionText.text = actionName;
        }
        if (bindText != null)
        {
            if (Application.isPlaying)
            {
                bindText.text = InputManager.GetBindingName(actionName, bindingIndex);
            }
            else
            {
                bindText.text = inputReference.action.GetBindingDisplayString(bindingIndex);
            }
        }
    }

    public void ResetTab()
    {
        InputManager.ResetBinding(actionName, bindingIndex);
        UpdateUI();
    }

    #endregion

    //~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~

    #region Callbacks

    private void DoRebind()
    {
        InputManager.StartRebind(actionName, bindingIndex, bindText);
    }

    #endregion
}
