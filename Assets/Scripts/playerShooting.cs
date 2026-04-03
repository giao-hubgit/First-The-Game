using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint;
    public string bulletPrefabS = "PlayerBullet";
    public float bulletForce = 20f;
    [SerializeField] private AudioClip bulletSFXClip;

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed) Shoot();
    }

    void Shoot()
    {
        GameObject bullet = ObjectPooler.Instance.SpawnFromPool(bulletPrefabS, firePoint.position, firePoint.rotation);

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        SFXManager.Instance?.PlaySFX(bulletSFXClip, transform.position);
    }
}