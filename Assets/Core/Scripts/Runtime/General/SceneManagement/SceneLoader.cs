using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SceneLoader : PersistentSingleton<SceneLoader>
{
    #region Variables

    public static bool clearPreviousScene = false;
    public static SceneInstance previousLoadedScene;
    public static event Action OnNewGameStart;
    public static event Action OnMainMenuSceneLoad;

    #endregion

    #region GeneralFunctions

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        AssetLoader.OnLoadingNewGameInstance += AssetLoader_OnLoadingNewGameInstance;
        AssetLoader.OnSingleSceneLoad += AssetLoader_OnSingleSceneLoad;
        AssetLoader.OnAdditiveSceneLoad += AssetLoader_OnAdditiveSceneLoad;
    }

    private void OnDisable()
    {
        AssetLoader.OnLoadingNewGameInstance -= AssetLoader_OnLoadingNewGameInstance;
        AssetLoader.OnSingleSceneLoad -= AssetLoader_OnSingleSceneLoad;
        AssetLoader.OnAdditiveSceneLoad -= AssetLoader_OnAdditiveSceneLoad;
    }

    #endregion

    #region MechanicsFunctions

    public static void LoadMainScene()
    {
        if (clearPreviousScene && previousLoadedScene.Scene != null)
        {
            AssetLoader.UnloadAddressableLevel(previousLoadedScene, clearPreviousScene);
        }
        SceneManager.LoadSceneAsync("Main Menu").completed += (handle) => 
        {
            AssetLoader.AsyncHandleDict.Clear();
            FPSCamControl.Instance.DisableFPSCamera();
            OnMainMenuSceneLoad?.Invoke();
        };
    }

    public static void LoadNewGame(string nameKey)
    {        
        AssetLoader.LoadFreshGameInstance(nameKey);
    }

    public static void LoadNewSingleScene(string nameKey, bool clearPrevious = true)
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.ParentPlayerCam();
        }
        AssetLoader.LoadAddressableLevel(nameKey, previousLoadedScene, clearPrevious, false);       
    }

    public static void LoadNewAdditiveScene(string nameKey, bool clearPrevious = false)
    {       
        AssetLoader.LoadAddressableLevel(nameKey, previousLoadedScene, clearPrevious, true);
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void AssetLoader_OnLoadingNewGameInstance(SceneInstance obj)
    {
        StartCoroutine(DelayInvoke());
        IEnumerator DelayInvoke()
        {
            yield return Helpers.GetWait(0.5f);
            previousLoadedScene = obj;
            clearPreviousScene = true;
            OnNewGameStart?.Invoke();
        }
    }

    private void AssetLoader_OnSingleSceneLoad(SceneInstance obj)
    {
        previousLoadedScene = obj;
        clearPreviousScene = true;
    }

    private void AssetLoader_OnAdditiveSceneLoad(SceneInstance obj)
    {
        previousLoadedScene = obj;
        clearPreviousScene = true;
    }

    #endregion
}
