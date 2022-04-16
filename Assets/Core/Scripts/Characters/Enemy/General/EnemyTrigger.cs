using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    private SphereCollider sphereCollider;
    public bool IsTargetFleed { get; private set; }
    public bool IsTargetInRange { get; private set; }
    public bool IsSlowed { get; private set; }
    private Enemy enemy;

    public void TriggerSetup(Enemy EM)
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        IsTargetFleed = true;
        IsTargetInRange = false;
        IsSlowed = false;
        enemy = EM;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemy != null)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                IsTargetFleed = false;
                if (Vector3.Distance(transform.position, other.transform.position) <= enemy.ThisEnemySO.AttackRange)
                {
                    IsTargetInRange = true;
                }
                else
                {
                    IsTargetInRange = false;
                }
                if (Vector3.Distance(transform.position, other.transform.position) <= enemy.ThisEnemySO.AttackRange + 40f)
                {
                    IsSlowed = true;
                }
                else
                {
                    IsSlowed = false;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (enemy != null)
        {           
            if (other.gameObject.CompareTag("Player"))
            {
                IsTargetFleed = false;
                if (Vector3.Distance(transform.position, other.transform.position) <= enemy.ThisEnemySO.AttackRange)
                {
                    IsTargetInRange = true;
                }
                else
                {
                    IsTargetInRange = false;
                }
                if (Vector3.Distance(transform.position, other.transform.position) <= enemy.ThisEnemySO.AttackRange + 40f)
                {
                    IsSlowed = true;
                }
                else
                {
                    IsSlowed = false;
                }
            }
        }
    }

    private IEnumerator OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            yield return new WaitForSeconds(0.5f);
            IsTargetInRange = false;
            IsTargetFleed = true;
            IsSlowed = false;
        }
    }
}
