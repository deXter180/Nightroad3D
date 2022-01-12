using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMenuUIHandler : MonoBehaviour
{
    public bool IsMainMenuActive { get => isMainMenuActive; }
    public static InputMenuUIHandler Instance { get; private set; }
    [SerializeField] private Button resetButton;
    private bool isMainMenuActive;
    private Canvas canvas;
    private PlayerInputAsset inputs;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else Instance = this;
        canvas = GetComponentInChildren<Canvas>();
        StartCoroutine(InputDone());
        canvas.enabled = false;
    }

    private void OnEnable()
    {
        resetButton.onClick.AddListener(() => DoReset());
    }

    private void OnDisable()
    {
        resetButton.onClick.RemoveListener(() => DoReset());
    }

    private void Update()
    {
        if (inputs != null)
        {
            ControlMainMenu();
        }       
    }

    private IEnumerator InputDone()
    {
        yield return new WaitUntil(() => InputManager.InputReady);
        inputs = InputManager.InputActions;
    }

    private void ControlMainMenu()
    {
        if (inputs.UI.Menu.triggered)
        {
            Control();
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

    private void Control()
    {
        if (!isMainMenuActive)
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
    }

    //~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~

    private void DoReset()
    {
        foreach(var tab in InputManager.MenuTabsList)
        {
            tab.Reset();
        }
    }
}
