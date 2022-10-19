using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeDecsTab : MonoBehaviour
{
    #region Variables

    [SerializeField] private TMP_Text recipeText;
    [SerializeField] private TMP_Text amountText;

    #endregion

    #region General

    public void UpdateRecipeDecsTab(string text1, string text2)
    {
        recipeText.text = text1;
        amountText.text = text2;
    }

    #endregion
}
