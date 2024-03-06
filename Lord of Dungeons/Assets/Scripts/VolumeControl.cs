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
        masterVolumeSlider.value = soundManager.masterVolume;
        bgmVolumeSlider.value = soundManager.bgmMasterVolume;
        seVolumeSlider.value = soundManager.seMasterVolume;

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

