using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempShieldTrigger : MonoBehaviour
{
    private ParticleSystem PS;

    private void Awake()
    {
        PS = GetComponent<ParticleSystem>();
    }

    public void SetShield(float size, float duration)
    {
        var main = PS.main;
        main.startSize = size;
        main.startLifetime = duration;
    }

    public IEnumerator TriggerShield(float duration)
    {
        PS.Play();
        yield return new WaitForSeconds(duration);
        if (!PS.isStopped)
        PS.Stop();
    }
}
