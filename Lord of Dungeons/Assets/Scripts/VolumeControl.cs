using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider seVolumeSlider;
    public SoundManager soundManager;

    void Start()
    {
        // 初期スライダーの値を現在の音量に合わせます
        masterVolumeSlider.value = soundManager.masterVolume;
        bgmVolumeSlider.value = soundManager.bgmMasterVolume;
        seVolumeSlider.value = soundManager.seMasterVolume;

        // スライダーの値が変更されたときに呼び出されるメソッドを登録します
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        seVolumeSlider.onValueChanged.AddListener(SetSEVolume);
    }

    public void SetMasterVolume(float volume)
    {
        soundManager.masterVolume = volume;
    }

    public void SetBGMVolume(float volume)
    {
        soundManager.bgmMasterVolume = volume;
    }

    public void SetSEVolume(float volume)
    {
        soundManager.seMasterVolume = volume;
    }
}

