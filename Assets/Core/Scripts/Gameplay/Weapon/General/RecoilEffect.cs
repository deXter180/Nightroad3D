using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilEffect : Singleton<RecoilEffect>
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private float returnSpeed = 0;
    private float recoilSpeed = 0;

    [System.Serializable]
    public struct RecoilProperty
    {
        public float RecoilX;
        public float RecoilY;
        public float RecoilZ;
        public float RecoilSpeed;
        public float ReturnSpeed;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, recoilSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void ApplyRecoil(RecoilProperty recoilProperty)
    {
        targetRotation += new Vector3(recoilProperty.RecoilX, Random.Range(-recoilProperty.RecoilY, recoilProperty.RecoilY), Random.Range(-recoilProperty.RecoilZ, recoilProperty.RecoilZ));
        returnSpeed = recoilProperty.ReturnSpeed;
        recoilSpeed = recoilProperty.RecoilSpeed;
    }
}
