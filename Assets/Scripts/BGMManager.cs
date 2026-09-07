using UnityEngine;
using UnityEngine.Audio;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioMixerGroup bgmGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip, float volume = 0.5f)
    {
        if (clip == null) return;

        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.outputAudioMixerGroup = bgmGroup;
        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
}