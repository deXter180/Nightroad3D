using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCam;
    private Transform camTransform;

    private void Start()
    {
        mainCam = Helpers.MainCam;
        camTransform = mainCam.transform;
    }

    private bool IsVisibleToCamera()
    {
        if (camTransform == null)
        {
            mainCam = Helpers.MainCam;
            camTransform = mainCam.transform;
        }
        float distanceFromCam = Vector3.Distance(camTransform.position, transform.position);
        if (distanceFromCam <= mainCam.farClipPlane)
        {
            return true;
        }
        else return false;
    }

    private void LateUpdate()
    {
        if (IsVisibleToCamera())
        {
            transform.rotation = camTransform.rotation;
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }        
    }
}
