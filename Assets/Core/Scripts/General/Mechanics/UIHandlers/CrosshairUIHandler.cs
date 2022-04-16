using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairUIHandler : MonoBehaviour
{
    public static CrosshairUIHandler Instance { get; private set; }

    private Canvas canvas;

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
        canvas = GetComponentInChildren<Canvas>();
    }

    public void Control(bool isCrosshairActive)
    {
        canvas.enabled = isCrosshairActive;
    }
}
