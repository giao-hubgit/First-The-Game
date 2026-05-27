using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [SerializeField] private AudioSource sfxPrefab;
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
        newSource.gameObject.SetActive(false); // Tắt đi khi chưa dùng
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
        AudioSource source = GetAvailableSource();

        source.transform.position = position;
        source.clip = clip;
        source.volume = volume;

        if (randPitch == true)
        {
            source.pitch = Random.Range(minP, maxP);
        }
        else source.pitch = 1f;

        source.gameObject.SetActive(true);
        source.Play();

        StartCoroutine(ReturnObject(source, clip.length));
    }

    private IEnumerator ReturnObject(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);

        source.Stop();
        source.gameObject.SetActive(false);
    }
}