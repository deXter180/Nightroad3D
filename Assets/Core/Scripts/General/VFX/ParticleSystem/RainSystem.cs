using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSystem : PersistentSingleton<RainSystem>
{
    [SerializeField] private ParticleSystem rainPS;
    [SerializeField] private GameObject lightningFlash;
    private bool IsRainActive;
    private AudioManager audioManager;

    protected override void Awake()
    {
        base.Awake();
        IsRainActive = false;
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;       
    }

    private void Update()
    {
        if (IsRainActive)
        {
            audioManager.PlayRainAudio();
            audioManager.PlayWindAudio();
        }
        else
        {
            audioManager.StopRainAudio();
            audioManager.StopWindAudio();
        }
    }

    public void ControlRain()
    {
        if (!IsRainActive)
        {            
            rainPS.Play();
            lightningFlash.SetActive(true);            
            IsRainActive = true;
            
        }
        else
        {            
            rainPS.Stop();
            lightningFlash.SetActive(false);           
            IsRainActive = false;
        }       
    } 
   
}
