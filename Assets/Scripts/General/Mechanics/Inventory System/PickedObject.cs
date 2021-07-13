using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedObject : MonoBehaviour
{
    private ItemTypes item;
    public ItemTypes itemType { get => item; }

    public void SetupInGameWorld(ItemTypes item)
    {
        this.item = item;
    }
}
