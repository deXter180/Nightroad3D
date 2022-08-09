using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private float Xoffset = 0;
    [SerializeField] private float Zoffset = 0;

    private void Start()
    {
        playerController = PlayerController.Instance;
    }

    private void LateUpdate()
    {
        Vector3 pos = playerController.transform.position;
        transform.position = new Vector3(pos.x + Xoffset, transform.position.y, pos.z + Zoffset);
    }

}
