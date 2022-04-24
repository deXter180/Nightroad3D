using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadUpDisplayHandler : MonoBehaviour
{
    public static HeadUpDisplayHandler Instance { get; private set; }
    private BulletCounterUI bulletCounter;

    private Canvas canvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = this;
        }
        canvas = GetComponentInChildren<Canvas>();
        bulletCounter = GetComponentInChildren<BulletCounterUI>();
    }

    private void Start()
    {
        bulletCounter.gameObject.SetActive(false);
        WeaponManager.OnMeleeWeaponEquip += WeaponManager_OnMeleeWeaponEquip;
        WeaponManager.OnRangeWeaponEquip += WeaponManager_OnRangeWeaponEquip;
    }

    public void Control(bool isCrosshairActive)
    {
        canvas.enabled = isCrosshairActive;
    }

    //~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~

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
