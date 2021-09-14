using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainParticleSystem : MonoBehaviour
{
    [SerializeField] private float OwnYPosition;
    [SerializeField] private float RainYLocalPos;
    [SerializeField] private float CloudYLocalPos;

    private void Awake()
    {
        transform.position = new Vector3(transform.position.x, OwnYPosition, transform.position.z);
    }

    public IEnumerator EnableRain()
    {
        AssetCollections.InstantiateAssetsByName("Rain_VFX", this.transform);
        List<GameObject> VfxList = new List<GameObject>();
        VfxList = AssetCollections.GetGOListFromDictByName("Rain_VFX");
        yield return new WaitForSeconds(1f);
        if (VfxList != null && VfxList.Count > 0)
        {
            foreach (var temp in VfxList)
            {
                temp.transform.rotation = Quaternion.identity;
                if (temp.name == "Rain_vfx")
                {
                    temp.transform.localPosition = new Vector3(0, RainYLocalPos, 0);
                }
                else if (temp.name == "Clouds_vfx")
                {
                    temp.transform.localPosition = new Vector3(0, CloudYLocalPos, 0);
                }
            }
        }
    }

    public void DisableRain()
    {
        List<GameObject> VfxList = new List<GameObject>();
        VfxList = AssetCollections.GetGOListFromDictByName("Rain_VFX");
        if (VfxList != null && VfxList.Count > 0)
        {
            foreach (var temp in VfxList)
            {
                AssetCollections.ReleaseAssetInstance(temp, "Rain_VFX", true);
            }
        }       
    }


}
