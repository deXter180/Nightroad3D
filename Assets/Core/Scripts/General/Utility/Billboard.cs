using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private bool IsVisibleToCamera()
    {
        float distanceFromCam = Vector3.Distance(mainCam.transform.position, transform.position);
        if (distanceFromCam <= mainCam.farClipPlane)
        {
            return true;
        }
        else return false;
        //Vector3 visTest = Camera.main.WorldToViewportPoint(transform.position);
        //return (visTest.x >= 0 && visTest.y >= 0) && (visTest.x <= 1 && visTest.y <= 1) && visTest.z >= 0;
    }

    private void LateUpdate()
    {
        if (IsVisibleToCamera())
        {
            transform.rotation = mainCam.transform.rotation;
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }        
        // Alternative method but bends the asset when the player comes too close
        //transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
    }
}
