using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [SerializeField] private AudioSource sfxPrefab; // Prefab có sẵn AudioSource
    [SerializeField] private int poolSize = 10;      // Số lượng tạo sẵn ban đầu

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
        // Tìm AudioSource nào đang rảnh
        foreach (var source in sfxPool)
        {
            if (!source.gameObject.activeInHierarchy)
            {
                return source;
            }
        }

        // Nếu hết chỗ trong pool, tạo thêm cái mới
        return CreateNewPoolObject();
    }

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        AudioSource source = GetAvailableSource();

        // Thiết lập vị trí và thông số
        source.transform.position = position;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;

        source.gameObject.SetActive(true);
        source.Play();

        // Tự động thu hồi sau khi chạy xong clip
        StartCoroutine(ReturnObject(source, clip.length));
    }

    private IEnumerator ReturnObject(AudioSource source, float duration)
    {
        // Đợi theo thời gian thực để tránh bị ảnh hưởng bởi Pause game (Time.timeScale = 0)
        yield return new WaitForSeconds(duration);

        // Dừng âm thanh chắc chắn trước khi tắt
        source.Stop();
        source.gameObject.SetActive(false);
    }
}