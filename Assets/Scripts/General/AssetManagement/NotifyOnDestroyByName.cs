using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

public class NotifyOnDestroyByName : MonoBehaviour
{
    public event Action<AssetReference, NotifyOnDestroyByName> Destroyed;

    private void OnDestroy()
    {
        Addressables.ReleaseInstance(this.gameObject);
    }
}
