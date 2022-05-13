using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = PlayerController.Instance;
    }

    private void LateUpdate()
    {
        Vector3 pos = playerController.transform.position;
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
    }

}
