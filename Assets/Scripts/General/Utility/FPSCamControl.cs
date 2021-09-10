using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamControl : MonoBehaviour
{
    private Vector3 ConstantDistFromPlayer = new Vector3();

    void Start()
    {
        ConstantDistFromPlayer = transform.position - PlayerController.Instance.transform.position;
    }

    void Update()
    {
        transform.position = PlayerController.Instance.transform.position + ConstantDistFromPlayer;
    }
}
