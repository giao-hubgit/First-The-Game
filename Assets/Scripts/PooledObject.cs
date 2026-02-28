using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public float delay = 0.5f;

    private void OnEnable()
    {
        // Quan trọng: Reset lại thời gian mỗi khi lấy ra từ Pool
        Invoke(nameof(Deactivate), delay);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // Hủy các lệnh Invoke để tránh lỗi khi object bị tắt đột ngột
        CancelInvoke();
    }
}