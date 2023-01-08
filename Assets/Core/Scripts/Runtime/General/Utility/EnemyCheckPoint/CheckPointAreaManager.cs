using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointAreaManager : Singleton<CheckPointAreaManager>
{
    #region Variables

    [SerializeField] private List<CheckPointsArea> checkPointsAreas;
    private bool allCheckPointReady = false;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        foreach (var area in checkPointsAreas)
        {
            allCheckPointReady = area.IsReady;
        }
        if (allCheckPointReady)
        {
            CheckPointLOSManager.Instance.Setup();
        }
    }

    #endregion    
}
