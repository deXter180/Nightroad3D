using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Target : MonoBehaviour
{

    public int MaxHP;
    
    //~~~~~~~~~~~~~~Initialization~~~~~~~~~~~~~

    private void OnEnable()
    {
        HealthSystem.Instance.SetHealth(MaxHP);                     // Setting Max Health by calling method from HealthSystem
        if (HealthSystem.Instance != null)                          // HealthSystem Event subscription
        {
            HealthSystem.Instance.RegisterDamageEvent(OnDamage);
            HealthSystem.Instance.RegisterKillEvent(OnKilled);
        }

    }

    private void OnDisable()
    {
        if (HealthSystem.Instance != null)                          // HealthSystem Event desubscription
        {
            HealthSystem.Instance.UnRegisterDamageEvent(OnDamage);
            HealthSystem.Instance.UnRegisterKillEvent(OnKilled);
        }
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~Damage function~~~~~~~~~~~~~~~~~~~~~~~~~~~~ 

    public void DoDamage(int dmg)
    {
        HealthSystem.Instance.Damage(dmg);
    }
 
    //~~~~~~~~~~~~~~~~~~~~~~~~~~Incoming Health System Callback~~~~~~~~~~~~~~~~~~~~~~~~

    private void OnDamage(object sender, HealthSystem.DamagedEventArgs e)
    {
            if (e != null)
            {
                Debug.Log(e.Damage);
            }
            Debug.Log(HealthSystem.Instance.CurrentHealth);
    }

    private void OnKilled(object sender, EventArgs e)
    {
        if (e != null)
        {
            if (HealthSystem.Instance.IsDead == true)
            {
                Debug.Log("Dead !!");
               
            }
        }
    }
}

