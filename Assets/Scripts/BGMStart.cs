using UnityEngine;

public class BGMStart : MonoBehaviour
{
    [Header("BGM Settings")]
    [SerializeField] private AudioClip bgmClip;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;

    private void Start()
    {
        if (BGMManager.Instance != null && bgmClip != null)
        {
            BGMManager.Instance.PlayBGM(bgmClip, volume);
        }
    }
}