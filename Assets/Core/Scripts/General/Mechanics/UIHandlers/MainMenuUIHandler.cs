using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIHandler : MonoBehaviour
{
    public static MainMenuUIHandler Instance { get; private set; }
    [SerializeField] private Button resetButton;
    private Canvas canvas;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else Instance = this;
        canvas = GetComponentInChildren<Canvas>();
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

    public void Control(bool isMainMenuActive)
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
