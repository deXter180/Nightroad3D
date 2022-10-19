using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelSystemManager : Singleton<LevelSystemManager>
{
    #region Variables

    private AttributeUI attributeUI;
    [SerializeField] private List<int> RequiredXP;
    private List<LevelSystem> levelSystems;
    private LevelSystem currentLevel;
    private int xp;
    
    public int CurrentXP => xp;
    public static event Action<int> OnLevelUp;
    public event Action OnXPChanged;

    #endregion

    #region Mechanics

    protected override void Awake()
    {
        base.Awake();
        attributeUI = GetComponentInChildren<AttributeUI>(); 
    }

    private void AddLevels()
    {
        for (int i = 0; i < RequiredXP.Count; i++)
        {           
            levelSystems.Add(new LevelSystem(i + 1, RequiredXP[i]));
            if (i == 0)
            {
                currentLevel = levelSystems[i];
            }
        }

    }

    public void AddXP(int amount)
    {
        if (currentLevel.Level < levelSystems.Count)
        {
            xp += amount;
            while (xp >= currentLevel.XPToNextLevel)
            {
                int lastLevel = currentLevel.Level;
                attributeUI.ResetXPSlider();
                currentLevel.MarkCompleted();
                xp -= currentLevel.XPToNextLevel;
                if (!levelSystems[lastLevel].IsCompleted)
                {
                    currentLevel = levelSystems[lastLevel];
                }
                else
                {
                    for (int i = 0; i < levelSystems.Count; i++)
                    {
                        if (!levelSystems[i].IsCompleted)
                        {
                            currentLevel = levelSystems[i];
                        }
                    }
                }
                OnLevelUp?.Invoke(currentLevel.Level);
            }            
        }
        else
        {
            if (xp + amount <= currentLevel.XPToNextLevel)
            {
                xp += amount;
            }
            else
            {
                int restVlaue = currentLevel.XPToNextLevel - xp;
                xp += restVlaue;
            }
        }
        OnXPChanged?.Invoke();
    }

    public void ResetLevel()
    {
        xp = 0;
        levelSystems = new List<LevelSystem>();
        AddLevels();
        attributeUI.CompleteResetXPSlider();
        attributeUI.UpdateLevelNumText(currentLevel.Level);
        //OnLevelUp?.Invoke(currentLevel.Level);

        //Delete this later
        AddXP(35);
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return Helpers.GetWait(5f);
            AddXP(100);
        }
    }

    public float GetPctXP()
    {
        float pct = (float) xp / currentLevel.XPToNextLevel;
        return (float)Math.Round(pct, 2);
    }

    #endregion
}
