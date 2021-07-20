using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairUIHandler : MonoBehaviour
{
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
    }

    private void Update()
    {
        HandleCrosshair();
    }

    private void HandleCrosshair()
    {
        if (InventoryUIHandler.Instance.IsInventoryON)
        {
            canvas.enabled = false;
        }
        else canvas.enabled = true;
    }
}
