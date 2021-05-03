using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollision : MonoBehaviour
{
    private bool IsAtttacking;
    private Collider[] colliders;
    private Vector3[] ColRange;
    private Weapons meleeWeapn;
    private MeleeAnimator com;
    private IGetWeapon getMelee;
    private Input input;
    

    private void Awake()
    {
        com = FindObjectOfType<MeleeAnimator>();
        colliders = GetComponentsInChildren<Collider>();
        getMelee = GetComponentInParent<WeaponBrain>();
        input = GetComponentInParent<InputControl>();
        GetRange();
        SetStats(getMelee);
        SetRange();
    }

    private void SetStats(IGetWeapon getM)
    {
        if (getM != null)
        {
            meleeWeapn = getM.GetThisWeapon();
        }
    }
    private void GetRange()
    {
        for (int i = 0; i == colliders.Length; i++)
        {
            ColRange[i] = colliders[i].transform.position;
        }
    }

    private void SetRange()
    {
        for (int i = 0; i == colliders.Length; i++)
        {
            colliders[i].transform.position = new Vector3(ColRange[i].x, ColRange[i].y, meleeWeapn.AttackRange);
        }
    }
    
    private void OnCollisionStay(Collision collision) => TakeDamage(collision);

    private void TakeDamage(Collision col)
    {
        if (input.GetAttack() == 1 && gameObject.activeInHierarchy)
        {
            if (gameObject.CompareTag("WeaponCol"))
            {
                if (col.gameObject.CompareTag("Enemy") && col.gameObject.activeInHierarchy && IsAtttacking == false)
                {
                    col.gameObject.TryGetComponent<Target>(out Target target);
                    IsAtttacking = true;
                    meleeWeapn.DoAttack(target, target.GetEBFromTarget().GetThisEnemy().DodgeChance);
                    StartCoroutine(Attacking());
                }
            }
        }
    }

    IEnumerator Attacking()
    {
        if (IsAtttacking == true)
        {
            yield return new WaitForSeconds(meleeWeapn.AttackSpeed);
        }
        IsAtttacking = false;
    }

}




