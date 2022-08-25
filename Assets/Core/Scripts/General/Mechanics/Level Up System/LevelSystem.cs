using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem 
{
    private int level;
    private int xpToNextLevel;
    private bool isCompleted;
    public int Level => level;
    public int XPToNextLevel => xpToNextLevel;
    public bool IsCompleted => isCompleted;
    public LevelSystem(int levelNum, int xpToCompleteLevel)
    {
        isCompleted = false;
        level = levelNum;
        xpToNextLevel = xpToCompleteLevel;
    }

    public void MarkCompleted()
    {
        isCompleted = true;
    }
}
