using UnityEngine;
using Unity.Cinemachine;

public class SlowMotionCameraFix : MonoBehaviour
{
    void Update()
    {
        CinemachineCore.UniformDeltaTimeOverride = Time.unscaledDeltaTime;
    }
}
