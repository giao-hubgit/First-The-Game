using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyData data;
    private float nextDamageTime = 0f;

    protected int currentHP;
    public bool isCrashing = false;
    private bool isDead = false;

    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    [Header("Death Settings")]
    [SerializeField] protected float fadeDuration = 0.5f;

    public EntityHurtsVFX enemyHurtsVFX;

    protected virtual void Awake()
    {
        if (data != null) currentHP = data.maxHP;
        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        Vector2 laserSpawnPos = transform.position;
        laserSpawnPos.y += 1f * transform.localScale.y;
        ObjectPooler.Instance?.SpawnFromPool(data.spawnAnimation, laserSpawnPos, transform.rotation);
        SFXManager.Instance?.PlaySFX(data.spawnSFX, transform.position, 0.3f, true, 0.75f, 1.25f);
    }

    public virtual void takeDmg(int damage)
    {
        if (isDead) return;

        currentHP -= damage;

        if (enemyHurtsVFX != null)
        {
            enemyHurtsVFX.PlayOnDamageVFX();
        }

        if (currentHP <= 0)
        {
            isDead = true;
            Die();
        }
    }

    protected virtual void Die()
    {
        SFXManager.Instance?.PlaySFX(data.deathSFX, transform.position);

        if (ObjectPooler.Instance != null)
        {
            GameObject deathParticle = ObjectPooler.Instance.SpawnFromPool(data.deathParticle, transform.position, Quaternion.identity);
            if (deathParticle != null) deathParticle.transform.localScale = transform.localScale;

            ObjectPooler.Instance.SpawnFromPool(data.deathAnimation, transform.position, transform.rotation);
            ObjectPooler.Instance.SpawnFromPool(data.itemDrop, transform.position, transform.rotation);
        }

        Collider2D col = GetComponent<Collider2D>();
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
        if (spriteRenderer != null)
        {
            Color startColor = spriteRenderer.color;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float newAlpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.relativeVelocity.magnitude >= 4f && isCrashing == true)
        {
            SFXManager.Instance?.PlaySFX(data.crashSFX, transform.position);

            if ((collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("ClosedRoom")) && !collision.gameObject.TryGetComponent<Explode>(out Explode explosion_barrel))
            {
                this.takeDmg(data.collisionDMG);
            }
            else if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)
                    && !collision.gameObject.CompareTag("Player"))
            {
                damageable.takeDmg(data.collisionDMG);
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
                if (playerMovement.isDashing != true && Time.time >= nextDamageTime)
                {
                    player.takeDmg(data.collisionDMG);
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
                damageable.takeDmg(data.collisionDMG);
                nextDamageTime = Time.time + data.damageRate;
            }
        }
    }

    protected virtual void Update()
    {
        if (isCrashing == true)
        {
            if (rb.linearVelocity.magnitude < 4f)
            {
                isCrashing = false;
            }
        }
    }
}