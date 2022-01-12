using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamControl : MonoBehaviour
{
    private Vector3 ConstantDistFromPlayer = new Vector3();
    private PlayerController player;


    void Start()
    {
        player = PlayerController.Instance;
        transform.parent = null;
        ConstantDistFromPlayer = transform.position - player.transform.position;       
    }

    void Update()
    {
        transform.position = player.transform.position + ConstantDistFromPlayer;
    }
}
