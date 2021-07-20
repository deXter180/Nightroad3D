using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InventoryUIHandler : MonoBehaviour
{
    public static InventoryUIHandler Instance { get; private set; }
    public bool IsInventoryON { get; set; }
    private Canvas canvas;
    private GridLayoutGroup gridLayout;
    private RectTransform UIRect;
    [SerializeField] Transform BackgroundImage;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else Instance = this;
        canvas = GetComponentInChildren<Canvas>();
        gridLayout = GetComponentInChildren<GridLayoutGroup>();
        UIRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        canvas.enabled = false;
        IsInventoryON = false;
        CreateGridBackGround();
        
    }

    public RectTransform GetUIRect()
    {
        return UIRect;
    }

    private void CreateGridBackGround()
    {
        for (int x = 0; x < InventorySystem.Instance.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < InventorySystem.Instance.GetGrid().GetHeight(); y++)
            {
                Transform backgrnd = Instantiate(BackgroundImage, gridLayout.gameObject.transform);
                backgrnd.gameObject.SetActive(true);
            }
        }
    }

    public void Control()
    {
        if (!IsInventoryON)
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
    }
}

