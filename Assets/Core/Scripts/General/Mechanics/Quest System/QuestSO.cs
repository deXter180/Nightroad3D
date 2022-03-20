using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/New")]
public class QuestSO : ScriptableObject
{
    private Timer _timer;
    [Header("QuestData")]
    [SerializeField] private List<GoalItem> _goalItems;
    [SerializeField] private RewardItems _rewardItem;    
    [SerializeField] private string _name;
    [TextArea(2, 10)] [SerializeField] private string _description;
    [SerializeField] private bool _isComplex;
    [SerializeField] private bool _isWithTimer;
    [SerializeField] private int _durationInMinutes;
    [SerializeField] private int _XPRewardAmount;
    public List<Goal> Goals { get; private set; } = new List<Goal>();
    public string TimeLeft { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsFailed { get; private set; }
    public RewardItems RewardItem => _rewardItem;
    public string Name => _name;
    public string Description => _description;
    public bool IsComplex => _isComplex;
    public bool IsWithTimer => _isWithTimer;
    public int DurationInMinutes => _durationInMinutes;
    public int XPRewardAmount => _XPRewardAmount;
    public Timer QuestTimer => _timer;

    public static event Action<QuestSO> OnQuestStart;
    public static event Action<QuestSO> OnQuestCompletion;
    public static event Action<QuestSO> OnQuestFailure;

    public void StartQuest()
    {
        IsCompleted = false;
        IsFailed = false;
        SetGoals();
        SetTimer();
        OnQuestStart?.Invoke(this);
    }

    private void SetGoals()
    {
        foreach(var goalItem in _goalItems)
        {
            switch (goalItem.GoalType)
            {
                case (GoalTypes.Assassination):
                    {
                        Goals.Add(new AssassinationGoal(
                        goalItem.EnemyInfo.EnemyID,
                        goalItem.EnemyInfo.EnemyName,
                        goalItem.EnemyInfo.EnemyType
                        ));
                    }                    
                    break;
                case (GoalTypes.Collect):
                    {

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
            IsCompleted = Goals.All(g => g.IsCompleted);
            if (IsCompleted)
            {
                OnQuestCompletion?.Invoke(this);
            }
        }
        else
        {
            if(_timer != null)
            {
                if (_timer.IsTimeLeft)
                {
                    _timer.StartTimer();
                    TimeLeft = _timer.TimeString;
                    IsCompleted = Goals.All(g => g.IsCompleted);
                    if (IsCompleted)
                    {

                        OnQuestCompletion?.Invoke(this);
                        _timer = null;
                    }
                }
                else
                {
                    if (!IsCompleted)
                    {
                        IsFailed = true;
                        OnQuestFailure?.Invoke(this);
                        _timer = null;
                    }
                }
            }
            
        }
       
    }

    public void GiveReward()
    {
        //Add item to inventory
    }
}

[Serializable]
public class GoalItem
{
    public GoalTypes GoalType;
    public int RequiredAmount;
    public EnemyInfoItem EnemyInfo;
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