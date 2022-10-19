using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftStation : MonoBehaviour
{
    #region Variables

    private string hightlightMaterial = "Outline_SpriteLit";
    private SpriteRenderer SR;
    private GameController gameController;
    private Material defaultMat;

    #endregion

    #region General

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        gameController = GameController.Instance;
        defaultMat = SR.material;
    }

    public void EnterCrafting()
    {
        gameController.OpenCrafting();
    }

    #endregion
}
