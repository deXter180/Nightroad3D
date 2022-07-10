using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    private SphereCollider sphereCollider;
    public bool IsTargetFleed { get; private set; }
    private bool isMarked;

    public void TriggerSetup()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        IsTargetFleed = true;
        isMarked = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMarked)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                IsTargetFleed = false;
                isMarked = true;
            }
        }        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isMarked)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                IsTargetFleed = false;
                isMarked = true;
            }
        }
    }

    private IEnumerator OnTriggerExit(Collider other)
    {
        if (isMarked)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                yield return new WaitForSeconds(0.5f);
                IsTargetFleed = true;
                isMarked = false;
            }
        }       
    }
}
