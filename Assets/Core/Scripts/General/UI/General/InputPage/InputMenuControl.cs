using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMenuControl : MonoBehaviour
{
    #region General

    [SerializeField] private Button resetButton;

    private void OnEnable()
    {
        resetButton.onClick.AddListener(() => DoReset());
    }

    private void OnDisable()
    {
        resetButton.onClick.RemoveListener(() => DoReset());
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void DoReset()
    {
        foreach (var tab in InputManager.MenuTabsList)
        {
            tab.ResetTab();
        }
    }

    #endregion
}
