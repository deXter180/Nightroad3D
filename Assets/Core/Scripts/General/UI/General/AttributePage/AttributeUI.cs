using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class AttributeUI : MonoBehaviour
{
    private float pctXP;
    private bool isXPChanged;
    private bool resettingSlider;
    private float sliderValue;
    private AttributeManager attributeManager;
    private LevelSystemManager levelSystemManager;
    private TextMeshProUGUI applyText;
    private TextMeshProUGUI resetText;
    public static event Action<AttributeTypes> OnAddingAttributePoint;

    #region Serialized Properties
    [SerializeField] private float fadeSpeed = 0.3f;
    [SerializeField] private Image xpSliderImage;
    [SerializeField] private TextMeshProUGUI levelNum;
    [SerializeField] private Button strButton;
    [SerializeField] private Button dexButton;
    [SerializeField] private Button vitButton;
    [SerializeField] private Button sprButton;
    [SerializeField] private Button intButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button applyButton;
    [SerializeField] private TextMeshProUGUI strPointCount;
    [SerializeField] private TextMeshProUGUI dexPointCount;
    [SerializeField] private TextMeshProUGUI vitPointCount;
    [SerializeField] private TextMeshProUGUI sprPointCount;
    [SerializeField] private TextMeshProUGUI intPointCount;
    [SerializeField] private TextMeshProUGUI availablePoints;
    [SerializeField] private TextMeshProUGUI originalHP;
    [SerializeField] private TextMeshProUGUI modifiedHP;
    [SerializeField] private TextMeshProUGUI originalMP;
    [SerializeField] private TextMeshProUGUI modifiedMP;
    [SerializeField] private TextMeshProUGUI originalWeapon1Dmg;
    [SerializeField] private TextMeshProUGUI modifiedWeapon1Dmg;
    [SerializeField] private TextMeshProUGUI originalWeapon2Dmg;
    [SerializeField] private TextMeshProUGUI modifiedWeapon2Dmg;
    [SerializeField] private TextMeshProUGUI originalWeapon3Dmg;
    [SerializeField] private TextMeshProUGUI modifiedWeapon3Dmg;
    [SerializeField] private TextMeshProUGUI originalWeapon4Dmg;
    [SerializeField] private TextMeshProUGUI modifiedWeapon4Dmg;
    [SerializeField] private TextMeshProUGUI originalSpell1Dmg;
    [SerializeField] private TextMeshProUGUI modifiedSpell1Dmg;
    [SerializeField] private TextMeshProUGUI originalSpell2Dmg;
    [SerializeField] private TextMeshProUGUI modifiedSpell2Dmg;
    #endregion

    private void Awake()
    {
        attributeManager = GetComponentInParent<AttributeManager>();
        levelSystemManager = GetComponentInParent<LevelSystemManager>();
        applyText = applyButton.GetComponentInChildren<TextMeshProUGUI>();
        resetText = resetButton.GetComponentInChildren<TextMeshProUGUI>();
        LevelSystemManager.OnLevelUp += LevelSystemManager_OnLevelUp;
        levelSystemManager.OnXPChanged += LevelSystemManager_OnXPChanged;
        AttributeManager.OnAttributeStatChanged += AttributeManager_OnAttributeStatChanged;
        pctXP = 0;
        sliderValue = 0;
        isXPChanged = false;
        resettingSlider = false;
        xpSliderImage.fillAmount = 0;
    }

    private void OnEnable()
    {
        ApplyTextUpdate();
        strButton.onClick.AddListener(STRClick);
        dexButton.onClick.AddListener(DEXClick);
        vitButton.onClick.AddListener(VITClick);
        sprButton.onClick.AddListener(SPRClick);
        intButton.onClick.AddListener(INTClick);
        resetButton.onClick.AddListener(ClickedResetButton);
        applyButton.onClick.AddListener(ClickedApplyButton);
    }

    private void OnDisable()
    {
        strButton.onClick.RemoveAllListeners();
        dexButton.onClick.RemoveAllListeners();
        vitButton.onClick.RemoveAllListeners();
        sprButton.onClick.RemoveAllListeners();
        intButton.onClick.RemoveAllListeners();
        resetButton.onClick.RemoveAllListeners();
        applyButton.onClick.RemoveAllListeners();
        ApplyTextUpdate();
    }

    private void OnDestroy()
    {
        LevelSystemManager.OnLevelUp -= LevelSystemManager_OnLevelUp;
        levelSystemManager.OnXPChanged -= LevelSystemManager_OnXPChanged;
        AttributeManager.OnAttributeStatChanged -= AttributeManager_OnAttributeStatChanged;
    }

    private void Update()
    {
        if (resettingSlider)
        {
            sliderValue = Mathf.Lerp(xpSliderImage.fillAmount, 1, Time.deltaTime * 7);
            xpSliderImage.fillAmount = sliderValue;
            if (sliderValue >= 0.99)
            {
                xpSliderImage.fillAmount = 0;
                resettingSlider = false;
            }
        }
        if (isXPChanged && !resettingSlider)
        {
            sliderValue = Mathf.Lerp(xpSliderImage.fillAmount, pctXP, Time.deltaTime * 5);            
            xpSliderImage.fillAmount = sliderValue;
            if (sliderValue >= pctXP - 0.001)
            {
                xpSliderImage.fillAmount = pctXP;      
                isXPChanged = false;
            }
        }        
    }

    public void UpdateLevelNumText(int num)
    {
        levelNum.text = num.ToString();
    }

    public void UpdateAvailablePoint()
    {
        availablePoints.text = attributeManager.AvailablePoints.ToString();
    }

    public void UpdateStatPointText(AttributeTypes attributeType)
    {
        switch (attributeType)
        {
            case AttributeTypes.Strength:
                {
                    strPointCount.text = attributeManager.modStrengthPoint.ToString();
                }
                break;
            case AttributeTypes.Dexterity:
                {
                    dexPointCount.text = attributeManager.modDexterityPoint.ToString();
                }
                break;
            case AttributeTypes.Vitality:
                {
                    vitPointCount.text = attributeManager.modVitalityPoint.ToString();
                }
                break;
            case AttributeTypes.Spirit:
                {
                    sprPointCount.text = attributeManager.modSpiritPoint.ToString();
                }
                break;
            case AttributeTypes.Intelligence:
                {
                    intPointCount.text = attributeManager.modIntelligencePoint.ToString();
                }
                break;
        }
    }

    private void UpdateAllStatPointText()
    {
        strPointCount.text = attributeManager.modStrengthPoint.ToString();
        dexPointCount.text = attributeManager.modDexterityPoint.ToString();
        vitPointCount.text = attributeManager.modVitalityPoint.ToString();
        sprPointCount.text = attributeManager.modSpiritPoint.ToString();
        intPointCount.text = attributeManager.modIntelligencePoint.ToString();
    }

    public void ResetXPSlider()
    {
        resettingSlider = true;
    }

    public void CompleteResetXPSlider()
    {
        xpSliderImage.fillAmount = 0;
    }

    public void UpdateWeaponOGStatText(int num)
    {
        switch (num)
        {
            case 1:
                {
                    originalWeapon1Dmg.text = attributeManager.modWeapon1Stat.ToString();
                }
                break;
            case 2:
                {
                    originalWeapon2Dmg.text = attributeManager.modWeapon2Stat.ToString();
                }
                break;
            case 3:
                {
                    originalWeapon3Dmg.text = attributeManager.modWeapon3Stat.ToString();
                }
                break;
            case 4:
                {
                    originalWeapon4Dmg.text = attributeManager.modWeapon4Stat.ToString();
                }
                break;
        }
    }

    public void UpdateSpellOGStatText(int num)
    {
        if (num == 1)
        {
            originalSpell1Dmg.text = attributeManager.modSpell1Stat.ToString();
        }
        else
        {
            originalSpell2Dmg.text = attributeManager.modSpell2Stat.ToString();
        }
    }

    public void UpdateHPOGStatText()
    {
        originalHP.text = attributeManager.modHPStat.ToString();
    }

    public void UpdateMPOGStatText()
    {
        originalMP.text = attributeManager.modMPStat.ToString();
    }

    public void UpdateWeaponModStatText(int num)
    {
        switch (num)
        {
            case 1:
                {
                    modifiedWeapon1Dmg.text = attributeManager.modWeapon1Stat.ToString();
                }
                break;
            case 2:
                {
                    modifiedWeapon2Dmg.text = attributeManager.modWeapon2Stat.ToString() ;
                }
                break;
            case 3:
                {
                    modifiedWeapon3Dmg.text = attributeManager.modWeapon3Stat .ToString();
                }
                break;
            case 4:
                {
                    modifiedWeapon4Dmg.text = attributeManager.modWeapon4Stat.ToString();
                }
                break;
        }
    }

    public void UpdateSpellModStatText(int num)
    {
        if (num == 1)
        {
            modifiedSpell1Dmg.text = attributeManager.modSpell1Stat.ToString();
        }
        else
        {
            modifiedSpell2Dmg.text = attributeManager.modSpell2Stat.ToString();
        }
    }

    public void UpdateHPModStatText()
    {
        modifiedHP.text = attributeManager.modHPStat.ToString();
    }

    public void UpdateMPModStatText()
    {
        modifiedMP.text = attributeManager.modMPStat.ToString();
    }

    private void UpdateAllWeaponModStatText()
    {
        modifiedWeapon1Dmg.text = attributeManager.modWeapon1Stat.ToString();
        modifiedWeapon2Dmg.text = attributeManager.modWeapon2Stat.ToString();
        modifiedWeapon3Dmg.text = attributeManager.modWeapon3Stat.ToString();
        modifiedWeapon4Dmg.text = attributeManager.modWeapon4Stat.ToString();
    }

    private void UpdateAllSpellModStatText()
    {
        modifiedSpell1Dmg.text = attributeManager.modSpell1Stat.ToString();
        modifiedSpell2Dmg.text = attributeManager.modSpell2Stat.ToString();
    }

    public void UpdateAllWeaponStatText()
    {
        originalWeapon1Dmg.text = modifiedWeapon1Dmg.text = attributeManager.modWeapon1Stat.ToString();
        originalWeapon2Dmg.text = modifiedWeapon2Dmg.text = attributeManager.modWeapon2Stat.ToString();
        originalWeapon3Dmg.text = modifiedWeapon3Dmg.text = attributeManager.modWeapon3Stat.ToString();
        originalWeapon4Dmg.text = modifiedWeapon4Dmg.text = attributeManager.modWeapon4Stat.ToString();
    }

    public void UpdateAllSpellStatText()
    {
        originalSpell1Dmg.text = modifiedSpell1Dmg.text = attributeManager.modSpell1Stat.ToString();
        originalSpell2Dmg.text = modifiedSpell2Dmg.text = attributeManager.modSpell2Stat.ToString();      
    }

    public void UpdateAllHPStatText()
    {
        originalHP.text = modifiedHP.text = attributeManager.modHPStat.ToString();
    }

    public void UpdateAllMPStatText()
    {
        originalMP.text = modifiedMP.text = attributeManager.modMPStat.ToString();
    }

    public void ApplyTextUpdate()
    {        
        attributeManager.ApplyChanges();
        UpdateAllStatPointText();
        UpdateAvailablePoint();
        UpdateAllWeaponStatText();
        UpdateAllSpellStatText();
        UpdateAllHPStatText();
        UpdateAllMPStatText();
    }

    //~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~

    public void ClickedApplyButton()
    {
        applyText.color = Color.red;
        ApplyTextUpdate();
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return Helpers.GetWait(0.1f);
            applyText.color = Color.white;
        }
    }

    public void ClickedResetButton()
    {
        resetText.color = Color.red;
        attributeManager.ResetChanges();
        UpdateAllStatPointText();
        UpdateAvailablePoint();
        UpdateAllWeaponStatText();
        UpdateAllSpellStatText();
        UpdateAllHPStatText();
        UpdateAllMPStatText();
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return Helpers.GetWait(0.1f);
            resetText.color = Color.white;
        }
    }

    private void LevelSystemManager_OnXPChanged()
    {
        pctXP = levelSystemManager.GetPctXP();
        isXPChanged = true;
    }

    private void LevelSystemManager_OnLevelUp(int level)
    {
        UpdateLevelNumText(level);
    }

    private void AttributeManager_OnAttributeStatChanged(AttributeTypes type)
    {
        if (type == AttributeTypes.Strength || type == AttributeTypes.Dexterity)
        {
            UpdateAllWeaponModStatText();
        }
        else if (type == AttributeTypes.Intelligence)
        {
            UpdateAllSpellModStatText();
        }
        else if (type == AttributeTypes.Vitality)
        {
            UpdateHPModStatText();
        }
        else if (type == AttributeTypes.Spirit)
        {
            UpdateMPModStatText();
        }
    } 

    private void STRClick()
    {       
        strButton.image.canvasRenderer.SetAlpha(0.4f);
        OnAddingAttributePoint?.Invoke(AttributeTypes.Strength);
        strButton.image.CrossFadeAlpha(1f, fadeSpeed, false);
    }

    private void DEXClick()
    {
        dexButton.image.canvasRenderer.SetAlpha(0.4f);
        OnAddingAttributePoint?.Invoke(AttributeTypes.Dexterity);
        dexButton.image.CrossFadeAlpha(1f, fadeSpeed, false);
    }

    private void VITClick()
    {
        vitButton.image.canvasRenderer.SetAlpha(0.4f);
        OnAddingAttributePoint?.Invoke(AttributeTypes.Vitality);
        vitButton.image.CrossFadeAlpha(1f, fadeSpeed, false);
    }

    private void SPRClick()
    {
        sprButton.image.canvasRenderer.SetAlpha(0.4f);
        OnAddingAttributePoint?.Invoke(AttributeTypes.Spirit);
        sprButton.image.CrossFadeAlpha(1f, fadeSpeed, false);
    }

    private void INTClick()
    {
        intButton.image.canvasRenderer.SetAlpha(0.4f);
        OnAddingAttributePoint?.Invoke(AttributeTypes.Intelligence);
        intButton.image.CrossFadeAlpha(1f, fadeSpeed, false);
    }
}
