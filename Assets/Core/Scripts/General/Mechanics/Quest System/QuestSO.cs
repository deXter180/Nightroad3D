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
    private string questComplete = "Completed";
    private string questFail = "Failed";
    [Header("QuestData")]        
    [SerializeField] [Tooltip("Name of the Quest")] private string _name;
    [SerializeField] [Tooltip("Name of Ink variable to track status of this quest")] private string stateVariableName;
    [TextArea(2, 10)] [SerializeField] private string _description;
    [TextArea(2, 4)] [SerializeField] [Tooltip("Text to show if all goals of this quest are completed")] private string allGoalsCompletionText;
    [TextArea(2, 4)] [SerializeField] [Tooltip("Text to show if any goal of this quest is incomplete")] private string anyGoalFailureText;
    [TextArea(2, 4)] [SerializeField] [Tooltip("Text to show if this quest is completed")] private string questCompletionText;
    [TextArea(2, 4)] [SerializeField] [Tooltip("Text to show if this quest is failed")] private string questFailureText;  
    [SerializeField] private bool _isComplex;
    [SerializeField] private bool _isWithTimer;
    [SerializeField] private int _durationInMinutes;
    [SerializeField] private int _XPRewardAmount;   
    [SerializeField] private List<GoalItem> _goalItems;
    [SerializeField] private RewardItems _rewardItem;
    public List<Goal> Goals { get; private set; } = new List<Goal>();
    public string TimeLeft { get; private set; }
    public bool AllGoalsSatisfied { get; private set; }
    public bool IsCompleted { get; set; }
    public bool IsFailed { get; set; }
    public RewardItems RewardItem => _rewardItem;
    public string Name => _name;
    public string StateVariableName => stateVariableName;
    public string Description => _description;
    public string AllGoalsCompletionText => allGoalsCompletionText;
    public string AnyGoalFailureText => anyGoalFailureText;
    public string QuestCompletionText => questCompletionText;
    public string QuestFailureText => questFailureText;   
    public bool IsComplex => _isComplex;
    public bool IsWithTimer => _isWithTimer;
    public int DurationInMinutes => _durationInMinutes;
    public int XPRewardAmount => _XPRewardAmount;
    public Timer QuestTimer => _timer;
    public Story QuestStory => questStory;
    public event Action<string, QuestSO> OnQuestGoalsCompleted;
    public event Action<string, QuestSO> OnQuestGoalsFailed;

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
        bool temp = AllGoalsSatisfied;
        AllGoalsSatisfied = Goals.All(g => g.IsCompleted);
        if (AllGoalsSatisfied != temp)
        {
            if (AllGoalsSatisfied)
                OnQuestGoalsCompleted?.Invoke(allGoalsCompletionText, this);
            else
            {
                OnQuestGoalsFailed?.Invoke(anyGoalFailureText, this);
            }
        }       
    }

    public void CheckCurrentState(string varName, object state)
    {
        string temp = (string)state;
        if (temp == questComplete)
        {
            IsCompleted = true;
            GiveReward();
        }
        else if (temp == questFail)
        {
            IsFailed = true;
        }
        if (IsCompleted || IsFailed)
        {
            QuestManager.ActiveQuestDict.Remove(this.name);
            GoalsCloser();
            if (QuestManager.AllQuestsCompletedInStory(questStory))
            {
                questStory.RemoveVariableObserver();
            }           
        }
        
    }

    private void GoalsCloser()
    {
        foreach (var goal in Goals)
        {
            goal.Closer();
        }
        Goals.Clear();
    }

    private void GiveReward()
    {
        //Add item to inventory
    }
}

[Serializable]
public class GoalItem
{
    public GoalTypes GoalType;
    public int RequiredAmount;
    public string Description;
    public string GoalCompletionText;
    public string GoalFailureText;
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