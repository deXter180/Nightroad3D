using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource weaponAudioSource;
    [SerializeField] private AudioSource environmentAudioSource;
    [SerializeField] private AudioSource rainAudioSource;
    [SerializeField] private AudioSource windAudioSource;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    private const string MIXER_MASTER = "MasterVolume";
    private const string MIXER_MUSIC = "MusicVolume";
    private const string MIXER_SFX = "SFXVolume";
    private bool isLoaded;
    private static AudioSource MusicAudioSource { get; set; }
    private static AudioSource WeaponAudioSource { get; set; }
    private static AudioSource EnvironmentAudioSource { get; set; }

    protected override void Awake()
    {
        base.Awake();
        isLoaded = false;        
    }

    private void OnEnable()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        isLoaded = true;
        LoadVolume();
    }

    private void OnDisable()
    {
        if (isLoaded)
        {
            PlayerPrefs.SetFloat(MIXER_MASTER, masterVolumeSlider.value);
            PlayerPrefs.SetFloat(MIXER_MUSIC, musicVolumeSlider.value);
            PlayerPrefs.SetFloat(MIXER_SFX, sfxVolumeSlider.value);
        }        
    }

    private void Start()
    {
        MusicAudioSource = musicAudioSource;
        WeaponAudioSource = weaponAudioSource;
        EnvironmentAudioSource = environmentAudioSource;
        StartCoroutine(DelayPlayAudio());
        IEnumerator DelayPlayAudio()
        {
            yield return Helpers.GetWait(1f);
            PlayMusicSound(MusicTypes.Normal, 0, false, true);            
        }
    }

    public void PlayRainAudio()
    {
        if (!rainAudioSource.isPlaying)
        {
            AudioClip clip = GameController.GetAudioClip(AudioTypes.Environment, EnvironmentTypes.Rain, 0, true);
            if (clip != null)
            {
                rainAudioSource.clip = clip;
                rainAudioSource.pitch = UnityEngine.Random.Range(0.5f, 2);
                rainAudioSource.Play();
            }           
        }
    }

    public void PlayWindAudio()
    {
        if (!windAudioSource.isPlaying)
        {
            AudioClip clip = GameController.GetAudioClip(AudioTypes.Environment, EnvironmentTypes.Wind, 0, true);
            if (clip != null)
            {
                windAudioSource.clip = clip;
                windAudioSource.pitch = UnityEngine.Random.Range(0.5f, 2);
                windAudioSource.Play();
            }
        }
    }

    public static void PlayMusicSoundOnce(MusicTypes musicType, int index = 0, bool randomize = false)
    {
        AudioClip clip = GameController.GetAudioClip(AudioTypes.Music, musicType, index, randomize);
        if (clip != null)
        {
            MusicAudioSource.PlayOneShot(clip); 
        }            
    }

    public static void PlayMusicSound(MusicTypes musicType, int index = 0, bool randomize = false, bool loop = false)
    {
        if (!MusicAudioSource.isPlaying)
        {
            AudioClip clip = GameController.GetAudioClip(AudioTypes.Music, musicType, index, randomize);
            if (clip != null)
            {
                MusicAudioSource.clip = clip;
                MusicAudioSource.loop = loop;
                MusicAudioSource.Play();
            }
        }       
    }

    public static void PlayWeaponSoundOnce(WeaponTypes weaponType, int index = 0, bool randomize = false)
    {        
        AudioClip clip = GameController.GetAudioClip(AudioTypes.Weapon, weaponType, index, randomize);
        if (clip != null)
        {
            WeaponAudioSource.pitch = UnityEngine.Random.Range(0.5f, 2);
            WeaponAudioSource.PlayOneShot(clip);            
        }
    }

    public static void PlayWeaponSound(WeaponTypes weaponType, int index = 0, bool randomize = false)
    {
        if (MusicAudioSource.isPlaying)
        {
            MusicAudioSource.Stop();
        }
        AudioClip clip = GameController.GetAudioClip(AudioTypes.Weapon, weaponType, index, randomize);
        if (clip != null)
        {
            WeaponAudioSource.pitch = UnityEngine.Random.Range(0.5f, 2);
            WeaponAudioSource.clip = clip;
            WeaponAudioSource.Play();
        }
    }

    public static void PlayEnvironmentSoundOnce(EnvironmentTypes environmentType, int index = 0, bool randomize = false)
    {
        AudioClip clip = GameController.GetAudioClip(AudioTypes.Environment, environmentType, index, randomize);
        if (clip != null)
        {
            EnvironmentAudioSource.pitch = UnityEngine.Random.Range(0.5f, 2);
            EnvironmentAudioSource.PlayOneShot(clip);
        }            
    }

    public static void PlayEnvironmentSound(EnvironmentTypes environmentType, int index = 0, bool randomize = false, bool loop = false)
    {
        if (MusicAudioSource.isPlaying)
        {
            MusicAudioSource.Stop();
        }
        AudioClip clip = GameController.GetAudioClip(AudioTypes.Environment, environmentType, index, randomize);
        if (clip != null)
        {
            EnvironmentAudioSource.clip = clip;
            EnvironmentAudioSource.loop = loop;
            EnvironmentAudioSource.pitch = UnityEngine.Random.Range(0.5f, 2);
            EnvironmentAudioSource.Play();
        }
    }

    public void StopRainAudio()
    {
        rainAudioSource.Stop();
    }

    public void StopWindAudio()
    {
        windAudioSource.Stop();
    }

    public static void StopMusicSound()
    {
        MusicAudioSource.Stop();
    }

    public static void StopWeaponSound()
    {
        WeaponAudioSource.Stop();
    }

    public static void StopEnvironmentSound()
    {
        EnvironmentAudioSource.Stop();
    }

    private void SetMasterVolume(float value)
    {
        audioMixer.SetFloat(MIXER_MASTER, Mathf.Log10(value) * 20);
    }

    private void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
    }

    private void SetSFXVolume(float value)
    {
        audioMixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
    }

    private void LoadVolume()
    {
        if (isLoaded)
        {
            float masterVol = PlayerPrefs.GetFloat(MIXER_MASTER, 1f);
            float musicVol = PlayerPrefs.GetFloat(MIXER_MUSIC, 1f);
            float sfxVol = PlayerPrefs.GetFloat(MIXER_SFX, 1f);
            masterVolumeSlider.value = masterVol;
            musicVolumeSlider.value = musicVol;
            sfxVolumeSlider.value = sfxVol;
            SetMasterVolume(masterVol);
            SetMusicVolume(musicVol);
            SetSFXVolume(sfxVol);
        }        
    }
}
