using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RayShotGun : RangedWeapon
{
    [SerializeField] private float MuzzleFlashTime;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int PelletsCount;
    [SerializeField] private Crosshair.CrosshairProperties crosshairProperty;
    [SerializeField] private CameraShake.ShakeProperty shakeProperties;
    [SerializeField] private RecoilEffect.RecoilProperty recoilProperty;
    private GameController gameController;
    private Crosshair crosshair;
    private bool isRanged;
    private Weapons thisWeapon;
    private int PlayerLayer = 9;
    private int bitmask;
    private string enemyName = "Enemy";
    private string npcName = "NPC";
    private WeaponManager weaponManager;
    private CameraShake camShake;
    private RecoilEffect recoilEffect;
    private VisualEffect visualEffect;
    private Light lighting;
    public static event Action OnStopSGShoot;

    protected override void Awake()
    {
        base.Awake();
        weaponBrain = GetComponent<WeaponBrain>();
        visualEffect = GetComponentInChildren<VisualEffect>();
        bitmask = ~(1 << PlayerLayer);
        lighting = GetComponentInChildren<Light>();
        if (lighting.gameObject.activeInHierarchy)
            lighting.gameObject.SetActive(false);
        StartCoroutine(SetupWeapon());
        IEnumerator SetupWeapon()
        {
            yield return new WaitUntil(() => weaponBrain.IsWeaponReady());
            thisWeapon = weaponBrain.GetThisWeapon();
            weaponType = weaponBrain.GetWeaponTypes();
            maxMagazineAmmo = thisWeapon.ThisWeaponSO.AmmoPerMagazine;
            maxTotalAmmo = thisWeapon.ThisWeaponSO.TotalAmmo;
            currentMagazineAmmo = maxMagazineAmmo;
            currentTotalAmmo = maxTotalAmmo;
            weaponCategory = weaponBrain.GetWeaponCategories();
            attackRange = thisWeapon.ThisWeaponSO.AttackRange;
            attackDelay = thisWeapon.ThisWeaponSO.AttackDelay;
            bloom = thisWeapon.ThisWeaponSO.Bloom;
            isRanged = thisWeapon.ThisWeaponSO.IsRanged;
            isReady = true;
        }
    }

    protected override void Start()
    {
        base.Start();
        weaponManager = WeaponManager.Instance;
        camShake = CameraShake.Instance;
        gameController = GameController.Instance;
        recoilEffect = RecoilEffect.Instance;
    }

    private void OnEnable()
    {
        if (crosshair == null)
        {
            crosshair = Crosshair.Instance;
        }
        StartCoroutine(crosshair.SetupCrosshair(crosshairProperty));
    }

    private void OnDisable()
    {
        camShake.StopShake();
        crosshair.RemoveCrosshair();
    }


    private void Update()
    {
        if (gameController != null && inputs != null)
        {
            if (gameObject.activeInHierarchy && weaponBrain.IsWeaponReady())
            {
                if (!gameController.IsInventoryActive && !gameController.IsMainMenuActive && !gameController.IsStashActive && !gameController.IsCastingSpell && !gameController.IsDialogueActive)
                {
                    if (!weaponManager.IsAttacking)
                    {
                        if (inputs.BasicControls.Shoot.ReadValue<float>() == 1)
                        {
                            if (currentMagazineAmmo > 0 && !isReloading)
                            {
                                StartCoroutine(Shoot(() => {                                  
                                    currentMagazineAmmo -= 1;
                                    currentTotalAmmo -= 1;
                                    currentMagazineAmmo = currentTotalAmmo == 1 ? 1 : currentMagazineAmmo;
                                    camShake.StopShake();
                                    crosshair.SetRisizing(false);
                                    CallEvent(this);                                    
                                    weaponManager.IsAttacking = false;
                                }));
                            }
                            else
                            {                                
                                Debug.Log("RELOAD!");
                            }
                        }
                        else if (inputs.BasicControls.Reload.triggered)
                        {
                            StartCoroutine(Reload(thisWeapon, weaponBrain, weaponType));
                        }
                    }
                    else
                    {
                        OnStopSGShoot?.Invoke();
                    }
                }
            }
        }
       
    }

    public IEnumerator Shoot(Action action)
    {
        StartCoroutine(PlayMuzzleLight());
        if (isRanged)
        {
            weaponManager.IsAttacking = true;
            thisWeapon.RaiseOnPlayerAttack(thisWeapon, true, weaponBrain, weaponCategory, weaponType);
            crosshair.SetRisizing(true);
            visualEffect.Play();
            recoilEffect.ApplyRecoil(recoilProperty);
            camShake.StartShake(shakeProperties);
            for(int i = 0; i < PelletsCount; i++)
            {
                if (Physics.Raycast(cam.transform.position, GenerateRandomRecoil(), out RaycastHit hit, attackRange, bitmask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag(enemyName))
                        {
                            if (hit.collider.GetComponentInParent<Target>() != null)
                            {
                                Target target = hit.collider.GetComponentInParent<Target>();
                                if (target.enemyBrain != null && target.GetEnemy() == true && target.IsDead == false)
                                {
                                    thisWeapon.DoAttack(target, target.enemyBrain.ThisEnemySO.DodgeChance);
                                    if (!target.Dodging)
                                    {

                                    }
                                }
                            }
                        }                        
                        else if (hit.collider.CompareTag(npcName))
                        {
                            if (hit.collider.GetComponent<NPCBrain>() != null)
                            {
                                NPCBrain npc = hit.collider.GetComponent<NPCBrain>();
                                StartCoroutine(gameController.HighlightNPCSpeech(npc.SpeechBubblePos, npc.GetDialogueText()));
                            }
                        }
                        else
                        {

                        }
                        weaponBrain.SpawnHitVfx(hit.point);
                    }
                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(-hit.normal * thisWeapon.ThisWeaponSO.ImpactForce, ForceMode.Impulse);
                    }
                }
            }
        }
        yield return Helpers.GetWait(attackDelay);
        action.Invoke();
        IEnumerator PlayMuzzleLight()
        {
            if (!lighting.gameObject.activeInHierarchy)
            {
                lighting.gameObject.SetActive(true);
                yield return Helpers.GetWait(MuzzleFlashTime);
                lighting.gameObject.SetActive(false);
            }
        }
    }
}
