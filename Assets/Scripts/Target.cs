using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Target : MonoBehaviour
{

    public int MaxHP;
    public Guid TargetId;
    
    //~~~~~~~~~~~~~~Initialization~~~~~~~~~~~~~

    private void OnEnable()
    {
        TargetId = new Guid();
        Debug.Log($"onEnabled :- {TargetId}, {this.gameObject.name}");

        HealthSystem.Instance.SetHealthSystem(TargetId,MaxHP, this.gameObject.name); // Setting Max Health by calling method from HealthSystem   

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

    public void DoDamage(int dmg,string colEntity)
    {
        HealthSystem.Instance.Damage(dmg, colEntity);
    }
 
    //~~~~~~~~~~~~~~~~~~~~~~~~~~Incoming Health System Callback~~~~~~~~~~~~~~~~~~~~~~~~

    private void OnDamage(object sender, HealthSystem.DamagedEventArgs e)
    {
            if (e != null)
            {
               // Debug.Log(e.Damage);
            }
            //Debug.Log(HealthSystem.Instance.CurrentHealth);
    }

    private void OnKilled(object sender, EventArgs e)
    {
        if (e != null)
        {
            // if (HealthSystem.Instance.IsDead() == true)
            // {
            //     Debug.Log("Dead !!");
               
            // }
        }
    }
}

