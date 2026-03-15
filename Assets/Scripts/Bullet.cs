using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    public int damage = 20;
    public string bulletHitVFX = "BulletHitVFX";

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        ObjectPooler.Instance.SpawnFromPool(bulletHitVFX, transform.position, Quaternion.identity);

        if (hitInfo.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.takeDmg(damage);
        }

        gameObject.SetActive(false);
    }
}