using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InventoryUIHandler : Singleton<InventoryUIHandler>
{
    private Canvas canvas;  
    private RectTransform UIRect;
    public Transform gridVisual;
    private RectTransform canvasRect;
    [SerializeField] private float CellSize;

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponentInChildren<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        UIRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        canvas.enabled = false;   
    }

    public RectTransform GetCanvasTransfrom()
    {
        return canvasRect;
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    public RectTransform GetUIRect()
    {
        return UIRect;
    }

    public float GetCellSize()
    {
        return CellSize;
    }

    public void Control(bool isInventoryActive)
    {
        if (!isInventoryActive)
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
    }
}