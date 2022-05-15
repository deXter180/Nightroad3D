using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FPSCamControl : MonoBehaviour
{
    private Camera mainCam;
    private Camera UICam;
    private UniversalAdditionalCameraData cameraData;

    private void Awake()
    {
        mainCam = GetComponent<Camera>();
    }

    void Start()
    {
        transform.parent = null;               
    }

    private void OnEnable()
    {
        cameraData = mainCam.GetUniversalAdditionalCameraData();
        if (UICam == null)
        {
            UICam = GameObject.FindGameObjectWithTag("UICam").GetComponent<Camera>();
        }
        if (UICam != null && cameraData != null)
        {
            if (!cameraData.cameraStack.Contains(UICam))
            {
                cameraData.cameraStack.Add(UICam);
            }           
        }
    }
}
