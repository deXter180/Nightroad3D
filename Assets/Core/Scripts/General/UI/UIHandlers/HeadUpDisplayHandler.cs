using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadUpDisplayHandler : Singleton<HeadUpDisplayHandler>
{
    private BulletCounterUI bulletCounter;

    private Canvas canvas;

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponentInChildren<Canvas>();
        bulletCounter = GetComponentInChildren<BulletCounterUI>();
        bulletCounter.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        WeaponManager.OnWeaponEmpty += WeaponManager_OnWeaponEmpty;
        WeaponManager.OnMeleeWeaponEquip += WeaponManager_OnMeleeWeaponEquip;
        WeaponManager.OnRangeWeaponEquip += WeaponManager_OnRangeWeaponEquip;
    }

    private void OnDisable()
    {
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        WeaponManager.OnWeaponEmpty -= WeaponManager_OnWeaponEmpty;
        WeaponManager.OnMeleeWeaponEquip -= WeaponManager_OnMeleeWeaponEquip;
        WeaponManager.OnRangeWeaponEquip -= WeaponManager_OnRangeWeaponEquip;
    }

    public void Control(bool isCrosshairActive)
    {
        canvas.enabled = isCrosshairActive;
    }

    //~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~

    private void SceneLoader_OnNewGameStart()
    {
        bulletCounter.gameObject.SetActive(false);
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
}
