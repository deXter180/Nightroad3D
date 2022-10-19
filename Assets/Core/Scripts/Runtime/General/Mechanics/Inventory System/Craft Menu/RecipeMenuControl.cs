using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeMenuControl : MonoBehaviour
{
    #region Variables

    [SerializeField] private RecipeMenuTab recipeTab;
    [SerializeField] private RectTransform contentField;
    private RecipeManager recipeManager;
    private RecipeDescription recipeDescription;

    #endregion

    #region General

    private void Awake()
    {
        recipeDescription = GetComponentInChildren<RecipeDescription>();
    }

    private void OnEnable()
    {
        recipeDescription.Setup();
        if (recipeManager == null)
        {
            recipeManager = RecipeManager.Instance;
        }
        if (recipeManager != null && recipeManager.IsDirty && recipeManager.Recipes.Count > 0)
        {            
            contentField.DeleteChildren();
            for (int i = 0; i < recipeManager.Recipes.Count; i++)
            {
                var recipeTabInstance = Instantiate(recipeTab, contentField);
                recipeTabInstance.UpdateRecipeSO(recipeManager.Recipes[i]); 
            }
            recipeManager.ResetDirty();
        }
    }

    private void OnDisable()
    {
        if (recipeManager != null)
            recipeManager.ResetDirty();
    }

    #endregion
}
