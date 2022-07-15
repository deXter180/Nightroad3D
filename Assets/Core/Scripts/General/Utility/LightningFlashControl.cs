using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningFlashControl : MonoBehaviour
{
    public enum WaveForm { sin, tri, sqr, saw, inv, noise};
    public WaveForm waveform = WaveForm.sin;
    [SerializeField] private float onMin = 10f;
    [SerializeField] private float onMax = 60f;
    [SerializeField] private float offMin = 0.25f;
    [SerializeField] private float offMax = 0.8f;
    [SerializeField] private float soundOffMin = 0.25f;
    [SerializeField] private float soundOffMax = 1.75f;
    [SerializeField] private float baseStart = 0.0f;
    [SerializeField] private float amplitude = 1.0f;
    [SerializeField] private float frequency = 0.5f;
    [SerializeField] private float phase = 0f;
    private bool isLightningActive;
    private float elaplsedTimeOn = 0;
    private float elaplsedTimeOff = 0;
    private Color originalColor;
    private Light lightningLight;

    private void Awake()
    {
        lightningLight = GetComponent<Light>();
        originalColor = lightningLight.color;
        lightningLight.enabled = false;
        isLightningActive = false;
    }

    private void Update()
    {
        PlayLightningFX();
        if (isLightningActive)
        {
            lightningLight.color = originalColor * EvaluateWave();
        }
    }

    private void PlayLightningFX()
    {        
        if (!isLightningActive)
        {
            elaplsedTimeOn += Time.deltaTime;
            if (elaplsedTimeOn >= Random.Range(onMin, onMax))
            {
                lightningLight.enabled = true;
                isLightningActive = true;
                elaplsedTimeOn = 0;
                //StartCoroutine(PlaySoundFX());
            }
        }
        else
        {
            elaplsedTimeOff += Time.deltaTime;
            if (elaplsedTimeOff >= Random.Range(offMin, offMax))
            {
                lightningLight.enabled = false;
                isLightningActive = false;
                elaplsedTimeOff = 0;
            }
        }           
    }

    //private IEnumerator PlaySoundFX()
    //{
    //    yield return Helpers.GetWait(Random.Range(soundOffMin, soundOffMax));

    //}

    private float EvaluateWave()
    {
        float x = (Time.time + phase) * frequency;
        float y;
        x = x - Mathf.Floor(x);
        switch (waveform)
        {
            case WaveForm.sin:
                {
                    y = Mathf.Sin(x * 2 * Mathf.PI);
                    break;
                }
            case WaveForm.tri:
                {
                    if (x < 0.5f)
                        y = 4.0f * x - 1.0f;
                    else
                        y = -4.0f * x + 3.0f;
                    break;
                }
            case WaveForm.sqr:
                {
                    if (x < 0.5f)
                        y = 1.0f;
                    else
                        y = -1.0f;
                    break;
                }
            case WaveForm.saw:
                {
                    y = x;
                    break;
                }
            case WaveForm.inv:
                {
                    y = 1.0f - x;
                    break;
                }
            case WaveForm.noise:
                {
                    y = 1f - (Random.value * 2);
                    break;
                }
            default: y = 1.0f;
                break;           
        }
        return (y * amplitude) + baseStart;
    }
}
