using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestMenuManager : Singleton<QuestMenuManager>
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI descriptionTitleText;
    [SerializeField] private TextMeshProUGUI goalTitleText;
    [SerializeField] private RectTransform tabPrefab;
    [SerializeField] private Scrollbar scrollbar1;
    [SerializeField] private Scrollbar scrollbar2;
    [SerializeField] private Scrollbar scrollbar3;
    [SerializeField] private List<GameObject> panelGOList; 
    [SerializeField] private List<RectTransform> questListPanels;
    [SerializeField] private List<TextMeshProUGUI> buttonTexts;
    private int panelIndex;
    private Dictionary<QuestSO, QuestNameTab> activeQuestDict;
    private List<QuestNameTab> completedQuestList;
    private List<QuestNameTab> failedQuestList;

    protected override void Awake()
    {
        base.Awake();
        activeQuestDict = new Dictionary<QuestSO, QuestNameTab>();
        completedQuestList = new List<QuestNameTab>();
        failedQuestList = new List<QuestNameTab>();
        panelIndex = 0;
    }

    private void OnEnable()
    {    
        DisablePanels();
        Unhighlight();
        panelIndex = 0;
        EnablePanel();
        Highlight();
    }

    public void CreateActiveQuestTab(QuestSO quest)
    {
        var tabInstance = Instantiate(tabPrefab, questListPanels[0]) as RectTransform;
        quest.OnQuestCompleted += Quest_OnQuestCompleted;
        quest.OnQuestFailed += Quest_OnQuestFailed;
        tabInstance.localScale = Vector3.one;
        tabInstance.anchoredPosition = Vector3.zero;
        var questNameInstance = tabInstance.GetComponent<QuestNameTab>();
        if (questNameInstance == null)
        {
            questNameInstance = tabInstance.gameObject.AddComponent<QuestNameTab>();            
        }
        activeQuestDict.Add(quest, questNameInstance);
        questNameInstance.SetQuest(quest);
        questNameInstance.UpdateText(activeQuestDict.Count);
    }

    private void CreateFinishedQuestTab(QuestSO quest, bool isCompleted)
    {
        RectTransform tabInstance = new RectTransform();
        if (isCompleted)
        {
            tabInstance = Instantiate(tabPrefab, questListPanels[1]) as RectTransform;
        }
        else
        {
            tabInstance = Instantiate(tabPrefab, questListPanels[2]) as RectTransform;
        }
        tabInstance.localScale = Vector3.one;
        tabInstance.anchoredPosition = Vector3.zero;
        var questNameInstance = tabInstance.GetComponent<QuestNameTab>();
        if (questNameInstance == null)
        {
            questNameInstance = tabInstance.gameObject.AddComponent<QuestNameTab>();
        }
        questNameInstance.SetQuest(quest);       
        if (isCompleted)
        {
            completedQuestList.Add(questNameInstance);
            questNameInstance.UpdateText(completedQuestList.Count);
        }
        else
        {
            failedQuestList.Add(questNameInstance);
            questNameInstance.UpdateText(failedQuestList.Count);
        }
        questNameInstance.MarkFinished();
    }

    public void ShowQuestInfo(QuestSO quest)
    {       
        titleText.text = quest.Name;
        descriptionText.text = quest.QuestDescription;
        goalText.text = quest.GoalDescription;
        scrollbar1.value = 1;
        scrollbar2.value = 1;
    }

    public void UnHighlightAllTabs()
    {
        foreach (var tab in activeQuestDict.Values)
        {
            tab.UnHighlight();
        }
    }

    private void DisablePanels()
    {
        foreach (var panel in panelGOList)
        {
            panel.SetActive(false);
        }
    }

    private void EnablePanel()
    {
        panelGOList[panelIndex].SetActive(true);
    }

    private void Unhighlight()
    {
        foreach (var text in buttonTexts)
        {
            text.color = Color.white;
        }
    }

    private void Highlight()
    {
        buttonTexts[panelIndex].color = Color.red;
    }

    //~~~~~~~~~~~~~~~~ Calling from UI ~~~~~~~~~~~~~~~~

    public void OnPressActiveTab()
    {
        if (panelIndex != 0)
        {
            DisablePanels();
            Unhighlight();
            panelIndex = 0;
            EnablePanel();
            Highlight();
        }        
    }

    public void OnPressCompletedTab()
    {
        if (panelIndex != 1)
        {
            DisablePanels();
            Unhighlight();
            panelIndex = 1;
            EnablePanel();
            Highlight();
        }        
    }

    public void OnPressFailedTab()
    {
        if (panelIndex != 2)
        {
            DisablePanels();
            Unhighlight();
            panelIndex = 2;
            EnablePanel();
            Highlight();
        }        
    }

    //~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~

    private void Quest_OnQuestFailed(string arg1, QuestSO quest)
    {
        AssetLoader.ReleaseAssetInstance(activeQuestDict[quest].gameObject);
        CreateFinishedQuestTab(quest, false);
        quest.OnQuestCompleted -= Quest_OnQuestCompleted;
        quest.OnQuestFailed -= Quest_OnQuestFailed;
    }

    private void Quest_OnQuestCompleted(string arg1, QuestSO quest)
    {
        AssetLoader.ReleaseAssetInstance(activeQuestDict[quest].gameObject);
        CreateFinishedQuestTab(quest, true);
        quest.OnQuestCompleted -= Quest_OnQuestCompleted;
        quest.OnQuestFailed -= Quest_OnQuestFailed;
    }
}
