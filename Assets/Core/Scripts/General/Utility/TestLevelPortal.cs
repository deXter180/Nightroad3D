using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelPortal : MonoBehaviour
{
    private Collider thisCol;

    private void Awake()
    {
        thisCol = GetComponent<Collider>();
        thisCol.isTrigger = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            SceneLoader.LoadNewSingleScene("ProtoLevel");
        }       
    }
}
