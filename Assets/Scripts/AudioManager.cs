using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    public AudioMixer mainMixer;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ApplyAllSavedVolumes();
    }

    public void ApplyAllSavedVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_KEY, 1f));
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_KEY, 1f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_KEY, 1f));
    }

    public void SetMasterVolume(float sliderValue)
    {
        ApplyVolume("MasterVolume", sliderValue);
        PlayerPrefs.SetFloat(MASTER_KEY, sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        ApplyVolume("MusicVolume", sliderValue);
        PlayerPrefs.SetFloat(MUSIC_KEY, sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        ApplyVolume("SFXVolume", sliderValue);
        PlayerPrefs.SetFloat(SFX_KEY, sliderValue);
    }

    private void ApplyVolume(string parameterName, float sliderValue)
    {
        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float volumeInDecibels = Mathf.Log10(clampedValue) * 20;
        mainMixer.SetFloat(parameterName, volumeInDecibels);
    }
}