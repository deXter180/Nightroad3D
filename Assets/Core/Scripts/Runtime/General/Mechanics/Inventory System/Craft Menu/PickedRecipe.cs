using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedRecipe : MonoBehaviour
{
    #region Variables

    private string hightlightMaterial = "Outline_SpriteLit";
    private SpriteRenderer SR;
    private BoxCollider col;
    private Material defaultMaterial;
    private RecipeSO recipeSO;
    private RecipeTypes recipeType;
    public RecipeSO Recipe => recipeSO;
    public RecipeTypes RecipeType => recipeType;

    #endregion

    #region General

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
        SR = GetComponentInChildren<SpriteRenderer>();
        if (SR != null)
        {
            defaultMaterial = SR.material;
        }
    }

    private void OnEnable()
    {
        col.isTrigger = true;
    }

    #endregion

    #region Mechanics

    public void HighlightObject()
    {
        if (SR != null)
        {
            SR.material = GameController.GetMaterail(hightlightMaterial);
        }

    }

    public void UnhighlightObject()
    {
        if (SR != null)
        {
            SR.material = defaultMaterial;
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public static PickedRecipe SpawnItemsWorld(RecipeSO recipeSO, Vector3 position)
    {
        float posX = position.x;
        float posZ = position.z;
        Vector3 groundPos = new Vector3(posX, PlayerController.Instance.GroundHeight, posZ);
        var type = recipeSO.RecipeType;
        Transform spawnedTransform = Instantiate(recipeSO.PickedRecipe, groundPos, Quaternion.identity);
        if (spawnedTransform != null)
        {
            var pickedRecipe = spawnedTransform.GetComponent<PickedRecipe>();
            pickedRecipe.recipeSO = recipeSO;
            pickedRecipe.SetupType(type);
            return pickedRecipe;
        }
        return null;
    }

    private void SetupType(RecipeTypes RT)
    {
        recipeType = RT;
    }

    #endregion
}
