using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance { get; private set; }

    public float shakeMultiplier = 1f;
    private const string SCREENSHAKE_KEY = "Screenshake";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            shakeMultiplier = PlayerPrefs.GetFloat(SCREENSHAKE_KEY, 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetAmount(float screenshake)
    {
        shakeMultiplier = screenshake;
    }

    public void CameraShake(CinemachineImpulseSource impulseSource, float shakeForce)
    {
        if (impulseSource == null) return;

        Vector3 randomDirection = Random.insideUnitCircle.normalized;

        impulseSource.GenerateImpulse(randomDirection * shakeForce * shakeMultiplier);
    }
}