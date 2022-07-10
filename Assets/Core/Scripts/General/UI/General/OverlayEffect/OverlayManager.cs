using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OverlayManager : Singleton<OverlayManager>
{
    [SerializeField] private UniversalRendererData rendererData;
    private Dictionary<OverlayTypes, Blit> blitRendererFeatures = new Dictionary<OverlayTypes, Blit>();

    protected override void Awake()
    {
        base.Awake();
        Setup();

        void Setup()
        {
            foreach(var feature in rendererData.rendererFeatures)
            {
                switch (feature.name)
                {
                    case "Health":
                        blitRendererFeatures.Add(OverlayTypes.Damage, feature as Blit);
                        break;
                    case "Shield":
                        blitRendererFeatures.Add(OverlayTypes.Shield, feature as Blit);
                        break;
                }
                feature.SetActive(false);
            }
        }
    }

    private void Start()
    {
        AddOverlayEffect(OverlayTypes.Damage);
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
            if (blit.isActive)
            {
                blit.SetActive(false);
            }
        }
    }

    public void ClearAllOverlayEffects()
    {
        foreach(var blit in blitRendererFeatures.Values)
        {
            blit.SetActive(false);
        }
    }

    public Blit GetOverlayFeature(OverlayTypes overlayType)
    {
        if (blitRendererFeatures.TryGetValue(overlayType, out Blit blit))
        {
            return blit;
        }
        return null;
    }

    public void MadeChange()
    {
        rendererData.SetDirty();
    }
}
