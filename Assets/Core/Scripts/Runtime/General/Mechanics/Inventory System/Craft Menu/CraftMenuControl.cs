using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CraftMenuControl : Singleton<CraftMenuControl>
{
    #region Variables

    [SerializeField] private Transform inputPanel;
    [SerializeField] private RectTransform recipeDescPanel;
    [SerializeField] private RectTransform closeButton;
    [SerializeField] private Button recipeButton;
    private RecipeManager recipeManager;
    private CraftOutputPanel outputPanel;
    private RecipeCloseButton recipeClose;
    public static List<CraftMenuTile> CraftTileList = new List<CraftMenuTile>();
    public Transform InputPanelTransform => inputPanel;
    private InventorySystem inventorySystem;   

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        Initialize();
        recipeClose = GetComponentInChildren<RecipeCloseButton>();
        outputPanel = GetComponentInChildren<CraftOutputPanel>();
    }

    private void Start()
    {
        recipeManager = RecipeManager.Instance;
        inventorySystem = InventorySystem.Instance;
    }

    private void OnEnable()
    {
        recipeButton?.onClick.AddListener(OpenRecipeDesc);
        recipeClose.OnRecipeDecsClosed += RecipeClose_OnRecipeDecsClosed;
        recipeDescPanel.gameObject.SetActive(false);  
        outputPanel.gameObject.SetActive(false);
        foreach (var tile in CraftTileList)
        {
            tile.OnPlacedInCraftMenu += Tile_OnPlacedInCraftMenu;
            tile.OnRemovedInCraftMenu += Tile_OnRemovedInCraftMenu;
        }
    }

    private void OnDisable()
    {
        recipeButton?.onClick.RemoveAllListeners();
        recipeClose.OnRecipeDecsClosed += RecipeClose_OnRecipeDecsClosed;
        foreach (var tile in CraftTileList)
        {
            tile.OnPlacedInCraftMenu -= Tile_OnPlacedInCraftMenu;
            tile.OnRemovedInCraftMenu -= Tile_OnRemovedInCraftMenu;
        }
        ResetRevertMenu();
    }

    private void ChangeTooltip(bool isTrue)
    {
        foreach (var tile in CraftTileList)
        {
            if (tile.MenuPlacedObject != null)
            {
                tile.MenuPlacedObject.TooltipControl(isTrue);
                tile.MenuPlacedObject.StopDragging(isTrue);
            }
        }
    }

    public void OpenRecipeDesc()
    {
        transform.SetAsLastSibling();
        ChangeTooltip(true);
        inventorySystem.ChangeTooltip(true);
        closeButton.gameObject.SetActive(false);
        recipeDescPanel.gameObject.SetActive(true);
    }

    private void CloseRecipeDecs()
    {
        ChangeTooltip(false);
        inventorySystem.ChangeTooltip(false);
        recipeDescPanel.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(true);        
    }

    private void Initialize()
    {
        CraftMenuTile[] tileArray = GetComponentsInChildren<CraftMenuTile>();
        foreach (var tile in tileArray)
        {
            CraftTileList.Add(tile);
        }
    }

    private void ResetRevertMenu()
    {
        if (CraftTileList != null)
        {
            foreach (var tile in CraftTileList)
            {
                if (tile.MenuPlacedObject != null)
                {
                    var amount = inventorySystem.GetItemAmount(tile.MenuPlacedObject.GetItemType());
                    inventorySystem.RemoveFromInvDict(tile.MenuPlacedObject);
                    if (inventorySystem.TryAddingItem(tile.MenuPlacedObject.GetInventoryItemSO(), amount))
                    {
                        
                    }
                }
                tile.ResetTile();
            }
        }

    }

    public void ResetMenu()
    {
        if (CraftTileList != null)
        {
            foreach (var tile in CraftTileList)
            {
                tile.ResetTile();
            }
        }
    }

    public void Craft()
    {
        var recipeItems = new List<RecipeItems>();
        foreach (var tile in CraftTileList)
        {
            if (tile.ItemType != ItemTypes.None)
            {
                var amount = inventorySystem.GetItemAmount(tile.ItemType);
                if (amount > 0)
                {
                    var recipeItem = new RecipeItems(tile.ItemType, amount);
                    recipeItems.Add(recipeItem);
                }
            }
        }
        if (recipeItems.Count > 0)
        {
            if (recipeManager.IsRecipeValid(recipeItems, out ItemTypes IT))
            {
                bool isTrue = false;
                var tempList = new List<SavedItems>();
                foreach (var tile in CraftTileList)
                {
                    if (tile.MenuPlacedObject != null)
                    {
                        var item = new SavedItems(tile.MenuPlacedObject.GetInventoryItemSO(), tile.MenuPlacedObject.GetGridPos());
                        tempList.Add(item);
                        isTrue = tile.TryRemoveItem();
                    }                   
                }
                if (isTrue && IT != ItemTypes.None)
                {
                    var itemSO = GameController.GetInventoryItemSOFromList(IT);
                    if (inventorySystem.TryAddingItem(itemSO))
                    {
                        outputPanel.SetOutputItemImage(itemSO.AttributeIcon);
                        outputPanel.gameObject.SetActive(true);
                        tempList.Clear();
                    }
                    else
                    {
                        AfterFailCraft(recipeItems, tempList);
                    }                  
                }
                else
                {
                    AfterFailCraft(recipeItems, tempList);
                }
            }
        }
    }

    private bool AfterFailCraft(List<RecipeItems> recipeItems, List<SavedItems> savedItemList)
    {
        bool isTrue = false;
        for (int i = 0; i < recipeItems.Count; i++)
        {
            isTrue = CraftTileList[i].TryPlaceItem(savedItemList[i].InventorySO, savedItemList[i].Position, out PlacedObject PO);
        }
        return isTrue;
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void Tile_OnRemovedInCraftMenu(PlacedObject obj)
    {

    }

    private void Tile_OnPlacedInCraftMenu(PlacedObject obj)
    {

    }

    private void RecipeClose_OnRecipeDecsClosed()
    {
        CloseRecipeDecs();
    }

    #endregion

}
