using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainParticleSystem : MonoBehaviour
{
    [SerializeField] private float OwnYPosition;
    [SerializeField] private float RainYLocalPos;
    [SerializeField] private float CloudYLocalPos;
    private string rainVfxName = "Rain_pfx";
    private string cloudVfxName = "Clouds_pfx";
    private GameObject rainGO;
    private GameObject cloudGO;

    private void Awake()
    {
        transform.position = new Vector3(transform.position.x, OwnYPosition, transform.position.z);
    }

    private void OnEnable()
    {
        AssetLoader.OnGOCreated += AssetRefLoader_OnGOCreated;
    }

    private void OnDisable()
    {
        AssetLoader.OnGOCreated -= AssetRefLoader_OnGOCreated;
    }

    public void EnableRain()
    {
        AssetLoader.CreateGOAsset(rainVfxName, this.transform);
        AssetLoader.CreateGOAsset(cloudVfxName, this.transform);
    }

    public void DisableRain()
    {
        AssetLoader.ReleaseAssetInstance(rainGO);
        AssetLoader.ReleaseAssetInstance(cloudGO);
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~ Callback ~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void AssetRefLoader_OnGOCreated(GameObject obj)
    {
        obj.transform.rotation = Quaternion.identity;
        if (obj.name == rainVfxName)
        {
            rainGO = obj;
            obj.transform.localPosition = new Vector3(0, RainYLocalPos, 0);
        }
        else if (obj.name == cloudVfxName)
        {
            cloudGO = obj;
            obj.transform.localPosition = new Vector3(0, CloudYLocalPos, 0);
        }
    }
}
