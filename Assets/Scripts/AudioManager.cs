using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Components")]
    public AudioMixer mainMixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        float savedMusicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        if (masterSlider != null)
        {
            masterSlider.value = savedVolume;
        }
        if (musicSlider != null)
        {
            musicSlider.value = savedMusicVolume;
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFXVolume;
        }

        SetMasterVolume(savedVolume);
        SetMusicVolume(savedMusicVolume);
        SetSFXVolume(savedSFXVolume);
    }

    public void SetMasterVolume(float sliderValue)
    {
        float volumeInDecibels = Mathf.Log10(sliderValue) * 20;

        mainMixer.SetFloat("MasterVolume", volumeInDecibels);

        PlayerPrefs.SetFloat(MASTER_KEY, sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float volumeInDecibels = Mathf.Log10(sliderValue) * 20;

        mainMixer.SetFloat("MusicVolume", volumeInDecibels);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float volumeInDecibels = Mathf.Log10(sliderValue) * 20;

        mainMixer.SetFloat("SFXVolume", volumeInDecibels);
    }
}