using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    public BulletData data;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        ObjectPooler.Instance.SpawnFromPool(data.bulletHitVFX, transform.position, Quaternion.identity);

        if (hitInfo.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            if (data.knockback > 0)
            {
                Enemy enemy = hitInfo.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.isCrashing = true;
                }

                Rigidbody2D rb = hitInfo.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(transform.up * data.knockback * (rb.mass), ForceMode2D.Impulse);
                    rb.AddTorque(UnityEngine.Random.Range(-6f, 6f), ForceMode2D.Impulse);
                }
            }
            damageable.takeDmg(data.damage);
        }

        gameObject.SetActive(false);
    }
}