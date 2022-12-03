using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FPSCamControl : Singleton<FPSCamControl>
{
    #region Variables

    [SerializeField] private UniversalRenderPipelineAsset pilelineAsset;
    private Camera mainCam;
    private Camera UICam;    
    private UniversalAdditionalCameraData cameraData;
    private AudioListener audioListener; 
    public static event Action OnFPSCameraEneable;
    public static event Action OnFPSCameraDisable;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        mainCam = GetComponentInChildren<Camera>();       
        audioListener = GetComponentInChildren<AudioListener>();
    }

    private void OnEnable()
    {
        UpdatePipeline();
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
        transform.parent = null;
    }

    #endregion

    #region Mechanics

    public void EnableFPSCamera()
    {
        gameObject.SetActive(true);
        OnFPSCameraEneable?.Invoke();
    }

    public void DisableFPSCamera()
    {
        OnFPSCameraDisable?.Invoke();
        gameObject.SetActive(false);
    }

    private void UpdatePipeline()
    {
        if (pilelineAsset)
        {
            GraphicsSettings.renderPipelineAsset = pilelineAsset;
        }
    }

    #endregion
}

