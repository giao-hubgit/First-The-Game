using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class MeleeHitbox : MonoBehaviour
{
    [Header("Damage & Knockback Stats")]
    public float damage;
    public float knockback;
    public float hitImpact;

    [Header("Reflection Settings")]
    public bool canReflectBullets = false;
    public float reflectForce;
    public string reflectedBullet;

    [Header("Hit Target Settings")]
    public bool vulnerabilityIgnore;
    public LayerMask hitTargetMask;
    public AudioClip hitSFX;

    private CinemachineImpulseSource impulseSource;
    private List<Collider2D> alreadyHit = new List<Collider2D>();
    private IAttacker ownerAttacker;
    private Transform ownerTransform;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable()
    {
        alreadyHit.Clear();

        ownerAttacker = GetComponentInParent<IAttacker>();
        if (ownerAttacker is MonoBehaviour ownerMono)
        {
            ownerTransform = ownerMono.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyHit.Contains(other)) return;
        if (ownerTransform != null && (other.transform == ownerTransform || other.transform.IsChildOf(ownerTransform))) return;

        if (((1 << other.gameObject.layer) & hitTargetMask) != 0)
        {
            SFXManager.Instance?.PlaySFX(hitSFX, transform.position, 0.4f, true, 0.75f, 1.5f);

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                CameraShakeManager.Instance?.CameraShake(impulseSource, hitImpact);

                float finalDamage = ownerAttacker != null ? ownerAttacker.dmgDealt(damage) : damage;
                damageable.takeDmg(finalDamage);

                if (!vulnerabilityIgnore)
                {
                    alreadyHit.Add(other);
                }
            }

            if (canReflectBullets && reflectForce > 0)
            {
                HandleReflection(other);
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
                rb.AddTorque(Random.Range(-6f, 6f), ForceMode2D.Impulse);
            }
        }
    }

    private void HandleReflection(Collider2D other)
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

                if (enemyBullet.data != null && playerBullet.data != null)
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
                targetRb.AddTorque(Random.Range(-6f, 6f), ForceMode2D.Impulse);
            }
        }
    }
}