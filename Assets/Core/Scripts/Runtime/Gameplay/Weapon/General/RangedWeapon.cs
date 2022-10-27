using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RangedWeapon : MonoBehaviour
{
    #region Variables

    protected float bloom;
    private int recoilIndex = 0;
    protected WeaponTypes weaponType;
    protected WeaponCategories weaponCategory;
    protected InventorySystem inventorySystem;
    protected WeaponBrain weaponBrain;
    protected Camera cam;
    protected PlayerInputAsset inputs;
    protected int currentMagazineAmmo;
    protected int currentTotalAmmo;
    protected int maxMagazineAmmo;
    protected int maxTotalAmmo;
    protected float attackRange;
    protected float attackDelay;
    protected bool isReloading;
    protected bool isReady = false;
    public static event Action<RangedWeapon> OnAmmoAffected;

    #endregion

    #region Properties

    public int CurrentMagazineAmmo => currentMagazineAmmo;
    public int CurrentTotalAmmo => currentTotalAmmo;
    public int MaxMagazineAmmo => maxMagazineAmmo;
    public int MaxTotalAmmo => maxTotalAmmo;
    public bool IsReady => isReady;

    #endregion

    #region General

    protected virtual void Awake()
    {
        weaponBrain = GetComponent<WeaponBrain>();
    }

    protected virtual void Start()
    {
        isReloading = false;
        inventorySystem = InventorySystem.Instance;
        cam = Helpers.MainCam;
        weaponBrain.OnStopPlayingReload += WeaponBrain_OnStopPlayingReload;
        StartCoroutine(InputDone());
        IEnumerator InputDone()
        {
            yield return new WaitUntil(() => InputManager.InputReady);
            inputs = InputManager.InputActions;
            weaponBrain.GetThisWeapon();
        }
    }

    private void OnDestroy()
    {
        weaponBrain.OnStopPlayingReload -= WeaponBrain_OnStopPlayingReload;
    }

    #endregion

    #region Mechanics

    protected static void CallEvent(RangedWeapon RG)
    {
        OnAmmoAffected?.Invoke(RG);
    } 

    public bool AddAmmo(int ammoAmount)
    {
        if (maxTotalAmmo - currentTotalAmmo >= ammoAmount)
        {
            currentTotalAmmo += ammoAmount;
            OnAmmoAffected?.Invoke(this);
            return true;
        }
        return false;
    }

    protected IEnumerator Reload(Weapons weapon, WeaponBrain weaponBrain, WeaponTypes weaponType)
    {
        if (currentTotalAmmo > 0 || AddAmmoIfAvailable())
        {
            if (currentMagazineAmmo < maxMagazineAmmo)
            {
                if (currentTotalAmmo >= maxMagazineAmmo)
                {
                    currentMagazineAmmo = maxMagazineAmmo;
                }
                else
                {
                    currentMagazineAmmo = currentTotalAmmo;
                }
                isReloading = true;
                CallEvent(this);
                weapon.RaiseOnPlayerReload(weapon, this, weaponType);
                yield return Helpers.GetWait(weaponBrain.AnimDelay);
            }
        }
    }

    protected Vector3 GenerateRandomRecoil()
    {
        Vector3 t_bloom = cam.transform.position + cam.transform.forward * attackRange;
        t_bloom += UnityEngine.Random.Range(-bloom, bloom) * cam.transform.up;
        t_bloom += UnityEngine.Random.Range(-bloom, bloom) * cam.transform.right;
        t_bloom -= cam.transform.position;
        t_bloom.Normalize();
        return t_bloom;
    }

    private bool AddAmmoIfAvailable()
    {
        if (inventorySystem.GetAvailableAmmo(weaponType) != null)
        {
            PlacedObject placedObject = inventorySystem.GetAvailableAmmo(weaponType);
            int ammoAmount = AssetLoader.GetAmmoSOFromList(weaponType).AmmoAmountInPack;
            if (AddAmmo(ammoAmount))
            {
                inventorySystem.RemoveItem(placedObject);
                return true;
            }
        }
        return false;
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void WeaponBrain_OnStopPlayingReload()
    {
        isReloading = false;
    }

    #endregion
}
