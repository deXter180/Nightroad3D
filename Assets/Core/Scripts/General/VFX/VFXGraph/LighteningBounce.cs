using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LighteningBounce : MonoBehaviour
{
    [SerializeField] private float delayTime = 1;
    private Vector3 midPos1;
    private Vector3 midPos2;
    private string startPosName = "StartPos";
    private string midPos1Name = "MidPos1";
    private string midPos2Name = "MidPos2";
    private string endPosName = "EndPos";
    private VisualEffect visualEffect;

    private void Awake()
    {
        visualEffect = GetComponent<VisualEffect>();
    }

    public void BounceToTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        float distance = Vector3.Distance(targetPos, transform.position);
        Ray ray = new Ray(transform.position, direction);
        midPos1 = ray.GetPoint(distance / 3);
        midPos2 = ray.GetPoint(distance * 2 / 3);
        visualEffect.SetVector3(startPosName, transform.position);
        visualEffect.SetVector3(midPos1Name, midPos1);
        visualEffect.SetVector3(midPos2Name, midPos2);
        visualEffect.SetVector3(endPosName, targetPos);
        visualEffect.Play();
        StartCoroutine(End());
        IEnumerator End()
        {
            yield return Helpers.GetWait(delayTime);
            AssetLoader.ReleaseAssetInstance(this.gameObject);
        }
    }
}
