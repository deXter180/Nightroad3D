using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioSO", menuName = "Audios/New")]
public class AudioSO : ScriptableObject
{
    #region Variables

    public AudioTypes AudioType;
    [SerializeField] private List<MusicAudio> musicAudioList;
    [SerializeField] private List<EnvironmentAudio> environmentAudioList;
    [SerializeField] private List<WeaponAudio> weaponAudioList;
    [SerializeField] private List<SpellAudio> spellAudioList;
    [SerializeField] private List<EnemyAudio> enemyAudioList;

    #endregion

    #region General

    public List<AudioClip> GetMusicAudio(MusicTypes musicType)
    {
        foreach(var audio in musicAudioList)
        {
            if (audio.MusicType == musicType)
            {
                return audio.ClipList;
            }
        }
        return null;
    }

    public List<AudioClip> GetEnvironmentAudio(EnvironmentTypes environmentType)
    {
        foreach (var audio in environmentAudioList)
        {
            if (audio.EnvironmentType == environmentType)
            {
                return audio.ClipList;
            }
        }
        return null;
    }

    public List<AudioClip> GetWeaponAudio(WeaponTypes weaponType)
    {
        foreach (var audio in weaponAudioList)
        {
            if (audio.WeaponType == weaponType)
            {
                return audio.ClipList;
            }
        }
        return null;
    }

    public List<AudioClip> GetSpellAudio(SpellTypes spellType)
    {
        foreach (var audio in spellAudioList)
        {
            if (audio.SpellType == spellType)
            {
                return audio.ClipList;
            }
        }
        return null;
    }

    public List<AudioClip> GetEnemyAudio(EnemyTypes enemyType)
    {
        foreach (var audio in enemyAudioList)
        {
            if (audio.EnemyType == enemyType)
            {
                return audio.ClipList;
            }
        }
        return null;
    }

    #endregion
}

#region AudioContainers

[System.Serializable]
public class MusicAudio
{
    public MusicTypes MusicType;
    public List<AudioClip> ClipList;
}

[System.Serializable]
public class EnvironmentAudio
{
    public EnvironmentTypes EnvironmentType;
    public List <AudioClip> ClipList;
}

[System.Serializable]
public class WeaponAudio
{
    public WeaponTypes WeaponType;
    public List<AudioClip> ClipList;
}

[System.Serializable]
public class SpellAudio
{
    public SpellTypes SpellType;
    public List<AudioClip> ClipList;
}

[System.Serializable]
public class EnemyAudio
{
    public EnemyTypes EnemyType;
    public List<AudioClip> ClipList;
}

#endregion