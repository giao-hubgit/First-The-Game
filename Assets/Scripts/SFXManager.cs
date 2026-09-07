using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Settings & Mixer")]
    [SerializeField] private AudioSource sfxPrefab;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private int poolSize = 10;

    private List<AudioSource> sfxPool = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewPoolObject();
        }
    }

    private AudioSource CreateNewPoolObject()
    {
        AudioSource newSource = Instantiate(sfxPrefab, transform);

        if (sfxGroup != null)
        {
            newSource.outputAudioMixerGroup = sfxGroup;
        }

        newSource.gameObject.SetActive(false);
        sfxPool.Add(newSource);
        return newSource;
    }

    private AudioSource GetAvailableSource()
    {
        foreach (var source in sfxPool)
        {
            if (!source.gameObject.activeInHierarchy)
            {
                return source;
            }
        }

        return CreateNewPoolObject();
    }

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 0.3f, bool randPitch = true, float minP = 1f, float maxP = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSource();

        source.transform.position = position;
        source.clip = clip;
        source.volume = volume;

        if (randPitch)
        {
            source.pitch = Random.Range(minP, maxP);
        }
        else
        {
            source.pitch = 1f;
        }

        source.gameObject.SetActive(true);
        source.Play();

        float clipDuration = clip.length / Mathf.Abs(source.pitch);
        StartCoroutine(ReturnObject(source, clipDuration));
    }

    private IEnumerator ReturnObject(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);

        source.Stop();
        source.gameObject.SetActive(false);
    }
}