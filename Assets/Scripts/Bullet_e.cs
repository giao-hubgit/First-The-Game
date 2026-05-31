using UnityEngine;
using UnityEngine.Pool;

public class Bullet_e : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D hitInfo)
    {
        ObjectPooler.Instance.SpawnFromPool(data.bulletHitVFX, transform.position, Quaternion.identity);

        if (hitInfo.TryGetComponent<IDamageable>(out IDamageable damageable) && !hitInfo.gameObject.CompareTag("Enemy"))
        {
            damageable.takeDmg(data.damage);
        }

        gameObject.SetActive(false);
    }
}