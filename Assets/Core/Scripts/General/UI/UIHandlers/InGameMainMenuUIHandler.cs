using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMainMenuUIHandler : Singleton<InGameMainMenuUIHandler>
{
    #region Variables

    private float updateSpeed;
    private TabGroup tabGroup;
    private PlayerController player;
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Canvas loaderCanvas;
    [SerializeField] private Image progessbar;
    public static event Action OnMainMenuOpen;
    public static event Action OnMainMenuClose;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        menuCanvas.enabled = false;
        tabGroup = GetComponentInChildren<TabGroup>();
    }

    private void OnEnable()
    {
        player = PlayerController.Instance;
        AssetLoader.OnSingleSceneLoad += AssetLoader_OnSingleSceneLoad;
        AssetLoader.OnAdditiveSceneLoad += AssetLoader_OnAdditiveSceneLoad;
    }

    private void OnDisable()
    {
        AssetLoader.OnSingleSceneLoad -= AssetLoader_OnSingleSceneLoad;
        AssetLoader.OnAdditiveSceneLoad -= AssetLoader_OnAdditiveSceneLoad;
    }

    public void Control(bool isMainMenuActive)
    {
        if (!isMainMenuActive)
        {
            menuCanvas.enabled = true;
            tabGroup.InitialEnable();
            OnMainMenuOpen?.Invoke();
        }
        else
        {
            menuCanvas.enabled = false;
            OnMainMenuClose?.Invoke();
        }
    }

    public void MainSceneLoad()
    {
        SceneLoader.LoadMainScene();
    }

    public void LoadTestScene()
    {
        SceneLoader.LoadNewSingleScene("Test1");
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void AssetLoader_OnSingleSceneLoad(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance obj)
    {
        progessbar.fillAmount = 0;
        updateSpeed = 0;
        menuCanvas.enabled = false;
        loaderCanvas.enabled = true;
        var scene = obj.ActivateAsync();
        do
        {
            updateSpeed = scene.progress;
        }
        while (scene.progress < 0.9f);
        scene.completed += Scene_completed =>
        {
            if (loaderCanvas != null && menuCanvas != null)
            {
                loaderCanvas.enabled = false;
                player.PlayerTransform.position = new Vector3(0, 0.2f, 0);
                Helpers.MainCam.transform.position = new Vector3(0, 0.2f, 0);
            }
        };
    }

    private void AssetLoader_OnAdditiveSceneLoad(UnityEngine.ResourceManagement.ResourceProviders.SceneInstance obj)
    {

    }

    #endregion
}
