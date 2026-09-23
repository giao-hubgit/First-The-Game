using UnityEngine;
using Unity.Cinemachine;

public class Explode : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    public float explosionDmg = 100f;
    public LayerMask HitTarget;

    [Header("VFX & SFX")]
    public string shockwaveVFX = "Shockwave";
    public string explosionVFX = "Explosion";
    public string explosionLight = "ExplosionLight";
    public AudioClip explosionSFX;

    private CinemachineImpulseSource impulseSource;
    private bool isExploded = false;

    public bool IsExploded => isExploded;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void DoExplosion()
    {
        if (isExploded) return;
        isExploded = true;

        if (TryGetComponent<Collider2D>(out Collider2D col))
        {
            col.enabled = false;
        }

        if (explosionSFX != null)
        {
            SFXManager.Instance?.PlaySFX(explosionSFX, transform.position);
        }

        if (impulseSource != null)
        {
            CameraShakeManager.Instance?.CameraShake(impulseSource, 1f);
        }

        ObjectPooler.Instance?.SpawnFromPool(shockwaveVFX, transform.position, Quaternion.identity);
        ObjectPooler.Instance?.SpawnFromPool(explosionVFX, transform.position, Quaternion.identity);
        ObjectPooler.Instance?.SpawnFromPool(explosionLight, transform.position, Quaternion.identity);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, HitTarget);

        foreach (Collider2D hitCollider in colliders)
        {
            if (hitCollider.gameObject == gameObject) continue;

            Landmine lm = hitCollider.GetComponent<Landmine>();
            if (lm != null && !lm.touched)
            {
                lm.touched = true;
                lm.BoomImmediatly();
            }

            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.takeDmg(explosionDmg);
            }

            Rigidbody2D rb = hitCollider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                AddExplosionForce(rb, explosionForce, transform.position, explosionRadius);
            }

            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.isCrashing = true;
            }
        }
    }

    public static void AddExplosionForce(Rigidbody2D rb, float force, Vector2 explosionPosition, float radius)
    {
        Vector2 explosionDir = rb.position - explosionPosition;
        float explosionDistance = explosionDir.magnitude;
        float wearoff = 1f - (explosionDistance / radius);
        rb.AddForce(explosionDir.normalized * force * wearoff * rb.mass, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}