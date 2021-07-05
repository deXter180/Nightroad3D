using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    public static bool IsPlaySound;


    public static void PlaySound()
    {
        if (IsPlaySound)
        {
            GameObject soundObject = new GameObject("Sound");
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(GameController.Instance.LaserGunShoot, 0.2f);
        }
        else
        {
            //audioSource.Stop();
        }
        
    }
}
