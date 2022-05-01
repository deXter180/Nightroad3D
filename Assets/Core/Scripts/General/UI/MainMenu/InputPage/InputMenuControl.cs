using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMenuControl : MonoBehaviour
{
    [SerializeField] private Button resetButton;

    private void OnEnable()
    {
        resetButton.onClick.AddListener(() => DoReset());
    }

    private void OnDisable()
    {
        resetButton.onClick.RemoveListener(() => DoReset());
    }

    //~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~

    private void DoReset()
    {
        foreach (var tab in InputManager.MenuTabsList)
        {
            tab.Reset();
        }
    }
}
