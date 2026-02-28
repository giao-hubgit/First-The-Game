using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    [SerializeField] private AudioClip bulletSFXClip;

    // Object Pool cho đạn
    private IObjectPool<GameObject> bulletPool;


    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed) Shoot();
    }

    void Shoot()
    {
        // Lấy đạn từ Pool thay vì Instantiate
        GameObject bullet = ObjectPooler.Instance.SpawnFromPool("PlayerBullet", firePoint.position, firePoint.rotation);

        // Đặt vị trí và hướng
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        // Phát âm thanh
        SFXManager.Instance?.PlaySFX(bulletSFXClip, transform.position);
    }
}