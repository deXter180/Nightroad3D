using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

public class NotifyOnDestroyByAssetRef : MonoBehaviour
{
    public AssetReference AssetReference { get; set; }
    public event Action<AssetReference, NotifyOnDestroyByAssetRef> Destroyed;

    private void OnDestroy()
    {
        if (AssetReference != null)
        {
            Destroyed?.Invoke(AssetReference, this);
        }           
    }
}
