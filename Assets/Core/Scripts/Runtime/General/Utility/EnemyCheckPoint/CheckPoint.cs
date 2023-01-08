using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    public bool IsAvailable { get; private set; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        IsAvailable = false;
        SetMatColor(IsAvailable);
    }

    public void SetAvailability(bool isTrue)
    {
        IsAvailable = isTrue;
        SetMatColor(IsAvailable);
    }

    private void SetMatColor(bool isTrue)
    {
        if (isTrue)
        {
            meshRenderer.material.color = Color.green;
        }
        else
        {
            meshRenderer.material.color = Color.red;
        }
    }
}
