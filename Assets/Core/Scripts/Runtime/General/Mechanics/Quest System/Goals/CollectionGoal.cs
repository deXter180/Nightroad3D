using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionGoal : Goal
{
    #region General

    public ItemTypes ItemType { get; private set; }

    public CollectionGoal (ItemTypes itemType, int requiredAmount)
    {
        ItemType = itemType;
        RequiredAmount = requiredAmount;
        CurrentAmount = 0;
        IsCompleted = false;

    }

    public override void Initialize()
    {
        base.Initialize();
        InventorySystem.OnAddingToInventory += InventorySystem_OnAddingInInventory;
        InventorySystem.OnRemovingFromInventory += InventorySystem_OnRemovingFromInventory;
    }

    public override void Closer()
    {
        base.Closer();
        InventorySystem.OnAddingToInventory -= InventorySystem_OnAddingInInventory;
        InventorySystem.OnRemovingFromInventory -= InventorySystem_OnRemovingFromInventory;
    }

    #endregion

    #region Callbacks

    private void InventorySystem_OnRemovingFromInventory(ItemTypes obj, PlacedObject PO)
    {
        if (ItemType == obj)
        {
            CurrentAmount--;          
            Evaluate();
        }
    }

    private void InventorySystem_OnAddingInInventory(ItemTypes obj, PlacedObject PO)
    {
        if (ItemType == obj)
        {
            CurrentAmount++;
            Evaluate();
        }
    }

    #endregion
}
