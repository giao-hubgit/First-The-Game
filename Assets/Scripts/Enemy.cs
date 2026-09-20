using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour, IDamageable, IAttacker
{
    public EnemyData data;
    private float nextDamageTime = 0f;

    protected float currentHP;
    public bool isCrashing = false;
    private bool isDead = false;

    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    [Header("Death Settings")]
    [SerializeField] protected float fadeDuration = 0.5f;
    [SerializeField] protected Light2D[] spotLights;

    public event System.Action OnTakeDamage;

    protected virtual void Awake()
    {
        if (data != null) currentHP = data.maxHP;
        rb = GetComponent<Rigidbody2D>();

        if (spotLights == null || spotLights.Length == 0)
        {
            spotLights = GetComponentsInChildren<Light2D>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        Vector2 laserSpawnPos = transform.position;
        laserSpawnPos.y += 1f * transform.localScale.y;
        ObjectPooler.Instance?.SpawnFromPool(data.spawnAnimation, laserSpawnPos, transform.rotation);
        SFXManager.Instance?.PlaySFX(data.spawnSFX, transform.position, 0.3f, true, 0.75f, 1.25f);
    }

    public virtual void takeDmg(float damage)
    {
        if (isDead) return;

        currentHP -= damage;

        OnTakeDamage?.Invoke();

        if (currentHP <= 0)
        {
            isDead = true;
            Die();
        }
    }

    public virtual float dmgDealt(float damage)
    {
        return damage + data.baseDMG;
    }

    protected virtual void Die()
    {
        SFXManager.Instance?.PlaySFX(data.deathSFX, transform.position);
        HitStop.Instance?.Stop(data.deadHitStopDuration, true);

        if (ObjectPooler.Instance != null)
        {
            GameObject deathParticle = ObjectPooler.Instance.SpawnFromPool(data.deathParticle, transform.position, Quaternion.identity);
            if (deathParticle != null) deathParticle.transform.localScale = transform.localScale;

            ObjectPooler.Instance.SpawnFromPool(data.deathAnimation, transform.position, transform.rotation);
            ObjectPooler.Instance.SpawnFromPool(data.itemDrop, transform.position, transform.rotation);
        }

        Collider2D col = GetComponent<Collider2D>();
        NavMeshAgent navMesh = GetComponent<NavMeshAgent>();
        RedCubeRanged redCubeRanged = GetComponent<RedCubeRanged>();

        if (navMesh != null) navMesh.enabled = false;
        if (redCubeRanged != null) redCubeRanged.enabled = false;

        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float elapsed = 0f;
        Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        int lightCount = spotLights != null ? spotLights.Length : 0;
        float[] startRadiO = new float[lightCount];
        float[] startRadiI = new float[lightCount];
        float[] startIntensities = new float[lightCount];

        for (int i = 0; i < lightCount; i++)
        {
            if (spotLights[i] != null)
            {
                startRadiO[i] = spotLights[i].pointLightOuterRadius;
                startRadiI[i] = spotLights[i].pointLightInnerRadius;
                startIntensities[i] = spotLights[i].intensity;
            }
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            if (spriteRenderer != null)
            {
                float newAlpha = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            }

            for (int i = 0; i < lightCount; i++)
            {
                if (spotLights[i] != null)
                {
                    spotLights[i].pointLightOuterRadius = Mathf.Lerp(startRadiO[i], 0f, t);
                    spotLights[i].pointLightInnerRadius = Mathf.Lerp(startRadiI[i], 0f, t);
                    spotLights[i].intensity = Mathf.Lerp(startIntensities[i], 0f, t);
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.relativeVelocity.magnitude >= 4f && isCrashing)
        {
            SFXManager.Instance?.PlaySFX(data.crashSFX, transform.position);

            if ((collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("ClosedRoom")) && !collision.gameObject.TryGetComponent<Explode>(out Explode explosion_barrel))
            {
                this.takeDmg(data.collisionDMG);
            }
            else if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)
                    && !collision.gameObject.CompareTag("Player"))
            {
                damageable.takeDmg(dmgDealt(data.collisionDMG));
            }
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null && playerMovement != null)
            {
                if (!playerMovement.isDashing && Time.time >= nextDamageTime)
                {
                    player.takeDmg(dmgDealt(data.collisionDMG));
                    nextDamageTime = Time.time + data.damageRate;
                }
            }
        }
        else if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)
                && !collision.gameObject.CompareTag("Enemy")
                && !collision.gameObject.TryGetComponent<Explode>(out Explode explode))
        {
            if (damageable != null && Time.time >= nextDamageTime)
            {
                damageable.takeDmg(dmgDealt(data.collisionDMG));
                nextDamageTime = Time.time + data.damageRate;
            }
        }
    }

    protected virtual void Update()
    {
        if (isCrashing)
        {
            if (rb.linearVelocity.magnitude < 4f)
            {
                isCrashing = false;
            }
        }
    }
}