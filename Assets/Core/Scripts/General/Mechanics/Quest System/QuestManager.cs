using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    public string QuestText { get; private set; }
    public string GoalText { get; private set; }
    public static QuestManager Instance { get; private set; }   
    public static Dictionary<string, QuestSO> ActiveQuestDict = new Dictionary<string, QuestSO>();
    public static Dictionary<Story, List<QuestSO>> AllQuestInStoryDict = new Dictionary<Story, List<QuestSO>>();

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

    private void Update()
    {
        CheckForQuestCompletion();
    }

    public bool StartNewQuest(string questName, Story story)
    {
        QuestSO quest = AssetCollections.GetQeust(questName);
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

    //~~~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~~~~ 

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
}
