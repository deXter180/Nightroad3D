using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashHolder : MonoBehaviour
{
    #region Variables

    private ItemStash itemStash;
    //private string hightlightMaterial = "Outline_SpriteLit";
    [SerializeField] private List<StashItems> StashItemList;
    private bool isOpened;
    private List<SavedItems> savedStashItemList;
    private SpriteRenderer SR;
    private GameController gameController;
    //private Material defaultMat;

    #endregion

    #region General

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        if (SR == null)
        {
            SR = GetComponentInChildren<SpriteRenderer>();
        }
        gameController = GameController.Instance;
        itemStash = ItemStash.Instance;
        savedStashItemList = new List<SavedItems>();        
        isOpened = false;
        //defaultMat = SR.material;
    }

    public void LoadItemToStash()
    {        
        if (!isOpened)
        {
            if (!itemStash.IsStashFull())
            {
                itemStash.AddToStash(StashItemList);
                isOpened = true;
            }           
        }
        else
        {
            if (savedStashItemList != null)
            {
                itemStash.ReloadStash(savedStashItemList);
            }            
        }       
    }

    public void UnloadItemFromStash()
    {
        savedStashItemList = new List<SavedItems>();
        itemStash.ClearStash(savedStashItemList);
    }

    public void Highlight(float distance)
    {
        //SR.material = AssetLoader.GetMaterail(hightlightMaterial);
        gameController.HighlightSelection(SR.bounds, distance);
        gameController.HighlightStash(transform.position);
    }

    public void Unhighlight()
    {
        //SR.material = defaultMat;
        gameController.UnHighlightSelection();
        gameController.UnHighlightStash();
    }

    #endregion
}
