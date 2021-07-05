using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    public bool IsInventoryON { get; set; }
    private Canvas canvas;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else Instance = this;
        canvas = GetComponentInChildren<Canvas>();
    }

    private void Start()
    {
        canvas.enabled = false;
        IsInventoryON = false;
    }

    public void Control(Action action)
    {
        if (!IsInventoryON)
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
        action.Invoke();
    }
}

