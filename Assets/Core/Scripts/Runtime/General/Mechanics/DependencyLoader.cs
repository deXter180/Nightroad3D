using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependencyLoader : Singleton<DependencyLoader>
{
    private bool isInitialized;

    protected override void Awake()
    {
        base.Awake();

    }
}
