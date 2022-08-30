using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BulletCounterUI : MonoBehaviour
{
    #region SerializedVariables

    [SerializeField] private TextMeshProUGUI AmmoInMagCounter;
    [SerializeField] private TextMeshProUGUI TotalAmmoCounter;

    #endregion

    #region General

    private void Start()
    {
        RangedWeapon.OnAmmoAffected += RangedWeapon_OnAmmoAffected;
    }

    public void UpdateDisplay(RangedWeapon rangedWeapon) 
    {
        if (rangedWeapon.IsReady)
        {
            string magAmmo = "";
            string totalAmmo = "";
            if (rangedWeapon.CurrentMagazineAmmo < 10)
            {
                magAmmo = $"0{rangedWeapon.CurrentMagazineAmmo}";
            }
            else
            {
                magAmmo = $"{rangedWeapon.CurrentMagazineAmmo}";
            }
            if (rangedWeapon.CurrentTotalAmmo < 10)
            {
                totalAmmo = $"0{rangedWeapon.CurrentTotalAmmo}";
            }
            else
            {
                totalAmmo = $"{rangedWeapon.CurrentTotalAmmo}";
            }
            AmmoInMagCounter.text = magAmmo;
            TotalAmmoCounter.text = totalAmmo;
        }
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void RangedWeapon_OnAmmoAffected(RangedWeapon rangedWeapon)
    {
        UpdateDisplay(rangedWeapon);
    }

    #endregion

}
