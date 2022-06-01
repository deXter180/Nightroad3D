using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashHolder : MonoBehaviour
{
    private ItemStash itemStash;
    [SerializeField] private List<StashItems> StashItemList;
    private bool isOpened;
    private List<SavedStashItems> savedStashItemList;

    private void Start()
    {
        itemStash = ItemStash.Instance;
        savedStashItemList = new List<SavedStashItems>();
        isOpened = false;
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
}
