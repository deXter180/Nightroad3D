using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainExitTrigger : MonoBehaviour
{
    private RainParticleSystem rainParticleSystem;

    private void Awake()
    {
        rainParticleSystem = GetComponentInParent<RainParticleSystem>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            rainParticleSystem.DisableRain();
        }
    }
}