using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Ink.Runtime;


[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/New")]
public class QuestSO : ScriptableObject
{
    private Timer _timer;
    private Story questStory;
    [Header("QuestData")]
    [SerializeField][Tooltip("ID of the Quest")] private string _id;
    [SerializeField] [Tooltip("Name of the Quest")] private string _name;
    [SerializeField] [Tooltip("Name main knot in INK story")] private string mainKnotName;
    [SerializeField] [Tooltip("Name of Completion INK variable to track status of this quest")] private string completionStateName;
    [SerializeField] [Tooltip("Name of Failure INK variable to track status of this quest")] private string failureStateName;
    [TextArea(2, 10)] [SerializeField] private string _questDescription;
    [TextArea(2, 10)][SerializeField] private string _goalDescription;
    [TextArea(2, 4)] [SerializeField] [Tooltip("Text to show if this quest is completed")] private string questCompletionText;
    [TextArea(2, 4)] [SerializeField] [Tooltip("Text to show if this quest is failed")] private string questFailureText;  
    [SerializeField] private bool _isComplex;
    [SerializeField] private bool _isWithTimer;
    [SerializeField] private int _durationInMinutes;
    [SerializeField] private List<GoalItem> _goalItems;
    [SerializeField] private RewardItems _rewardItem;
    public List<Goal> Goals { get; private set; } = new List<Goal>();
    public string TimeLeft { get; private set; }
    public bool AllGoalsSatisfied { get; private set; }
    public bool IsCompleted { get; set; }
    public bool IsFailed { get; set; }
    public RewardItems RewardItem => _rewardItem;
    public string QuestID => _id;
    public string Name => _name;
    public string QuestDescription => _questDescription;
    public string GoalDescription => _goalDescription;
    public string QuestCompletionText => questCompletionText;
    public string QuestFailureText => questFailureText;   
    public bool IsComplex => _isComplex;
    public bool IsWithTimer => _isWithTimer;
    public int DurationInMinutes => _durationInMinutes;
    public Timer QuestTimer => _timer;
    public Story QuestStory => questStory;
    public event Action<string, QuestSO> OnQuestCompleted;
    public event Action<string, QuestSO> OnQuestFailed;

    public void StartQuest()
    {
        AllGoalsSatisfied = false;
        IsFailed = false;
        IsCompleted = false;
        SetGoals();
        SetTimer();
        
    }

    public void SetStoryName(Story story)
    {
        questStory = story;
        questStory.onChoosePathString += Story_onChoosePathString;

        questStory.onError += QuestStory_onError;
    }

    private void SetGoals()
    {
        foreach(var goalItem in _goalItems)
        {
            Goal goal = null;
            switch (goalItem.GoalType)
            {
                case (GoalTypes.Assassination):
                    {
                        goal = new AssassinationGoal(
                        goalItem.EnemyInfo.EnemyID,
                        goalItem.EnemyInfo.EnemyName,
                        goalItem.EnemyInfo.EnemyType
                        );
                    }                    
                    break;
                case (GoalTypes.Collect):
                    {
                        goal = new CollectionGoal(goalItem.CollectInfo.item, goalItem.RequiredAmount);                 
                    }
                    break;
                case (GoalTypes.MassKillAny):
                    {

                    }
                    break;
                case (GoalTypes.MassKillType):
                    {

                    }
                    break;
                case (GoalTypes.Delivery):
                    {

                    }
                    break;
            }
            if (goal != null)
            {
                goal.OnGoalCompletion += Goal_OnGoalCompletion;
                goal.OnGoalFailure += Goal_OnGoalFailure;
                goal.Description = goalItem.Description;
                goal.GoalCompleteText = goalItem.GoalCompletionText;
                goal.GoalFailureText = goalItem.GoalFailureText;
                Goals.Add(goal);
            }            
        }

    }

    private void SetTimer()
    {
        if (_isWithTimer)
        {
            _timer = new Timer(DurationInMinutes);
            _timer.Initialize();
        }        
    }

    public void CheckGoals()
    {
        if (!_isWithTimer)
        {
            CheckGoalStatus();
        }
        else
        {
            if(_timer != null)
            {
                if (_timer.IsTimeLeft)
                {
                    _timer.StartTimer();
                    TimeLeft = _timer.TimeString;
                    CheckGoalStatus();
                    if (AllGoalsSatisfied)
                    {                        
                        _timer = null;
                    }
                }
                else
                {
                    if (!AllGoalsSatisfied)
                    {
                        IsFailed = true;
                        _timer = null;
                    }
                }
            }           
        }
       
    }

    private void CheckGoalStatus()
    {       
        AllGoalsSatisfied = Goals.All(g => g.IsCompleted);    
    }

    public void FinalizeState(bool isCompleted, MonoBehaviour mono)
    {
        questStory.ResetCallstack();
        if (isCompleted)
        {
            questStory.ChoosePathString(completionStateName);
            OnQuestCompleted?.Invoke(questCompletionText, this);
            GiveReward();
        }
        else
        {
            questStory.ChoosePathString(failureStateName);
            OnQuestFailed?.Invoke(questFailureText, this);
        }
        mono.StartCoroutine(EndQuest());
    }

    public IEnumerator EndQuest()
    {
        yield return Helpers.GetWait(5f);       
        GoalsCloser();
        questStory.onChoosePathString -= Story_onChoosePathString;
        QuestManager.ActiveQuestDict.Remove(this.name);
        QuestManager.AllQuestInStoryDict[questStory].Remove(this);
        questStory.ResetCallstack();
        questStory.ChoosePathString(mainKnotName);
    }

    private void GoalsCloser()
    {
        foreach (var goal in Goals)
        {
            goal.OnGoalCompletion -= Goal_OnGoalCompletion;
            goal.OnGoalFailure -= Goal_OnGoalFailure;
            goal.Closer();
        }
        Goals.Clear();
    }

    private void GiveReward()
    {
        //Add item to inventory
        Debug.Log("Got Rewarded !");
    }

    public void ResetQuest()
    {
        questStory = null;
        _timer = null;
        foreach (var g in Goals)
        {
            g.Closer();
        }
        Goals.Clear();
    }

    //~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~

    private void Story_onChoosePathString(string arg1, object[] arg2)
    {
        if (arg1.Equals(completionStateName))
        {
            
        }
        else if (arg1.Equals(failureStateName))
        {
            
        }
    }

    private void Goal_OnGoalFailure(string obj, Goal goal)
    {
        
    }

    private void Goal_OnGoalCompletion(string obj, Goal goal)
    {
        
    }

    private void QuestStory_onError(string message, Ink.ErrorType type) //Debugging
    {
        Debug.Log(message);
    }
}

[Serializable]
public class GoalItem
{
    public GoalTypes GoalType;
    public int RequiredAmount;
    [TextArea(2, 4)] [Tooltip("Goal Description")] public string Description;
    [TextArea(2, 4)] [Tooltip("Text to show if all goals of this quest are completed")] public string GoalCompletionText;
    [TextArea(2, 4)] [Tooltip("Text to show if any goal of this quest is incomplete")] public string GoalFailureText;
    public ColletionItem CollectInfo;
    public EnemyInfoItem EnemyInfo;
}

[Serializable]
public class ColletionItem
{
    public ItemTypes item;
}

[Serializable]
public class EnemyInfoItem
{
    public EnemyTypes EnemyType;
    public string EnemyID;
    public string EnemyName;
}

[Serializable]
public class RewardItems
{
    public List<InventoryItems> Items;
    public int XP;
}

[Serializable]
public class InventoryItems
{
    public ItemTypes Item;
    public int Amount;
}