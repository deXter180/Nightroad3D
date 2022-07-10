using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashHolder : MonoBehaviour
{
    private ItemStash itemStash;
    private string hightlightMaterial = "Outline_SpriteLit";
    [SerializeField] private List<StashItems> StashItemList;
    private bool isOpened;
    private List<SavedStashItems> savedStashItemList;
    private SpriteRenderer SR;
    private GameController gameController;
    private Material defaultMat;

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        gameController = GameController.Instance;
        itemStash = ItemStash.Instance;
        savedStashItemList = new List<SavedStashItems>();        
        isOpened = false;
        defaultMat = SR.material;
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
        savedStashItemList = new List<SavedStashItems>();
        itemStash.ClearStash(savedStashItemList);
    }

    public void Highlight(Vector3 position)
    {
        SR.material = GameController.GetMaterail(hightlightMaterial);
        gameController.HighlightStash(position);
    }

    public void Unhighlight()
    {
        SR.material = defaultMat;
        gameController.UnHighlightStash();
    }

}
