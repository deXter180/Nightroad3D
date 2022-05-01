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
    private static bool clearPreviousScene = false;
    private static SceneInstance previousLoadedScene;
    public static event Action OnNewGameStart;
    public static event Action OnMainMenuSceneLoad;

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

    public static void LoadMainScene()
    {
        if (clearPreviousScene && previousLoadedScene.Scene != null)
        {
            AssetLoader.UnloadAddressableLevel(previousLoadedScene, clearPreviousScene);
        }
        SceneManager.LoadSceneAsync("Main Menu").completed += (handle) => 
        {
            AssetLoader.AsyncHandleDict.Clear();
            OnMainMenuSceneLoad?.Invoke();
        };
    }

    public static void LoadNewSingleScene(string nameKey)
    {        
        AssetLoader.LoadAddressableLevelSingle(nameKey, previousLoadedScene, clearPreviousScene);       
    }

    public static void LoadNewAdditiveScene(string nameKey)
    {       
        AssetLoader.LoadAddressableLevelAdditive(nameKey, previousLoadedScene, clearPreviousScene);
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void AssetLoader_OnLoadingNewGameInstance(SceneInstance obj)
    {
        StartCoroutine(DelayInvoke());
        IEnumerator DelayInvoke()
        {
            yield return Helpers.GetWait(1f);
            OnNewGameStart?.Invoke();
        }
    }

    private static void AssetLoader_OnSingleSceneLoad(SceneInstance obj)
    {

    }

    private static void AssetLoader_OnAdditiveSceneLoad(SceneInstance obj)
    {

    }
}
