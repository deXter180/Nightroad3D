using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestNameTab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questText;
    private Color defaultColor;
    public QuestSO quest { get; private set; }
    private bool isSelected;
    private bool isFinished;
    private QuestMenuManager questManager;

    private void OnEnable()
    {       
        questManager = QuestMenuManager.Instance;
        defaultColor = questText.color;
        isSelected = false;
        isFinished = false;
    }

    public void SetQuest(QuestSO questSO)
    {
        quest = questSO;
    }

    public void UpdateText(int questNum)
    {
        questText.text = $"{questNum.ToString()}. {quest.Name}";
    }

    public void Highlight()
    {
        questManager.UnHighlightAllTabs();
        if (!isSelected && !isFinished)
        {
            questText.color = Color.red;
            isSelected = true;
            questManager.ShowQuestInfo(quest);
        }
        
    }

    public void UnHighlight()
    {
        if (isSelected && !isFinished)
        {
            questText.color = defaultColor;
            isSelected = false;
        }       
    }

    public void MarkFinished()
    {
        questText.color = Color.gray;
        isFinished = true;
    }
   
}
