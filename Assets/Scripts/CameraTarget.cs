using UnityEngine;
using UnityEngine.InputSystem; // ⭐ Bắt buộc phải thêm dòng này

public class CameraTarget : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera mainCamera;

    [Header("Settings")]
    public float maxDistance = 3f;

    void Update()
    {
        if (player == null) return;

        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f));
        mousePos.z = 0f;

        Vector3 direction = mousePos - player.position;
        float distance = direction.magnitude;

        float clampedDistance = Mathf.Clamp(distance, 0f, maxDistance);

        transform.position = player.position + direction.normalized * (clampedDistance / 2f);
    }
}