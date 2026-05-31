using UnityEngine;
using UnityEngine.Pool;

public class Bullet_Trap : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D hitInfo)
    {
        ObjectPooler.Instance.SpawnFromPool(data.bulletHitVFX, transform.position, Quaternion.identity);

        if (hitInfo.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.takeDmg(data.damage);
        }

        gameObject.SetActive(false);
    }
}