using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    [SerializeField] public AudioSource bgmAudioSource;
    [SerializeField] public AudioSource seAudioSource;

    [SerializeField] public List<BGMSoundData> bgmSoundDatas;
    [SerializeField] public List<SESoundData> seSoundDatas;
    [SerializeField] private PlayerData playerHealth;
    public PlayerData GetPlayerHealth()
    {
        return playerHealth;
    }
    public float masterVolume = 1;
    public float bgmMasterVolume = 1;
    public float seMasterVolume = 1;

    private const float CrisisHp = 0.3f;
    private const int NormalHp = 200;
    public BGMSoundData.BGM music = BGMSoundData.BGM.Dungeon;
    private GameState currentGameState = GameState.Normal;
    //bool isCombat;
    
    public static SoundManager Instance { get; private set; }
    public enum GameState
    {
        Normal,
        Combat,
        Crisis,
    }
    public GameState CurrentGameState
    {
        get { return currentGameState; }
        set
        {
            if (currentGameState != value)
            {
                currentGameState = value;
                SetGameState(value);
            }
        }
    }
    

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PlayBGM(BGMSoundData.BGM.Dungeon);
        }
    }
    private void Update()
    {
        CheckPlayerHp();
    }

    public void SetGameState(GameState newState)
    {
        Debug.Log("3");
        if (currentGameState == newState) return;

        currentGameState = newState;
        Debug.Log("4");
        switch (newState)
        {
            case GameState.Normal:
                PlayBGM(BGMSoundData.BGM.Dungeon);
                Debug.Log("Normal");
                break;
            case GameState.Combat:
                PlayBGM(BGMSoundData.BGM.Combat);
                Debug.Log("Combat");
                break;
            case GameState.Crisis:
                PlayBGM(BGMSoundData.BGM.Crisis);
                Debug.Log("Crisis");
                break;
        }
    }
    private void CheckPlayerHp()
    {
        //var newGameState = currentGameState;
        if (currentGameState != GameState.Combat)
        {

            if (playerHealth.health <= CrisisHp * playerHealth.maxHealth)
            {
                SetGameState(GameState.Crisis);
            }
            if (playerHealth.health > CrisisHp * playerHealth.maxHealth)
            {
                SetGameState(GameState.Normal);
            }
        }
        
    }
    public void PlayBGM(BGMSoundData.BGM bgm)
    {
        Debug.Log("1");
        music = bgm;
        BGMSoundData data = bgmSoundDatas.Find(data => data.bgm == bgm);
        if (data != null)
        {
            bgmAudioSource.clip = data.audioClip;
            bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
            bgmAudioSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM data not found for: {bgm}");
        }
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
}

[System.Serializable]
public class BGMSoundData
{
    public enum BGM
    {
        Dungeon,
        Crisis,
        Combat,
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
        Warning,
    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}

//SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title);
//SoundManager.Instance.PlaySE(SESoundData.SE.Title);
//SoundManager.Instance.SetGameState(SoundManager.GameState.Combat);
//if (SoundManager.Instance.CurrentGameState == SoundManager.GameState.Combat)
//{
//    if (playerHealth == null)
//    {

//        Debug.Log("before");
//        if (playerHealth.health <= CrisisHp * playerHealth.maxHealth)
//        {
//            SoundManager.Instance.SetGameState(SoundManager.GameState.Crisis);
//        }
//        if (playerHealth.health > CrisisHp * playerHealth.maxHealth)
//        {
//            SoundManager.Instance.SetGameState(SoundManager.GameState.Normal);
//        }
//    }
