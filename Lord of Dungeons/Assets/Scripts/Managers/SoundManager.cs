using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource seAudioSource;

    [SerializeField] List<BGMSoundData> bgmSoundDatas;
    [SerializeField] List<SESoundData> seSoundDatas;

    public float masterVolume = 1;
    public float bgmMasterVolume = 1;
    public float seMasterVolume = 1;
    private BGMSoundData.BGM music = BGMSoundData.BGM.Dungeon;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }
    public void PlayBGM(BGMSoundData.BGM bgm)
    {
        music = bgm;
        BGMSoundData data = bgmSoundDatas.Find(data => data.bgm == bgm);
        bgmAudioSource.clip = data.audioClip;
        bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
        bgmAudioSource.Play();
    }


    public void PlaySE(SESoundData.SE se)
    {
        SESoundData data = seSoundDatas.Find(data => data.se == se);
        if (data != null) 
        {
            seAudioSource.volume = data.volume * seMasterVolume * masterVolume;
            seAudioSource.PlayOneShot(data.audioClip);
        }
        else
        {
            Debug.LogWarning($"SE data not found for: {se}");
        }
        //SESoundData data = seSoundDatas.Find(data => data.se == se);
        //seAudioSource.volume = data.volume * seMasterVolume * masterVolume;
        //seAudioSource.PlayOneShot(data.audioClip);
    }

    public void ChangeSFXVolume(float value)
    {
        seMasterVolume = value;
    }

    public void ChangeMusicVolume(float value)
    {
        BGMSoundData data = bgmSoundDatas.Find(data => data.bgm == music);
        bgmMasterVolume = value;
        bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
    }
}

[System.Serializable]
public class BGMSoundData
{
    public enum BGM
    {
        
        Dungeon,
        Run,
    }

    public BGM bgm;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}

[System.Serializable]
public class SESoundData
{
    public enum SE
    {
        Attack,
        Shield,
        GhoastProjectile,
        Leap,
        Dash,
        Run,
        WarriorBoomerang,
        Fire,
        PushingWave,
        GuardAttack,
        GuardSpecialAttack,
        PlayerExplosion,
        PlayerKnife,
        HitProjectile,
        PlayerDeath,
        PlayerHit,
    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}
//SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title);
//SoundManager.Instance.PlaySE(SESoundData.SE.Title);
