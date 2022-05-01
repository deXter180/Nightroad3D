using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMainMenuUIHandler : Singleton<InGameMainMenuUIHandler>
{
    [SerializeField] private Canvas menuCanvas;

    protected override void Awake()
    {
        base.Awake();
        menuCanvas.enabled = false;
    }

    public void Control(bool isMainMenuActive)
    {
        if (!isMainMenuActive)
        {
            menuCanvas.enabled = true;
        }
        else
        {
            menuCanvas.enabled = false;
        }
    }

    public void MainSceneLoad()
    {
        SceneLoader.LoadMainScene();
    }
}
