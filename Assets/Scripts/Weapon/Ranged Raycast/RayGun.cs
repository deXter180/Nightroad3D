using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class RayGun : MonoBehaviour
{
    private float LastTimeShot = 0;
    private Camera cam;
    private Animator anim;
    private Input input;
    private WeaponBrain weaponBrain;
    //private int ShootHash = Animator.StringToHash("Shoot");

    private void Awake()
    {
        input = GetComponentInParent<InputControl>();
        weaponBrain = GetComponent<WeaponBrain>();
        anim = GetComponent<Animator>();
        cam = GetComponentInParent<Camera>();
    }

    private void FixedUpdate()
    {
        if (LastTimeShot + weaponBrain.GetThisWeapon().AttackSpeed <= Time.fixedDeltaTime)
        {
            //anim.SetTrigger(ShootHash);
            LastTimeShot = Time.fixedDeltaTime;

            Shoot();
        }
    }
    public void Shoot()
    {
        if (input.GetAttack() == 1 && gameObject.activeInHierarchy)
        {
            int PlayerLayer = 8;
            int EnemyLayer = 9;
            int bitmask = ~(1 << PlayerLayer) & (1 << EnemyLayer) ;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, weaponBrain.GetThisWeapon().AttackRange, bitmask))
            {
                Debug.Log(hit.transform.name);
                if (hit.collider.GetComponent<Target>() != null)
                {
                    Target target = hit.collider.GetComponent<Target>();
                    weaponBrain.GetThisWeapon().DoAttack(target, target.GetEBFromTarget().GetThisEnemy().DodgeChance);
                }
            }
        }
    }



}
