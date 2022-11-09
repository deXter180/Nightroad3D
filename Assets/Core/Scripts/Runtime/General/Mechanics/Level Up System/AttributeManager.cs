using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;

public class AttributeManager : Singleton<AttributeManager>
{
    #region SerializedProperties

    //Strength
    [SerializeField] private AttributeModType STRModType;
    [SerializeField] private float STR1stMultiplier;
    [SerializeField] private float STR2ndMultiplier;
    [SerializeField] private float STR3rdMultiplier;
    [SerializeField] private float STR4thMultiplier;
    [Space(10)]
    [SerializeField] private int STR1stThreshold;
    [SerializeField] private int STR2ndThreshold;
    [SerializeField] private int STR3rdThreshold;
    [SerializeField] private int STR4thThreshold;

    //Dexterity
    [SerializeField] private AttributeModType DEXModType;
    [SerializeField] private float DEX1stMultiplier;
    [SerializeField] private float DEX2ndMultiplier;
    [SerializeField] private float DEX3rdMultiplier;
    [SerializeField] private float DEX4thMultiplier;
    [Space(10)]
    [SerializeField] private int DEX1stThreshold;
    [SerializeField] private int DEX2ndThreshold;
    [SerializeField] private int DEX3rdThreshold;
    [SerializeField] private int DEX4thThreshold;

    //Vitality
    [SerializeField] private AttributeModType VITModType;
    [SerializeField] private float VIT1stMultiplier;
    [SerializeField] private float VIT2ndMultiplier;
    [SerializeField] private float VIT3rdMultiplier;
    [SerializeField] private float VIT4thMultiplier;
    [Space(10)]
    [SerializeField] private int VIT1stThreshold;
    [SerializeField] private int VIT2ndThreshold;
    [SerializeField] private int VIT3rdThreshold;
    [SerializeField] private int VIT4thThreshold;

    //Spirit
    [SerializeField] private AttributeModType SPRModType;
    [SerializeField] private float SPR1stMultiplier;
    [SerializeField] private float SPR2ndMultiplier;
    [SerializeField] private float SPR3rdMultiplier;
    [SerializeField] private float SPR4thMultiplier;
    [Space(10)]
    [SerializeField] private int SPR1stThreshold;
    [SerializeField] private int SPR2ndThreshold;
    [SerializeField] private int SPR3rdThreshold;
    [SerializeField] private int SPR4thThreshold;

    //Intelligence
    [SerializeField] private AttributeModType INTModType;
    [SerializeField] private float INT1stMultiplier;
    [SerializeField] private float INT2ndMultiplier;
    [SerializeField] private float INT3rdMultiplier;
    [SerializeField] private float INT4thMultiplier;
    [Space(10)]
    [SerializeField] private int INT1stThreshold;
    [SerializeField] private int INT2ndThreshold;
    [SerializeField] private int INT3rdThreshold;
    [SerializeField] private int INT4thThreshold;

    [SerializeField] private StatInfo StrengthInfo;
    [SerializeField] private StatInfo DexterityInfo;
    [SerializeField] private StatInfo VitalityInfo;
    [SerializeField] private StatInfo SpiritInfo;
    [SerializeField] private StatInfo IntelligenceInfo;

    #endregion

    #region Properties

    public int AvailablePoints { get; private set; }
    public int modStrengthPoint { get; private set; }
    public int modDexterityPoint { get; private set; }
    public int modVitalityPoint { get; private set; }
    public int modSpiritPoint { get; private set; }
    public int modIntelligencePoint { get; private set; }
    public int ogStrengthPoint => attributes.strength;
    public int ogDexterityPoint => attributes.dexterity;
    public int ogVitalityPoint => attributes.vitality;
    public int ogSpiritPoint => attributes.spirit;
    public int ogIntelligencePoint => attributes.intelligence;
    public int modWeapon1Stat { get; private set; }
    public int modWeapon2Stat { get; private set; }
    public int modWeapon3Stat { get; private set; }
    public int modWeapon4Stat { get; private set; }
    public float modSpell1Stat { get; private set; }
    public float modSpell2Stat { get; private set; }
    public int modHPStat { get; private set; }
    public int modMPStat { get; private set; }
    public float StrengthStat => strength.ModifiedValue;
    public float DexterityStat => dexterity.ModifiedValue;
    public float VitalityStat => vitality.ModifiedValue;
    public float SpiritStat => spirit.ModifiedValue;
    public float IntelligenceStat => intelligence.ModifiedValue;

    #endregion

    #region Variables

    private float elapsedTime = 0;
    private int pointsToAdd = 1;
    private int availablePoints;
    private int originalWeapon1Stat = 0;
    private int originalWeapon2Stat = 0;
    private int originalWeapon3Stat = 0;
    private int originalWeapon4Stat = 0;
    private float originalSpell1Stat = 0;
    private float originalSpell2Stat = 0;
    private int originalHPStat = 0;
    private int originalMPStat = 0;
    private BaseAttributes attributes;
    private BaseAttributes origAttributes;
    private AttributeUI attributeUI;
    private PlayerController playerController;
    private LevelSystemManager levelmanager;
    private CharacterAttribute strength;
    private CharacterAttribute dexterity;
    private CharacterAttribute vitality;
    private CharacterAttribute spirit;
    private CharacterAttribute intelligence;
    private List<CharacterAttribute> characterAttributes;
    private Dictionary<int, Weapons> availableWeapons;
    private Dictionary<int, Spells> availableSpell;
    public static event Action<AttributeTypes> OnAttributeStatChanged;

    #endregion

    #region Functions
    protected override void Awake()
    {
        base.Awake();   
        attributeUI = GetComponentInChildren<AttributeUI>();
        origAttributes = new BaseAttributes
            (
            attributes.strength,
            attributes.dexterity,
            attributes.vitality,
            attributes.spirit,
            attributes.intelligence
            );
        SetupStats();
    }

    private void OnEnable()
    {
        SceneLoader.OnMainMenuSceneLoad += SceneLoader_OnMainMenuSceneLoad;
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
        AttributeUI.OnAddingAttributePoint += AttributeUI_OnAddingAttributePoint;
        LevelSystemManager.OnLevelUp += LevelSystemManager_OnLevelUp;
        WeaponManager.OnAddingWeapon += WeaponManager_OnAddingWeapon;
        WeaponManager.OnRemovingWeapon += WeaponManager_OnRemovingWeapon;
        SpellManager.OnAddingSpell += SpellManager_OnAddingSpell;
        SpellManager.OnRemovingSpell += SpellManager_OnRemovingSpell;
    }

    private void OnDisable()
    {
        SceneLoader.OnMainMenuSceneLoad -= SceneLoader_OnMainMenuSceneLoad;
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
        AttributeUI.OnAddingAttributePoint -= AttributeUI_OnAddingAttributePoint;
        LevelSystemManager.OnLevelUp -= LevelSystemManager_OnLevelUp;
        WeaponManager.OnAddingWeapon -= WeaponManager_OnAddingWeapon;
        WeaponManager.OnRemovingWeapon -= WeaponManager_OnRemovingWeapon;
        SpellManager.OnAddingSpell -= SpellManager_OnAddingSpell;
        SpellManager.OnRemovingSpell -= SpellManager_OnRemovingSpell;
    }

    private void Start()
    {        
        playerController = PlayerController.Instance;
        levelmanager = LevelSystemManager.Instance;
    }

    public void SetupStats()
    {
        if (characterAttributes != null)
        {
            foreach (var attribute in characterAttributes)
            {
                attribute.OnStatChanged -= Attribute_OnStatChanged;
            }
        }
        availableWeapons = new Dictionary<int, Weapons>();
        availableSpell = new Dictionary<int, Spells>();
        characterAttributes = new List<CharacterAttribute>();
        strength = new CharacterAttribute(StrengthInfo.BaseValue, AttributeTypes.Strength);
        dexterity = new CharacterAttribute(DexterityInfo.BaseValue, AttributeTypes.Dexterity);
        vitality = new CharacterAttribute(VitalityInfo.BaseValue, AttributeTypes.Vitality);
        spirit = new CharacterAttribute(SpiritInfo.BaseValue, AttributeTypes.Spirit);
        intelligence = new CharacterAttribute(IntelligenceInfo.BaseValue, AttributeTypes.Intelligence);
        characterAttributes.Add(strength);
        characterAttributes.Add(dexterity);
        characterAttributes.Add(vitality);
        characterAttributes.Add(spirit);
        characterAttributes.Add(intelligence);
        foreach (var attribute in characterAttributes)
        {
            attribute.OnStatChanged += Attribute_OnStatChanged;
        }
        availablePoints = 0;
    }

    public void AddAttributePoint(AttributeTypes attributeType)
    {
        if (AvailablePoints > 0)
        {
            float point = 0;
            switch (attributeType)
            {
                case AttributeTypes.Strength:
                    {
                        modStrengthPoint++;
                        point = GetSTRPoint();                       
                        strength.IncrementBaseValue(point, STRModType);
                    }
                    break;
                case AttributeTypes.Dexterity:
                    {
                        modDexterityPoint++;
                        point = GetDEXPoint();                                               
                        dexterity.IncrementBaseValue(point, DEXModType);
                    }
                    break;
                case AttributeTypes.Vitality:
                    {
                        modVitalityPoint++;
                        point = GetVITPoint();                                            
                        vitality.IncrementBaseValue(point, VITModType);
                    }
                    break;
                case AttributeTypes.Spirit:
                    {
                        modSpiritPoint++;
                        point = GetSPRPoint();                                              
                        spirit.IncrementBaseValue(point, SPRModType);
                    }
                    break;
                case AttributeTypes.Intelligence:
                    {
                        modIntelligencePoint++;
                        point = GetINTPoint();                                              
                        intelligence.IncrementBaseValue(point, INTModType);
                    }
                    break;
            }  
        }        
    }

    public void ApplyChanges()
    {        
        attributes.strength = modStrengthPoint;
        attributes.dexterity = modDexterityPoint;
        attributes.vitality = modVitalityPoint;
        attributes.spirit = modSpiritPoint;
        attributes.intelligence = modIntelligencePoint;
        availablePoints = AvailablePoints;
        originalWeapon1Stat = modWeapon1Stat;
        originalWeapon2Stat = modWeapon2Stat;
        originalWeapon3Stat = modWeapon3Stat;
        originalWeapon4Stat = modWeapon4Stat;
        originalSpell1Stat = modSpell1Stat;
        originalSpell2Stat = modSpell2Stat;
        originalHPStat = modHPStat;
        originalMPStat = modMPStat;
        foreach (var attribute in characterAttributes)
        {
            attribute.ApplyValue();
        }
    }

    public void ResetChanges(bool isInitialSetup = false)
    {
        if (isInitialSetup)
        {
            attributes.strength = origAttributes.strength;
            attributes.dexterity = origAttributes.dexterity;
            attributes.vitality = origAttributes.vitality;
            attributes.spirit = origAttributes.spirit;
            attributes.intelligence = origAttributes.intelligence;
            originalHPStat = modHPStat = playerController.BaseHitPoints;
            originalMPStat = playerController.BaseMana;
            originalWeapon1Stat = 0;
            originalWeapon2Stat = 0;
            originalWeapon3Stat = 0;
            originalWeapon4Stat = 0;
            originalSpell1Stat = 0;
            originalSpell2Stat = 0;
        }
        modHPStat = originalHPStat;
        modMPStat = originalMPStat;
        if (!isInitialSetup)
        {
            foreach (var attribute in characterAttributes)
            {
                attribute.ResetValue();
            }
            playerController.ResetHP(modHPStat);
            playerController.ResetMP(modMPStat);
        }        
        modStrengthPoint = attributes.strength;
        modDexterityPoint = attributes.dexterity;
        modVitalityPoint = attributes.vitality;
        modSpiritPoint = attributes.spirit;
        modIntelligencePoint = attributes.intelligence;
        AvailablePoints = availablePoints;
        modWeapon1Stat = originalWeapon1Stat;
        modWeapon2Stat = originalWeapon2Stat;
        modWeapon3Stat = originalWeapon3Stat;
        modWeapon4Stat = originalWeapon4Stat;
        modSpell1Stat = originalSpell1Stat;
        modSpell2Stat = originalSpell2Stat;        
    }

    private float GetSTRPoint()
    {
        float point = 0;
        if (strength.BaseValue < STR1stThreshold)
        {
            point = STR1stMultiplier;
        }
        else if (strength.BaseValue < STR2ndThreshold)
        {
            point = STR2ndMultiplier;
        }
        else if (strength.BaseValue < STR3rdThreshold)
        {
            point = STR3rdMultiplier;
        }
        else
        {
            point = STR4thMultiplier;
        }
        return point;        
    }

    private float GetDEXPoint()
    {
        float point = 0;
        if (dexterity.BaseValue < DEX1stThreshold)
        {
            point = DEX1stMultiplier;
        }
        else if (dexterity.BaseValue < DEX2ndThreshold)
        {
            point = DEX2ndMultiplier;
        }
        else if (dexterity.BaseValue < DEX3rdThreshold)
        {
            point = DEX3rdMultiplier;
        }
        else
        {
            point = DEX4thMultiplier;
        }
        return point;
    }

    private float GetVITPoint()
    {
        float point = 0;
        if (vitality.BaseValue < VIT1stThreshold)
        {
            point = VIT1stMultiplier;
        }
        else if (vitality.BaseValue < VIT2ndThreshold)
        {
            point = VIT2ndMultiplier;
        }
        else if (vitality.BaseValue < VIT3rdThreshold)
        {
            point = VIT3rdMultiplier;
        }
        else
        {
            point = VIT4thMultiplier;
        }
        return point;
    }

    private float GetSPRPoint()
    {
        float point = 0;
        if (spirit.BaseValue < SPR1stThreshold)
        {
            point = SPR1stMultiplier;
        }
        else if (spirit.BaseValue < SPR2ndThreshold)
        {
            point = SPR2ndMultiplier;
        }
        else if (spirit.BaseValue < SPR3rdThreshold)
        {
            point = SPR3rdMultiplier;
        }
        else
        {
            point = SPR4thMultiplier;
        }
        return point;
    }

    private float GetINTPoint()
    {
        float point = 0;
        if (intelligence.BaseValue < INT1stThreshold)
        {
            point = INT1stMultiplier;
        }
        else if (intelligence.BaseValue < INT2ndThreshold)
        {
            point = INT2ndMultiplier;
        }
        else if (intelligence.BaseValue < INT3rdThreshold)
        {
            point = INT3rdMultiplier;
        }
        else
        {
            point = INT4thMultiplier;
        }
        return point;
    }

    public void IncrementPoint()
    {
        AvailablePoints++;
        attributeUI.UpdateAvailablePoint();
    }

    public void IncreasePoint(int value)
    {
        AvailablePoints += value;
        attributeUI.UpdateAvailablePoint();
    }

    public void DeductPoint()
    {
        if (AvailablePoints > 0)
        {
            AvailablePoints--;
        }        
    }

    public bool IsStatRequirementCompleted(AttributeTypes attributeType, int modValue)
    {
        bool result = false;
        switch (attributeType)
        {
            case AttributeTypes.Strength:
                result = attributes.strength >= modValue;
                break;
            case AttributeTypes.Dexterity:
                result = attributes.dexterity >= modValue;
                break;
            case AttributeTypes.Vitality:
                result = attributes.vitality >= modValue;
                break;
            case AttributeTypes.Spirit:
                result = attributes.spirit >= modValue;
                break;
            case AttributeTypes.Intelligence:
                result = attributes.intelligence >= modValue;
                break;
        }
        return result;
    }

    private void UpdateWeaponStats()
    {
        foreach (var index in availableWeapons.Keys)
        {
            int modValue = availableWeapons[index].GetModifiedStat();
            switch (index)
            {
                case 1:
                    {
                        modWeapon1Stat = modValue;
                    }
                    break;
                case 2:
                    {
                        modWeapon2Stat = modValue;
                    }
                    break;
                case 3:
                    {
                        modWeapon3Stat = modValue;
                    }
                    break;
                case 4:
                    {
                        modWeapon4Stat = modValue;
                    }
                    break;
            }
        }
    }

    private void UpdateSpellStat()
    {
        foreach (var index in availableSpell.Keys)
        {
            float modValue = availableSpell[index].GetModifiedStats();
            if (index == 1)
            {
                modSpell1Stat = modValue;
            }
            else
            {
                modSpell2Stat = modValue;
            }
        }
    }

    private void UpdateHPStat()
    {
        modHPStat = playerController.MaxHitPoints;
    }

    private void UpdateMPStat()
    {
        modMPStat = playerController.MaxMana;
    }

    private void InitialUpdateWeaponStats(WeaponTypes weaponType, int num)
    {
        Weapons thisWeapon = WeaponManager.weaponInventory[weaponType].GetThisWeapon();
        if (thisWeapon != null)
        {
            int modValue = thisWeapon.GetModifiedStat();
            switch (num)
            {
                case 1:
                    {
                        originalWeapon1Stat = modWeapon1Stat = modValue;
                    }
                    break;
                case 2:
                    {
                        originalWeapon2Stat = modWeapon2Stat = modValue;
                    }
                    break;
                case 3:
                    {
                        originalWeapon3Stat = modWeapon3Stat = modValue;
                    }
                    break;
                case 4:
                    {
                        originalWeapon4Stat = modWeapon4Stat = modValue;
                    }
                    break;
            }
            availableWeapons.Add(num, thisWeapon);
        }       
    }

    private void InitialUpdateSpellStats(SpellTypes spellType, int num)
    {
        Spells thisSpell = SpellManager.spellDict[spellType];
        if (thisSpell != null)
        {
            float modValue = thisSpell.GetModifiedStats();
            if (num == 1)
            {
                originalSpell1Stat = modSpell1Stat = modValue;
            }
            else
            {
                originalSpell2Stat = modSpell2Stat = modValue;
            }
            availableSpell.Add(num, thisSpell);
        }
    }

    private void ResetWeaponStat(int num)
    {
        switch (num)
        {
            case 1:
                {
                    originalWeapon1Stat = modWeapon1Stat = 0;
                }
                break;
            case 2:
                {
                    originalWeapon2Stat = modWeapon2Stat = 0;
                }
                break;
            case 3:
                {
                    originalWeapon3Stat = modWeapon3Stat = 0;
                }
                break;
            case 4:
                {
                    originalWeapon4Stat = modWeapon4Stat = 0;
                }
                break;
        }
        availableWeapons.Remove(num);
    }

    private void ResetSpellStat(int num)
    {
        if (num == 1)
        {
            originalSpell1Stat = modSpell1Stat = 0;
        }
        else
        {
            originalSpell2Stat = modSpell2Stat = 0;
        }
        availableSpell.Remove(num);
    }
    #endregion

    //~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~

    #region Callback

    private void SceneLoader_OnMainMenuSceneLoad()
    {
        if (attributeUI != null)
        {
            attributeUI.ApplyTextUpdate();
        }
    }

    private void SceneLoader_OnNewGameStart()
    {
        SetupStats();
        ResetChanges(true);
        levelmanager.ResetLevel();
        attributeUI.UpdateAllWeaponStatText();
        attributeUI.UpdateAllSpellStatText();
        attributeUI.UpdateAllHPStatText();
        attributeUI.UpdateAllMPStatText();
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return Helpers.GetWait(1);
            IncreasePoint(80);
            availablePoints = AvailablePoints;
        }
    }

    private void Attribute_OnStatChanged(AttributeTypes type)
    {

        if (type == AttributeTypes.Strength || type == AttributeTypes.Dexterity)
        {
            UpdateWeaponStats();
        }
        else if (type == AttributeTypes.Intelligence)
        {
            UpdateSpellStat();
        }
        else if (type == AttributeTypes.Vitality || type == AttributeTypes.Spirit)
        {
            playerController.UpdateResouce(type);
            if (type == AttributeTypes.Vitality)
            {
                UpdateHPStat();
            }
            else
            {
                UpdateMPStat();
            }
        }
        OnAttributeStatChanged?.Invoke(type);
    }

    private void AttributeUI_OnAddingAttributePoint(AttributeTypes type)
    {
        if (Time.time - elapsedTime >= 0.3f)
        {
            AddAttributePoint(type);
            DeductPoint();
            attributeUI.UpdateAvailablePoint();
            attributeUI.UpdateStatPointText(type);
            elapsedTime = Time.time;
        }
    }

    private void LevelSystemManager_OnLevelUp(int value)
    {
        if (value != 1)
        {
            IncreasePoint(1);
        }       
    }

    private void WeaponManager_OnAddingWeapon(WeaponTypes type, int num)
    {
        InitialUpdateWeaponStats(type, num);
        attributeUI.UpdateWeaponOGStatText(num);
        attributeUI.UpdateWeaponModStatText(num);
    }

    private void WeaponManager_OnRemovingWeapon(WeaponTypes type, int num)
    {
        ResetWeaponStat(num);
        attributeUI.UpdateWeaponOGStatText(num);
        attributeUI.UpdateWeaponModStatText(num);
    }

    private void SpellManager_OnAddingSpell(SpellTypes type, int num)
    {
        InitialUpdateSpellStats(type, num);
        attributeUI.UpdateSpellOGStatText(num);
        attributeUI.UpdateSpellModStatText(num);
    }

    private void SpellManager_OnRemovingSpell(SpellTypes type, int num)
    {
        ResetSpellStat(num);
        attributeUI.UpdateSpellOGStatText(num);
        attributeUI.UpdateSpellModStatText(num);
    }

    #endregion
}

[System.Serializable]
public struct BaseAttributes
{
    public int strength;
    public int dexterity;
    public int vitality;
    public int spirit;
    public int intelligence;

    public BaseAttributes(int str, int dex, int vit, int spr, int inte)
    {
        strength = str;
        dexterity = dex;
        vitality = vit;
        spirit = spr;
        intelligence = inte;
    }
}

[System.Serializable]
public struct StatInfo
{
    public float BaseValue;
}
