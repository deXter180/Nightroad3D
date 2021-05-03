using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Camera cam;
    private Vector3 camDistance = new Vector3(0, 144f, 0f);
    [SerializeField] private Transform playerPos;
    public static CameraControl Instance { get; private set; }

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    void Update()
    {
        FollowChar();
    }
    private void FollowChar()
    {
        transform.position = (playerPos.position + camDistance);
    }
}
