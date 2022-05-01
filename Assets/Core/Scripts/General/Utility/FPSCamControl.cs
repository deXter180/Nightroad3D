using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FPSCamControl : MonoBehaviour
{
    private Vector3 ConstantDistFromPlayer = new Vector3();
    private PlayerController player;
    private Camera mainCam;
    private Camera UICam;
    private UniversalAdditionalCameraData cameraData;

    private void Awake()
    {
        mainCam = GetComponent<Camera>();
    }

    void Start()
    {
        player = PlayerController.Instance;
        transform.parent = null;
        ConstantDistFromPlayer = transform.position - player.transform.position;        
    }

    private void OnEnable()
    {
        cameraData = mainCam.GetUniversalAdditionalCameraData();
        if (UICam == null)
        {
            UICam = GameObject.FindGameObjectWithTag("UICam").GetComponent<Camera>();
        }
        if (UICam != null && cameraData != null)
        {
            if (!cameraData.cameraStack.Contains(UICam))
            {
                cameraData.cameraStack.Add(UICam);
            }           
        }
    }

    void Update()
    {
        transform.position = player.transform.position + ConstantDistFromPlayer;
    }
}
