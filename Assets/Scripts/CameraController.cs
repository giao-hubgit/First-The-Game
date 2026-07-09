using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public Transform cameraTarget;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ChangeRoom(Vector3 roomCenterPosition)
    {
        Vector3 newTargetPos = new Vector3(roomCenterPosition.x, roomCenterPosition.y, cameraTarget.position.z);

        cameraTarget.position = newTargetPos;
    }
}