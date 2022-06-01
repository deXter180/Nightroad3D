using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffectOnHit : MonoBehaviour
{
    private ParticleSystem PS;

    private void OnEnable()
    {
        PS = GetComponent<ParticleSystem>();
    }

    public void PlayBloodOnHit()
    {
        PS.Play();
    }
}
