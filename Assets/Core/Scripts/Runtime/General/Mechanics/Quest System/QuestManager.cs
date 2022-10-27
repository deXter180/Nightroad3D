using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using System.Linq;

public class QuestManager : Singleton<QuestManager>
{
    #region Variables

    private QuestMenuManager questMenu;
    public string QuestText { get; private set; }
    public string GoalText { get; private set; }
    public static Dictionary<string, QuestSO> ActiveQuestDict = new Dictionary<string, QuestSO>();
    public static Dictionary<Story, List<QuestSO>> AllQuestInStoryDict = new Dictionary<Story, List<QuestSO>>();

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        questMenu = QuestMenuManager.Instance;
    }

    private void Update()
    {
        CheckForQuestCompletion();
    }

    #endregion

    #region Mechanics

    public bool StartNewQuest(string questName, Story story)
    {
        QuestSO quest = AssetLoader.GetQeust(questName);
        if (quest != null)
        {
            if (AllQuestInStoryDict.ContainsKey(story))
            {
                AllQuestInStoryDict[story].Add(quest);
            }
            else
            {
                List<QuestSO> questList = new List<QuestSO>();
                questList.Add(quest);
                AllQuestInStoryDict.Add(story, questList);
            }
            quest.StartQuest();
            quest.SetStoryName(story);
            ActiveQuestDict.Add(quest.name, quest);
            quest.OnQuestCompleted += OnQuestGoalsCompleted;
            quest.OnQuestFailed += OnQuestGoalsFailed;
            foreach(var g in quest.Goals)
            {
                g.OnGoalCompletion += OnGoalCompletion;
                g.OnGoalFailure += OnGoalFailPostCompletion;
                g.Initialize();
            }
            questMenu.CreateActiveQuestTab(quest);
            return true;
        }
        return false;
    }   

    private void CheckForQuestCompletion()
    {
        foreach (var q in ActiveQuestDict.Values)
        {
            q.CheckGoals();
        }
    }

    public void ResetAllQuests()
    {
        foreach (var q in ActiveQuestDict.Values)
        {
            q.ResetQuest();
        }
        ActiveQuestDict.Clear();
        AllQuestInStoryDict.Clear();
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~~~~ 

    #region Callbacks

    private void OnQuestGoalsCompleted(string obj, QuestSO quest)
    {
        QuestText = obj;
        Debug.Log(QuestText);
    }

    private void OnQuestGoalsFailed(string obj, QuestSO quest)
    {
        QuestText = obj;
        Debug.Log(QuestText);
    }
  
    private void OnGoalFailPostCompletion(string obj, Goal goal)
    {
        GoalText = obj;
        Debug.Log(GoalText);
    }

    private void OnGoalCompletion(string obj, Goal goal)
    {
        GoalText = obj;
        Debug.Log(GoalText);
    }

    #endregion
}
