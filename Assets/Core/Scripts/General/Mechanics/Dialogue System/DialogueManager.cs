using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Ink.Runtime;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    private Vector3 screenPos;
    private Story activeStory;   
    private GameController gameController;
    private QuestManager questManager;
    private string stateInkList = "QuestStates"; 
    private WaitForSeconds endDelay = new WaitForSeconds(10f);
    [SerializeField] private Transform dialoguePanel;
    [SerializeField] private Transform choicePanel;  
    [SerializeField] private Transform dialoguePrefab;
    [SerializeField] private Transform choicePrefab;
    [SerializeField] private Button endButton;
    [SerializeField] private Image TalkUIimage;
    [SerializeField] private Image dialoguebox;
    public static Dictionary<string, Story> ActiveStoryDict = new Dictionary<string, Story>();
    private Dictionary<Story, string> lastTextFromStoryDict = new Dictionary<Story, string>();

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
    }

    private void Start()
    {
        dialoguebox.gameObject.SetActive(false);
        TalkUIimage.gameObject.SetActive(false);
        endButton.gameObject.SetActive(false);
        gameController = GameController.Instance;
        questManager = QuestManager.Instance;
    }

    private void OnEnable()
    {
        endButton.onClick.AddListener(() => EndConversation());
    }

    private void OnDisable()
    {
        endButton.onClick.RemoveAllListeners();
    }

    public void SetupStory(TextAsset storyTextAsset)
    {
        if (!dialoguebox.isActiveAndEnabled)
        {
            activeStory = null;
            activeStory = new Story(storyTextAsset.text);
            ActiveStoryDict.Add(storyTextAsset.name, activeStory);
            activeStory.allowExternalFunctionFallbacks = true;
            activeStory.BindExternalFunction("startQuest", (string name) => { StartQuest(name); });
            RefreshStory();
        }        
    }

    public void Highlight(Vector3 position)
    {
        TalkUIimage.gameObject.SetActive(true);
        screenPos = Helpers.MainCam.WorldToScreenPoint(position);
        TalkUIimage.transform.position = screenPos;
    }

    public void UnHighlight()
    {
        TalkUIimage.gameObject.SetActive(false);
    }

    public void ContinueStory(TextAsset storyTextAsset)
    {
        if (!dialoguebox.isActiveAndEnabled)
        {
            if (ActiveStoryDict.TryGetValue(storyTextAsset.name, out Story story))
            {
                activeStory = story;
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

    public void StartQuest(string name)
    {       
        if (questManager.StartNewQuest(name, activeStory))
        {         
            Debug.Log(activeStory.state.currentPathString);
        }
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

    //~~~~~~~~~~~~~~~~~~ Utility ~~~~~~~~~~~~~~~~~~~~

    private List<string> GetStateInkListItem()
    {
        List<string> tempList = new List<string>();
        var temp = activeStory.variablesState[stateInkList] as InkList;
        foreach (var item in temp.all)
        {
            tempList.Add(item.Key.itemName);
        }
        return tempList;
    }

    private void AddToStateInkList(string ListItemToAdd)
    {
        var tempList = activeStory.variablesState[stateInkList] as InkList;
        InkListItem listItem = new InkListItem(stateInkList, ListItemToAdd);
        tempList.AddItem(listItem);
        activeStory.variablesState[stateInkList] = tempList;
    }

    private void RemoveFromStateInkList(string ListItemToRemove)
    {
        var tempList = activeStory.variablesState[stateInkList] as InkList;
        InkListItem listItem = new InkListItem(stateInkList, ListItemToRemove);
        tempList.Remove(listItem);
        activeStory.variablesState[stateInkList] = tempList;
    }

}
