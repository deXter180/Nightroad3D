using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : Singleton<OverlayManager>
{
    #region Variables

    private GradientFog gradientFog;
    private Dictionary<OverlayTypes, Blit> blitRendererFeatures = new Dictionary<OverlayTypes, Blit>();

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();        
    }

    private void OnEnable()
    {
        FPSCamControl.OnFPSCameraEneable += FPSCamControl_OnFPSCameraEneable;
        FPSCamControl.OnFPSCameraDisable += FPSCamControl_OnFPSCameraDisable;
    }

    private void OnDisable()
    {
        FPSCamControl.OnFPSCameraEneable -= FPSCamControl_OnFPSCameraEneable;
        FPSCamControl.OnFPSCameraDisable -= FPSCamControl_OnFPSCameraDisable;
    }

    private void Start()
    {
        Setup();
        void Setup()
        {
            foreach (var feature in AssetLoader.URPRenderData.rendererFeatures)
            {
                switch (feature.name)
                {
                    case "Health":
                        blitRendererFeatures.Add(OverlayTypes.Damage, feature as Blit);
                        break;
                    case "Shield":
                        blitRendererFeatures.Add(OverlayTypes.Shield, feature as Blit);
                        break;
                    case "GradientFog":
                        gradientFog = feature as GradientFog;
                        break;
                }
            }
            ClearAllOverlayEffects();
        }
    }

    public void AddOverlayEffect(OverlayTypes overlayType)
    {
        if (blitRendererFeatures.TryGetValue(overlayType, out Blit blit))
        {
            blit.SetActive(true);
        }         
    }

    public void RemoveOverlayEffect(OverlayTypes overlayType)
    {
        if (blitRendererFeatures.TryGetValue(overlayType, out Blit blit))
        {
            blit.SetActive(false);
        }
    }

    public void ClearAllOverlayEffects()
    {
        foreach(var blit in blitRendererFeatures.Values)
        {
            blit.SetActive(false);            
        }
        DisableFog();
    }

    public Blit GetOverlayFeature(OverlayTypes overlayType)
    {
        if (blitRendererFeatures.TryGetValue(overlayType, out Blit blit))
        {
            return blit;
        }
        return null;
    }

    public void MadeRenderDataChange()
    {
        AssetLoader.URPRenderData.SetDirty();
    }

    public void EnableFog()
    {
        gradientFog.SetActive(true);
    }

    public void DisableFog()
    {
        gradientFog.SetActive(false);
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~

    #region Callbacks

    private void FPSCamControl_OnFPSCameraDisable()
    {
        ClearAllOverlayEffects();
    }

    private void FPSCamControl_OnFPSCameraEneable()
    {

    }

    #endregion
}

