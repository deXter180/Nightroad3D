using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffectOnDeath : MonoBehaviour
{
    private ParticleSystem PS;

    private void OnEnable()
    {
        PS = GetComponent<ParticleSystem>();
    }

    public void PlayBloodOnDeath()
    {
        PS.Play();
    }
}
