using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using UnityEngine;
using Unity.Cinemachine;

public class Explode : MonoBehaviour, IDamageable
{
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    public int explosionDmg = 100;
    public int health = 100;

    public LayerMask HitTarget;
    private bool isExploded = false;
    public string shockwaveVFX = "Shockwave";
    public string explosionVFX = "Explosion";
    private CinemachineImpulseSource impulseSource;

    [SerializeField] AudioClip breakSFX;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void takeDmg(int dmg)
    {
        if (isExploded) return;

        health -= dmg;
        if (health <= 0) Boom();
    }

    private Collider2D[] results = new Collider2D[20];

    void Boom()
    {
        if (isExploded) return;
        isExploded = true;

        if (TryGetComponent<Collider2D>(out Collider2D col)) col.enabled = false;

        SFXManager.Instance?.PlaySFX(breakSFX, transform.position);
        CameraShakeManager.Instance?.CameraShake(impulseSource, 1f);
        ObjectPooler.Instance?.SpawnFromPool(shockwaveVFX, transform.position, Quaternion.identity);
        ObjectPooler.Instance?.SpawnFromPool(explosionVFX, transform.position, Quaternion.identity);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, HitTarget);

        foreach (Collider2D hitCollider in colliders)
        {
            if (hitCollider.gameObject == gameObject) continue;

            Landmine lm = hitCollider.GetComponent<Landmine>();
            if (lm != null && lm.touched == false)
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
        }

        Destroy(gameObject);
    }

    public static void AddExplosionForce(Rigidbody2D rb, float force, Vector2 explosionPosition, float radius)
    {
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = explosionDir.magnitude;
        var wearoff = 1 - (explosionDistance / radius);
        rb.AddForce(explosionDir.normalized * force * wearoff * rb.mass, ForceMode2D.Impulse);
        rb.AddTorque(UnityEngine.Random.Range(-10f, 10f), ForceMode2D.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
