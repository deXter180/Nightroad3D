using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    private string questStart = "Start";
    private string goalSatisfied = "GoalSatisfied";
    private string goalUnsatisfied = "GoalUnsatisfied";
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
                AllQuestInStoryDict.Add(story, questList);
            }
            quest.StartQuest();
            quest.SetStoryName(story);
            story.variablesState[quest.StateVariableName] = questStart;
            story.ObserveVariable(quest.StateVariableName, (string varName, object val) => {
                quest.CheckCurrentState(varName, val);
            });
            ActiveQuestDict.Add(quest.name, quest);
            quest.OnQuestGoalsCompleted += OnQuestGoalsCompleted;
            quest.OnQuestGoalsFailed += OnQuestGoalsFailed;
            foreach(var g in quest.Goals)
            {
                g.OnGoalCompletion += OnGoalCompletion;
                g.OnGoalFailPostCompletion += OnGoalFailPostCompletion;
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

    public static bool AllQuestsCompletedInStory(Story story)
    {
        bool temp = AllQuestInStoryDict[story].All(g => g.IsCompleted || g.IsFailed);
        if (temp)
        {
            AllQuestInStoryDict.Remove(story);
        }
        return temp;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~~~~

   

    private void OnQuestGoalsCompleted(string obj, QuestSO quest)
    {
        QuestText = obj;
        quest.QuestStory.variablesState[quest.StateVariableName] = goalSatisfied;
    }

    private void OnQuestGoalsFailed(string obj, QuestSO quest)
    {
        QuestText = obj;
        quest.QuestStory.variablesState[quest.StateVariableName] = goalUnsatisfied;
    }
  
    private void OnGoalFailPostCompletion(string obj)
    {
        GoalText = obj;
    }

    private void OnGoalCompletion(string obj)
    {
        GoalText = obj;
    }
}
