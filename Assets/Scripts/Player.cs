using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable, IAttacker
{
    [SerializeField] PlayerData data;
    [SerializeField] private float fadeDuration = 1f;
    private SpriteRenderer spriteRenderer;
    private bool isInvulnerable = false;
    public bool isDead = false;
    private float currentHP;

    public EntityHurtsVFX playerHurtsVFX;

    [SerializeField] Image hpBar;

    public static Transform Instance;
    public static event System.Action onPlayerDeath;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Instance = this.transform;
        currentHP = data.maxHP;
    }

    public void takeDmg(float damage)
    {
        if (isInvulnerable) return;

        currentHP -= damage;

        if (hpBar != null)
        {
            hpBar.fillAmount = Mathf.Max(0f, (float)currentHP / data.maxHP);
        }

        if (playerHurtsVFX != null)
        {
            playerHurtsVFX.PlayOnDamageVFX();
            SFXManager.Instance?.PlaySFX(data.hurtVFX, transform.position);
        }

        if (currentHP <= 0 && !isDead)
        {
            isDead = true;
            HitStop.Instance?.Stop(0.5f);
            Die();
        }
    }

    public float dmgDealt(float damage)
    {
        return damage * data.baseDMG;
    }

    private void Die()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.DeactivateInput();

        onPlayerDeath?.Invoke();
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

        SFXManager.Instance?.PlaySFX(data.deathSFX, transform.position);

        if (data.DeathParticle != null)
        {
            ParticleSystem particle = Instantiate(data.DeathParticle, transform.position, Quaternion.identity);
            particle.Play();
            Destroy(particle.gameObject, 10f);
        }

        yield return new WaitForSecondsRealtime(4f);

        Destroy(gameObject);
    }

    public void TriggerInvulnerability(float duration)
    {
        StartCoroutine(BecomeInvulnerable(duration));
    }

    private IEnumerator BecomeInvulnerable(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }
}
