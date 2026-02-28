using UnityEngine;
using UnityEngine.Pool; // Cần dùng thư viện này

public class Bullet : MonoBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. VFX: Lấy từ Pool chung, không Instantiate/Destroy nữa
        ObjectPooler.Instance.SpawnFromPool("BulletHitVFX", transform.position, Quaternion.identity);

        // 2. Gây sát thương
        if (hitInfo.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.takeDmg(damage);
        }

        // 3. Trả về Pool
        gameObject.SetActive(false);
    }
}