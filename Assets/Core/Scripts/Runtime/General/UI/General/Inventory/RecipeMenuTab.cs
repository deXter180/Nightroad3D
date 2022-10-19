using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeMenuTab : MonoBehaviour, IPointerClickHandler
{
    #region Variables

    private TMP_Text recipeText;
    private RecipeSO recipeSO;
    private RecipeDescription recipeDescription;

    #endregion

    #region General

    private void Awake()
    {        
        recipeText = GetComponentInChildren<TMP_Text>();
    }

    private void UpdateText(string text)
    {
        recipeText.text = text;
    }

    public void UpdateRecipeSO(RecipeSO recipe)
    {
        recipeDescription = RecipeDescription.Instance;
        recipeSO = recipe;
        UpdateText(recipeSO.RecipeName);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (recipeSO != null)
        {
            recipeDescription.UpdateRecipeText(recipeSO);
        }      
    }

    #endregion
}
