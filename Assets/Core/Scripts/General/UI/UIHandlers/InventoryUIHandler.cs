using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InventoryUIHandler : Singleton<InventoryUIHandler>
{
    [SerializeField] private Transform BackgroundImage;
    [SerializeField] private GridLayoutGroup inventoryLayout;
    [SerializeField] private GridLayoutGroup stashLayout;
    private Vector3 screenPos;
    private StashClose stashClose;
    private InventorySystem inventorySystem;
    private ItemStash itemStash;
    private EquipMenuControl equipMenu;
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
        stashClose = GetComponentInChildren<StashClose>();
    }

    private void Start()
    {
        stashClose.gameObject.SetActive(false);
        
        canvas.enabled = false;
        inventorySystem = InventorySystem.Instance;
        equipMenu = EquipMenuControl.Instance;
        itemStash = ItemStash.Instance;
        CreateGridBackGround();
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

    public void ControlInv(bool isInventoryActive)
    {
        if (!isInventoryActive)
        {
            canvas.enabled = true;
            ActivateInventory(true);
        }
        else
        {
            ActivateInventory(false);
            canvas.enabled = false;
            TooltipSystem.Hide();
        }
    }

    public void ControlSth(bool isInventoryActive)
    {
        if (!isInventoryActive)
        {
            canvas.enabled = true;
            ActivateStash(true);
            stashClose.gameObject.SetActive(true);
        }
        else
        {
            ActivateStash(false);
            stashClose.gameObject.SetActive(false);
            canvas.enabled = false;
            TooltipSystem.Hide();
        }
    }

    private void ActivateStash(bool isActive)
    {
        Initialize();
        equipMenu.gameObject.SetActive(false);
        AffectInventory(isActive);
        AffectStash(isActive);               
    }

    private void ActivateInventory(bool isActive)
    {
        Initialize();
        AffectStash(false);
        AffectInventory(isActive);
        equipMenu.gameObject.SetActive(isActive);       
    }

    private void Initialize()
    {
        if (inventorySystem == null)
            inventorySystem = InventorySystem.Instance;
        if (equipMenu == null)
            equipMenu = EquipMenuControl.Instance;
        if (itemStash == null)
            itemStash = ItemStash.Instance;
    }

    private void AffectInventory(bool isActive)
    {
        inventoryLayout.gameObject.SetActive(isActive);
        inventorySystem.gameObject.SetActive(isActive);        
    }

    private void AffectStash(bool isActive)
    {
        stashLayout.gameObject.SetActive(isActive);
        itemStash.gameObject.SetActive(isActive);       
    }

    private void CreateGridBackGround()
    {
        for (int x = 0; x < inventorySystem.GetWidth(); x++)
        {
            for (int y = 0; y < inventorySystem.GetHeight(); y++)
            {
                Transform backgrnd = Instantiate(BackgroundImage, inventoryLayout.gameObject.transform);
                backgrnd.gameObject.SetActive(true);
            }
        }

        for (int x = 0; x < itemStash.GetWidth(); x++)
        {
            for (int y = 0; y < itemStash.GetHeight(); y++)
            {
                Transform backgrnd = Instantiate(BackgroundImage, stashLayout.gameObject.transform);
                backgrnd.gameObject.SetActive(true);
            }

        }
    }
}