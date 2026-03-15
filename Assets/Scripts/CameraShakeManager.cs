using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void CameraShake(CinemachineImpulseSource impulseSource, float shakeForce)
    {
        if (impulseSource == null) return;

        Vector3 randomDirection = Random.insideUnitCircle.normalized;

        impulseSource.GenerateImpulse(randomDirection * shakeForce);
    }
}