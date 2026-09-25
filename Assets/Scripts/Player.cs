using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.GlobalIllumination;

public class Player : MonoBehaviour, IDamageable, IAttacker
{
    public PlayerData data;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] protected Light2D[] spotLights;
    private SpriteRenderer spriteRenderer;
    private bool isInvulnerable = false;
    public bool isDead = false;
    private float currentHP;

    [SerializeField] Image hpBar;

    public static Transform Instance;
    public static event System.Action onPlayerDeath;

    public event System.Action OnTakeDamage;

    [SerializeField] private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (spotLights == null || spotLights.Length == 0)
        {
            spotLights = GetComponentsInChildren<Light2D>();
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        Instance = this.transform;
        currentHP = data.maxHP;
    }

    private void Start()
    {
        Vector2 laserSpawnPos = transform.position;
        laserSpawnPos.y += 1.5f * transform.localScale.y;
        Instantiate(data.spawnLaser, laserSpawnPos, Quaternion.identity);
        SFXManager.Instance?.PlaySFX(data.spawnSFX, transform.position, 0.3f, true, 0.75f, 1.25f);
    }

    public void takeDmg(float damage)
    {
        if (isInvulnerable) return;

        currentHP -= damage;

        OnTakeDamage?.Invoke();

        HitStop.Instance?.Stop(damage * (1 / 1000));
        CameraShakeManager.Instance?.CameraShake(impulseSource, 0.25f);

        if (hpBar != null)
        {
            hpBar.fillAmount = Mathf.Max(0f, (float)currentHP / data.maxHP);
        }

        SFXManager.Instance?.PlaySFX(data.hurtSFX, transform.position);

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
        if (playerInput != null)
        {
            playerInput.DeactivateInput();
        }

        onPlayerDeath?.Invoke();
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        Color startColor = spriteRenderer.color;
        float elapsed = 0f;

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
                float newAlpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
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