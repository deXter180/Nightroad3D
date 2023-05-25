using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStop : Singleton<TimeStop>
{
    #region Variables

    private float speed;
    private bool isRestoreTime;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        isRestoreTime = false;
    }

    private void Update()
    {
        if (isRestoreTime)
        {
            if (Time.timeScale < 1f)
            {
                Time.timeScale += Time.deltaTime * speed;
            }
            else
            {
                Time.timeScale = 1f;
                isRestoreTime = false;
            }
        }
    }

    #endregion

    #region Mechanics

    public void StopTime(float changeTime, int restoreSpeed, float delay)
    {
        speed = restoreSpeed;

        if (delay > 0)
        {
            StopCoroutine(StartTimeAgaing(delay));
            StartCoroutine(StartTimeAgaing(delay));
        }
        else
        {
            isRestoreTime = true;
        }
        Time.timeScale = changeTime;

        IEnumerator StartTimeAgaing(float amount)
        {            
            yield return Helpers.GetRealtimeWait(amount);
            isRestoreTime = true;
        }
    }

    #endregion
}
