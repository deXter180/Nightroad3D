using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollision : MonoBehaviour
{
    private int AppliedDmg = 100;
    private float AttackSpeed = 2f;
    private bool IsAtttacking;
    CombatController com;

    private void Awake()
    {
        com = FindObjectOfType<CombatController>();
    }


    private void OnCollisionStay(Collision collision) => TakeDamage(collision);

    private void TakeDamage(Collision col)
    {

        if ((com.isRSPressed || com.isLSPressed || com.isBAPressed) == true)
        {
            if (gameObject.CompareTag("WeaponCol"))
            {
                if (col.gameObject.tag == "Enemy" && col.gameObject.activeInHierarchy && IsAtttacking == false)
                {
                    IsAtttacking = true;
                    col.gameObject.GetComponent<Target>().DoDamage(AppliedDmg); 
                    StartCoroutine(Attacking());
                }
            }
        }
    }

    IEnumerator Attacking()
    {
        if (IsAtttacking == true)
        {
            yield return new WaitForSeconds(AttackSpeed);
        }
        IsAtttacking = false; 
    }
}
