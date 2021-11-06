using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else 
        {
            Destroy(Instance);
        }           
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayWeaponSound(bool controlSound, WeaponTypes weaponType)
    {
        if (AssetCollections.GetAudioClipByWeaponType(weaponType) != null)
        {
            if (weaponType == WeaponTypes.Rifle)
            {
                if (controlSound)
                {
                    if (!audioSource.isPlaying)
                        audioSource.PlayOneShot(AssetCollections.GetAudioClipByWeaponType(weaponType), 0.5f);
                }
                else
                {
                    audioSource.Stop();
                }
            }
        }       
    }
}
