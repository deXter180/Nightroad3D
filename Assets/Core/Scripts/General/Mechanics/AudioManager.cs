using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();         
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayWeaponSound(bool controlSound, WeaponTypes weaponType)
    {
        if (GameController.GetAudioClipByWeaponType(weaponType) != null)
        {
            if (weaponType == WeaponTypes.Rifle)
            {
                if (controlSound)
                {
                    if (!audioSource.isPlaying)
                        audioSource.PlayOneShot(GameController.GetAudioClipByWeaponType(weaponType), 0.5f);
                }
                else
                {
                    audioSource.Stop();
                }
            }
        }       
    }
}
