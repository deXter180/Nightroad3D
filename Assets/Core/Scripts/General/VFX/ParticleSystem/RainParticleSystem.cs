using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainParticleSystem : MonoBehaviour
{
    [SerializeField] private ParticleSystem rainPS;
    [SerializeField] private ParticleSystem cloudPS;
    private bool IsRainActive; 

    private void Start()
    {
        IsRainActive = false;
    }

    public void ControlRain()
    {
        if (!IsRainActive)
        {
            cloudPS.Play();
            rainPS.Play();
            IsRainActive = true;
        }
        else
        {
            cloudPS.Stop();
            rainPS.Stop();
            IsRainActive = false;
        }       
    } 
   
}
