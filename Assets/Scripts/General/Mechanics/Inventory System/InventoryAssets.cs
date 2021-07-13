using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAssets : MonoBehaviour
{
    public static InventoryAssets Instance { get; private set; }
    public Transform gridVisual;

    private void Awake()
    {
        Instance = this;    
    }

    public List<InventoryItemSO> ItemSOList;
    
    public InventoryItemSO GetInventoryItemSO(ItemTypes itemType)
    {
        foreach (InventoryItemSO itemSO in ItemSOList)
        {
            if (itemSO.itemType == itemType)
            {
                return itemSO;
            }
        }
        return null;
    }

}
