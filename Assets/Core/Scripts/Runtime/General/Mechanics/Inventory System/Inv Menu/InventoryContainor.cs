using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryContainor : MonoBehaviour
{
    private RectTransform rectTransform;
    public RectTransform ContainorRect => rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

}
