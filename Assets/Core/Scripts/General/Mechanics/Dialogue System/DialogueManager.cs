using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Ink.Runtime;
using System;

public class DialogueManager : Singleton<DialogueManager>
{
    private Story activeStory;   
    private GameController gameController;
    private QuestManager questManager;
    private WaitForSeconds endDelay = new WaitForSeconds(10f);
    [SerializeField] private Transform dialoguePanel;
    [SerializeField] private Transform choicePanel;  
    [SerializeField] private Transform dialoguePrefab;
    [SerializeField] private Transform choicePrefab;
    [SerializeField] private Button endButton;
    [SerializeField] private Image dialoguebox;
    public static Dictionary<string, Story> ActiveStoryDict = new Dictionary<string, Story>();
    private Dictionary<Story, string> lastTextFromStoryDict = new Dictionary<Story, string>();

    protected override void Awake()
    {
        base.Awake();      
    }

    private void Start()
    {
        dialoguebox.gameObject.SetActive(false);       
        endButton.gameObject.SetActive(false);
        gameController = GameController.Instance;
        questManager = QuestManager.Instance;
    }

    private void OnEnable()
    {
        endButton.onClick.AddListener(() => EndConversation());
        SceneLoader.OnNewGameStart += SceneLoader_OnNewGameStart;
    }

    private void OnDisable()
    {
        endButton.onClick.RemoveAllListeners();
        SceneLoader.OnNewGameStart -= SceneLoader_OnNewGameStart;
    }

    public void SetupStory(TextAsset storyTextAsset)
    {
        if (!dialoguebox.isActiveAndEnabled)
        {
            activeStory = null;
            activeStory = new Story(storyTextAsset.text);
            ActiveStoryDict.Add(storyTextAsset.name, activeStory);
            activeStory.allowExternalFunctionFallbacks = true;
            activeStory.BindExternalFunction("StartQuest", (string name) => { StartQuest(name); });
            RefreshStory();
        }        
    }

    public void ContinueStory(TextAsset storyTextAsset)
    {
        if (!dialoguebox.isActiveAndEnabled)
        {
            if (ActiveStoryDict.TryGetValue(storyTextAsset.name, out Story story))
            {
                activeStory = story;
                if (QuestManager.AllQuestInStoryDict.TryGetValue(story, out List<QuestSO> quests))
                {
                    foreach(var q in quests)
                    {
                        if (q.AllGoalsSatisfied)
                        {
                            q.FinalizeState(true, this);                            
                        }
                        if (q.IsFailed)
                        {
                            q.FinalizeState(false, this);
                        }
                    }
                }
                RefreshStory();
            }
        }     
    }

    private void RefreshStory()
    {
        if (activeStory != null)
        {               
            dialoguebox.gameObject.SetActive(true);
            endButton.gameObject.SetActive(true);
            gameController.SetDialogueActive(true);
            ClearUI();
            Transform dialogueTransform = Instantiate(dialoguePrefab, dialoguePanel);
            TextMeshProUGUI textMesh = dialogueTransform.GetComponent<TextMeshProUGUI>();
            string text = "";
            if (activeStory.canContinue)
            {
                text = LoadNextStoryBlock();
                AddLastTextToDict(activeStory, text);
            }
            else
            {
                text = lastTextFromStoryDict[activeStory];
            }
            
            textMesh.text = text;

            if (activeStory.currentChoices.Count > 0)
            {
                foreach (Choice choice in activeStory.currentChoices)
                {
                    Transform choiceTransform = Instantiate(choicePrefab, choicePanel);
                    Button choiceButton = choiceTransform.GetComponent<Button>();
                    TextMeshProUGUI choiceText = choiceTransform.GetComponentInChildren<TextMeshProUGUI>();
                    choiceText.text = $" {choice.index + 1}.{choice.text}";
                    choiceButton.onClick.AddListener(() => OnClickChoiceButton(choice));
                }
            }
            StartCoroutine(DelayEnd());
        }       
    }

    private void OnClickChoiceButton(Choice choice)
    {
        activeStory.ChooseChoiceIndex(choice.index);
        RefreshStory();
    }

    private void ClearUI()
    {
        dialoguePanel.DeleteChildren();
        choicePanel.DeleteChildren();    
    }

    public void EndConversation()
    {       
        dialoguebox.gameObject.SetActive(false);
        endButton.gameObject.SetActive(false);
        gameController.SetDialogueActive(false);
    }

    private IEnumerator DelayEnd()
    {
        yield return endDelay;
        EndConversation();
    }

    private string LoadNextStoryBlock()
    {
        string text = "";
        text = activeStory.ContinueMaximally();
        return text;
    }

    private void AddLastTextToDict(Story story, string text)
    {
        if (lastTextFromStoryDict.ContainsKey(story))
        {
            lastTextFromStoryDict[story] = text;
        }
        else
        {
            lastTextFromStoryDict.Add(story, text);
        }
    }

    public void StartQuest(string name)
    {
        if (questManager.StartNewQuest(name, activeStory))
        {

        }
    }

    //~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~~~

    private void SceneLoader_OnNewGameStart()
    {
        activeStory = null;
        foreach (var s in ActiveStoryDict.Values)
        {
            var temp = QuestManager.AllQuestInStoryDict[s];
            foreach (var q in temp)
            {
                q.ResetQuest();
            }
        }
        questManager.ResetAllQuests();
        ActiveStoryDict.Clear();
        lastTextFromStoryDict.Clear();
    }
}
