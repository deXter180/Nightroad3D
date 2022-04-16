using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeVisual : MonoBehaviour
{
    private ParticleSystem PS;

    private void Awake()
    {
        PS = GetComponentInChildren<ParticleSystem>();
    }

    public void TriggerEvent()
    {
        if (!PS.isPlaying)
        {
            PS.Play();
        }
    }
}
