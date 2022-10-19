using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeDescription : Singleton<RecipeDescription>
{
    #region Variables

    [SerializeField] private List<RecipeDecsTab> InputTabs;
    [SerializeField] private RecipeDecsTab OutputTab;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
    }

    public void Setup()
    {
        foreach (var tab in InputTabs)
        {
            tab.gameObject.SetActive(false);
        }
        OutputTab.gameObject.SetActive(false);
    }

    public void UpdateRecipeText(RecipeSO recipeSO)
    {
        for (int i = 0; i < recipeSO.InputItems.Count; i++)
        {
            var inputItem = recipeSO.InputItems[i];
            InputTabs[i].UpdateRecipeDecsTab(inputItem.ItemType.ToString(), inputItem.Amount.ToString());
            InputTabs[i].gameObject.SetActive(true);
        }
        var outputItem = recipeSO.OutputItem;
        OutputTab.UpdateRecipeDecsTab(outputItem.ItemType.ToString(), outputItem.Amount.ToString());
        OutputTab.gameObject.SetActive(true);
    }

    #endregion
}
