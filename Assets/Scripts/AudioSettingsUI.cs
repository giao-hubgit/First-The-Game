using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void OnEnable()
    {
        if (masterSlider != null) masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        if (musicSlider != null) musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (sfxSlider != null) sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void OnMasterSliderChanged(float value)
    {
        AudioManager.Instance?.SetMasterVolume(value);
    }

    public void OnMusicSliderChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
    }

    public void OnSFXSliderChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }
}