using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;


public class MeleeHitbox : MonoBehaviour
{
    public int damage;
    public float knockback;
    public float reflectForce;
    public float hitImpact;
    public string reflectedBullet;
    public LayerMask hitTargetMask;
    public AudioClip hitSFX;
    private CinemachineImpulseSource impulseSource;


    private List<Collider2D> alreadyHit = new List<Collider2D>();

    private void Start()
    {
        impulseSource = GetComponent<Unity.Cinemachine.CinemachineImpulseSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || alreadyHit.Contains(other)) return;

        if (((1 << other.gameObject.layer) & hitTargetMask) != 0)
        {
            SFXManager.Instance?.PlaySFX(hitSFX, transform.position, 0.4f, true, 0.75f, 1.5f);

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                CameraShakeManager.Instance?.CameraShake(impulseSource, hitImpact);
                damageable.takeDmg(damage);
                alreadyHit.Add(other);
            }

            if (reflectForce > 0)
            {
                if (other.GetComponent<Bullet>() != null && other.CompareTag("EnemyBullet"))
                {
                    GameObject reflected_bullet = ObjectPooler.Instance.SpawnFromPool(reflectedBullet, other.transform.position, Quaternion.identity);
                    reflected_bullet.transform.localScale = other.transform.localScale;

                    if (other.transform.TryGetComponent<Bullet>(out Bullet enemyBullet) &&
                        other.transform.TryGetComponent<TrailRenderer>(out TrailRenderer enemyTrail))
                    {
                        Bullet playerBullet = reflected_bullet.GetComponent<Bullet>();
                        TrailRenderer playerTrail = reflected_bullet.GetComponent<TrailRenderer>();

                        if (enemyBullet.data != null)
                        {
                            BulletData clonedData = Instantiate(enemyBullet.data);

                            clonedData.damage *= 1;

                            playerBullet.data.damage = clonedData.damage;
                            playerBullet.data.knockback = clonedData.knockback;
                        }

                        if (playerTrail != null)
                        {
                            playerTrail.widthCurve = enemyTrail.widthCurve;
                            playerTrail.widthMultiplier = enemyTrail.widthMultiplier;

                            playerTrail.Clear();
                        }
                    }

                    Rigidbody2D targetRb = reflected_bullet.GetComponent<Rigidbody2D>();
                    if (targetRb != null)
                    {
                        targetRb.AddForce(transform.up * reflectForce * targetRb.mass, ForceMode2D.Impulse);
                        targetRb.AddTorque(UnityEngine.Random.Range(-6f, 6f), ForceMode2D.Impulse);
                    }
                }
            }

            if (knockback > 10)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null) enemy.isCrashing = true;
            }

            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(transform.up * knockback * rb.mass, ForceMode2D.Impulse);
                rb.AddTorque(UnityEngine.Random.Range(-6f, 6f), ForceMode2D.Impulse);

            }
        }
    }
}
