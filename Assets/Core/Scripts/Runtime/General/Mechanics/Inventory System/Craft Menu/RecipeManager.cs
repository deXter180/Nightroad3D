using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class RecipeManager : Singleton<RecipeManager>
{
    #region Variables

    private bool isDirty;
    public List<RecipeSO> Recipes;
    public bool IsDirty => isDirty;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize()
    {
        Recipes = new List<RecipeSO>();
        isDirty = true;
    }

    public void AddRecipe(RecipeSO recipeSO)
    {
        if (recipeSO != null)
        {
            Recipes.Add(recipeSO);
            recipeSO.InputItems.Sort();
            isDirty = true;
        }       
    }

    public void ResetDirty()
    {
        isDirty = false;
    }

    public bool IsRecipeValid(List<RecipeItems> recipeItems, out ItemTypes IT)
    {
        bool isTrue = false;
        IT = ItemTypes.None;
        for (int i = 0; i < Recipes.Count; i++)
        {
            if (Recipes[i].InputItems.Count == recipeItems.Count)
            {
                for (int j = 0; j < Recipes[i].InputItems.Count; j++)
                {
                    if (Recipes[i].InputItems[j].Equals(recipeItems[j]))
                    {
                        IT = Recipes[i].OutputItem.ItemType;
                        isTrue = true;
                    }
                    else
                    {
                        isTrue = false;
                        break;
                    }
                }
            }
            else continue;
        } 
        return isTrue;
    }

    #endregion
}

[Serializable]
public class RecipeItems : IEquatable<RecipeItems>, IComparable<RecipeItems>
{
    public int Amount;
    public ItemTypes ItemType;
    

    public RecipeItems (ItemTypes type, int amount)
    {
        ItemType = type;
        Amount = amount;
    }

    public int CompareTo(RecipeItems other)
    {
        int sortNum1 = (int)ItemType;
        int sortNum2 = (int)other.ItemType;      
        int temp = -1;
        if (sortNum1 < sortNum2)
            temp = -1;
        else if (sortNum1 > sortNum2)
            temp = 1;
        else if (sortNum2 == sortNum1)
            temp = 0;
        return temp;
    }

    public bool Equals(RecipeItems other)
    {
        return other is RecipeItems
            && other.ItemType == ItemType
            && other.Amount == Amount;
    }
}
