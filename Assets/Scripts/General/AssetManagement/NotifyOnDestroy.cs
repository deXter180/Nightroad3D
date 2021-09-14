using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

public class NotifyOnDestroy : MonoBehaviour
{
    public AssetReference AssetReference { get; set; }
    public event Action<AssetReference, NotifyOnDestroy> Destroyed;

    private void OnDestroy()
    {
        Destroyed?.Invoke(AssetReference, this);
    }
}
