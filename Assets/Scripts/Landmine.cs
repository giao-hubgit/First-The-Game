using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Landmine : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    public int explosionDmg = 100;
    public LayerMask HitTarget;

    [SerializeField] AudioClip tickSFX;
    [SerializeField] AudioClip explosionSFX;

    public string shockwaveVFX = "Shockwave";
    public string explosionVFX = "Explosion";

    private CinemachineImpulseSource impulseSource;
    private Animator anim;
    public bool touched = false;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (touched == false)
        {
            if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
            {
                StartCoroutine(Boom());
                touched = true;
            }
        }
    }

    IEnumerator Boom()
    {
        int totalTicks = 16;
        float currentDelay = 1f;
        float speedUpFactor = 0.85f;
        if (anim != null) anim.speed = 1f;

        for (int i = 0; i < totalTicks; i++)
        {
            SFXManager.Instance?.PlaySFX(tickSFX, transform.position, 0.2f, false);

            yield return new WaitForSeconds(currentDelay);

            currentDelay = currentDelay * speedUpFactor;

            if (anim != null)
            {
                anim.speed = anim.speed / speedUpFactor;
            }
        }

        SFXManager.Instance?.PlaySFX(explosionSFX, transform.position);

        if (TryGetComponent<Collider2D>(out Collider2D col)) col.enabled = false;

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

            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.isCrashing = true;
            }
        }

        Destroy(gameObject);
    }

    public void BoomImmediatly()
    {
        SFXManager.Instance?.PlaySFX(explosionSFX, transform.position);

        if (TryGetComponent<Collider2D>(out Collider2D col)) col.enabled = false;

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
