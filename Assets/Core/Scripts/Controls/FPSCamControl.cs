using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FPSCamControl : Singleton<FPSCamControl>
{
    private Camera mainCam;
    private Camera UICam;
    private UniversalAdditionalCameraData cameraData;
    private AudioListener audioListener; 

    protected override void Awake()
    {
        base.Awake();
        mainCam = GetComponent<Camera>();
        transform.parent = null;
        audioListener = GetComponent<AudioListener>();
    }

    public void EnableAudioListener()
    {
        audioListener.enabled = true;
    }

    public void DisableAudioListener()
    {
        audioListener.enabled = false;
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

