using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Item", menuName = "CraftRecipe/Item")]
public class RecipeSO : ScriptableObject
{
    #region SerializedVariables

    [SerializeField][TextArea] private string recipeName;
    [SerializeField] private List<RecipeItems> inputItems;
    [SerializeField] private RecipeItems outputItem;
    [SerializeField] private RecipeTypes recipeType;
    [SerializeField] private Transform pickedRecipe;

    #endregion

    #region Properties

    public string RecipeName => recipeName;
    public List<RecipeItems> InputItems => inputItems;
    public RecipeItems OutputItem => outputItem;
    public RecipeTypes RecipeType => recipeType;
    public Transform PickedRecipe => pickedRecipe;

    #endregion

    #region General

    public bool IsInputTypeMatching(RecipeItems recipeItem)
    {
        for (int i = 0; i < inputItems.Count; i++)
        {
            if (inputItems[i].ItemType == recipeItem.ItemType)
            {
                return true;
            }
        }
        return false;
    }

    #endregion
}
