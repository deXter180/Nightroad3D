using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomShaderEffect : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material material;
    private Camera FPSCam;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        FPSCam = Camera.main;
    }

    private void Update()
    {
        ApplyScreenPos();
    }

    private void ApplyScreenPos()
    {
        Vector2 screenPixels = FPSCam.WorldToScreenPoint(transform.position);
        screenPixels = new Vector2(screenPixels.x / Screen.width, screenPixels.y / Screen.height);
        material.SetVector("ObjectScreenPosition", screenPixels);
    }
}
