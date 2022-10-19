using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float _duration;
    private float _startTime;
    public bool IsTimeLeft { get; private set; }
    public string TimeString { get; private set; }


    public Timer(int DurationInMin)
    {
        _duration = DurationInMin * 60;
    }

    public void Initialize()
    {
        IsTimeLeft = true;
        _startTime = Time.time;
    }

    public void StartTimer()
    {
        float t = Time.time - _startTime;
        float t1 = _duration - t;
        if (t1 > 0)
        {
            int i = ((int)t1 / 60);
            string minutes;
            if (i < 10)
            {
                minutes = $"0{i}";
            }
            else
            {
                minutes = i.ToString();
            }
            string seconds = (t1 % 60).ToString("f0");
            TimeString = $"{minutes} : {seconds}";
        }
        else
        {
            IsTimeLeft = false;
        }
    }

}
