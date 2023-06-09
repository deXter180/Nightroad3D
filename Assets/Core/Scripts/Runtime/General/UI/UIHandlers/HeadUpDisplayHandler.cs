using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadUpDisplayHandler : Singleton<HeadUpDisplayHandler>
{
    #region Variables

    [SerializeField] private GameObject deathScreen;
    private BulletCounterUI bulletCounter;
    private PlayerResourceBarUI resourceBar;
    private Crosshair crosshair;
    private Canvas canvas;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponentInChildren<Canvas>();
        bulletCounter = GetComponentInChildren<BulletCounterUI>();
        resourceBar = GetComponentInChildren<PlayerResourceBarUI>();
        bulletCounter.gameObject.SetActive(false);        
    }

    private void Start()
    {        
        deathScreen.SetActive(false);
        crosshair = Crosshair.Instance;
    }

    private void OnEnable()
    {
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        WeaponManager.OnEmptyWeapon += WeaponManager_OnWeaponEmpty;
        WeaponManager.OnMeleeWeaponEquip += WeaponManager_OnMeleeWeaponEquip;
        WeaponManager.OnRangeWeaponEquip += WeaponManager_OnRangeWeaponEquip;
    }

    private void OnDisable()
    {
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        WeaponManager.OnEmptyWeapon -= WeaponManager_OnWeaponEmpty;
        WeaponManager.OnMeleeWeaponEquip -= WeaponManager_OnMeleeWeaponEquip;
        WeaponManager.OnRangeWeaponEquip -= WeaponManager_OnRangeWeaponEquip;

    }

    #endregion

    #region Mechanics

    public void Control(bool isCrosshairActive)
    {
        canvas.enabled = isCrosshairActive;
        crosshair.gameObject.SetActive(isCrosshairActive);
    }

    public void ExecuteOnDeath()
    {
        bulletCounter.gameObject.SetActive(false);
        resourceBar.gameObject.SetActive(false);
        Control(true);
        deathScreen.SetActive(true);      
    }

    public void AfterReturningToMainScene()
    {
        deathScreen.SetActive(false);
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void SceneLoader_OnNewGameStart()
    {
        bulletCounter.gameObject.SetActive(false);
        resourceBar.ResetOnGameStart();
        resourceBar.gameObject.SetActive(true);
    }

    private void WeaponManager_OnWeaponEmpty()
    {
        bulletCounter.gameObject.SetActive(false);
    }

    private void WeaponManager_OnRangeWeaponEquip(RangedWeapon obj)
    {
        if (obj.IsReady)
        {
            bulletCounter.gameObject.SetActive(true);
            bulletCounter.UpdateDisplay(obj);
        }
        else
        {
            StartCoroutine(WaitForReady());
        }

        IEnumerator WaitForReady()
        {
            yield return new WaitUntil(() => obj.IsReady);
            bulletCounter.gameObject.SetActive(true);
            bulletCounter.UpdateDisplay(obj);
        }
    }

    private void WeaponManager_OnMeleeWeaponEquip()
    {
        bulletCounter.gameObject.SetActive(false);
    }

    #endregion
}
