using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollision : MonoBehaviour
{
    private int AppliedDmg = 100;
    private float AttackSpeed = 2f;
    private bool iSAtttacking;
    CombatController com;

    private void Awake()
    {
        com = FindObjectOfType<CombatController>();
    }

    public class DamageEventArgs : EventArgs
    {
        public int DoDamage;
        public bool IsAttacking;

        public DamageEventArgs(int _doDamage, bool _isAttacking)
        {
            DoDamage = _doDamage;
            IsAttacking = _isAttacking;
        }
    }

    public event EventHandler<DamageEventArgs> OnInflictingDamage;

    private void OnCollisionStay(Collision collision) => TakeDamage(collision);

    private void TakeDamage(Collision col)
    {

        if ((com.isRSPressed || com.isLSPressed || com.isBAPressed) == true)
        {
            if (gameObject.CompareTag("WeaponCol"))
            {
                if (col.gameObject.tag == "Enemy" && col.gameObject.activeInHierarchy && iSAtttacking == false)
                {
                    iSAtttacking = true;
                    OnInflictingDamage?.Invoke(this, new DamageEventArgs(AppliedDmg, iSAtttacking));
                    StartCoroutine(Attacking());
                }
            }
        }
    }

    IEnumerator Attacking()
    {
        if (iSAtttacking == true)
        {
            yield return new WaitForSeconds(AttackSpeed);
        }
        iSAtttacking = false; 
    }
}
