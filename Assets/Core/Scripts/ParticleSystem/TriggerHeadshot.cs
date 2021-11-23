using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TriggerHeadshot : MonoBehaviour
{
    private VisualEffect visualEffect;

    private void Awake()
    {
        visualEffect = GetComponentInChildren<VisualEffect>();
    }

    public void PlayHeadshotVFX() 
    {
        visualEffect.Play();
    }

}
