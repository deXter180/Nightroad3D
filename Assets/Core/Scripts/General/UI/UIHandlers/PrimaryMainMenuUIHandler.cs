using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

public class PrimaryMainMenuUIHandler : MonoBehaviour
{

    #region SerializedVariables

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Canvas loaderCanvas;
    [SerializeField] private Image progessbar;
    [SerializeField] private float progressMultiplyer = 3;

    #endregion

    #region Variables

    private float updateSpeed;
    private bool clearPreviousScene = false;
    private SceneInstance previousLoadedScene;

    #endregion

    #region General

    private void Awake()
    {
        loaderCanvas.enabled = false;     
    }

    private void OnEnable()
    {
        AssetLoader.OnLoadingNewGameInstance += AssetLoader_OnLoadingNewGameInstance;
    }

    private void OnDisable()
    {
        AssetLoader.OnLoadingNewGameInstance -= AssetLoader_OnLoadingNewGameInstance;
    }

    private void Update()
    {
        progessbar.fillAmount = Mathf.MoveTowards(progessbar.fillAmount, updateSpeed, progressMultiplyer * Time.deltaTime);
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~ MainMenu Scene ~~~~~~~~~~~~~~~~~~~~~~

    #region Mechanics

    public void StartNewGame()
    {
        SceneLoader.LoadNewGame("PSLevel1");
    }

    public void ContinueGame()
    {

    }

    public void OpenSettings()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void AssetLoader_OnLoadingNewGameInstance(SceneInstance obj)
    {
        progessbar.fillAmount = 0;
        updateSpeed = 0;
        mainMenu.SetActive(false);
        loaderCanvas.enabled = true;
        var scene = obj.ActivateAsync();
        do
        {
            updateSpeed = scene.progress;
        }
        while (scene.progress < 0.9f);
        scene.completed += Scene_completed =>
        {
            if (loaderCanvas != null && mainMenu != null)
            {
                loaderCanvas.enabled = false;
                mainMenu.SetActive(true);
            }           
        };
    }

    #endregion
}
