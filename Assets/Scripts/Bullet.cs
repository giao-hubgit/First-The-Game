using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    public BulletData data;
    [SerializeField] string ignoreTag = "Player";

    protected virtual void OnTriggerEnter2D(Collider2D hitInfo)
    {
        ObjectPooler.Instance.SpawnFromPool(data.bulletHitVFX, transform.position, Quaternion.identity);

        if (hitInfo.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            bool isIgnored = !string.IsNullOrEmpty(ignoreTag) && hitInfo.gameObject.CompareTag(ignoreTag);

            if (!isIgnored)
            {
                if (data.knockback > 0)
                {
                    if (hitInfo.TryGetComponent<Enemy>(out Enemy enemy))
                    {
                        enemy.isCrashing = true;
                    }

                    if (hitInfo.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                    {
                        rb.AddForce(transform.up * data.knockback * (rb.mass), ForceMode2D.Impulse);
                        rb.AddTorque(UnityEngine.Random.Range(-6f, 6f), ForceMode2D.Impulse);
                    }
                }
                damageable.takeDmg(data.damage);
            }
        }

        gameObject.SetActive(false);
    }
}