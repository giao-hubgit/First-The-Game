using UnityEngine;
using System;
using Unity.Cinemachine;
using NUnit.Framework;
using UnityEngine.Rendering.Universal;
using System.Collections;
using Unity.VisualScripting;

public class Boss : Enemy
{
    public BossData bossData => data as BossData;
    public bool isIntroFinished = false;
    public bool isTransforming = false;
    public bool isAttacking = false;
    public bool isInvulnerable = false;
    public event Action<float> OnHealthChanged;
    public event Action OnBossDeath;
    public Animator animator;

    public event Action OnBossIntroStarted;
    public event Action OnBossIntroFinished;
    public event Action<int> OnPhaseChanged;

    [SerializeField] private BossHealthBar bossHealthBarUI;
    [SerializeField] Light2D[] spotLight2d;

    [HideInInspector] public int currentPhase = 1;

    protected override void Awake()
    {
        base.Awake();

        spotLight2d = GetComponentsInChildren<Light2D>();

        foreach (Light2D child in spotLight2d)
        {
            if (child.TryGetComponent<Light2D>(out _))
            {
                child.gameObject.SetActive(false);
            }
        }

        BGMManager.Instance?.PlayBGM(bossData.musicSFX[currentPhase - 1]);
    }

    public void Start()
    {
        OnBossIntroStarted?.Invoke();
    }

    public float PhaseTransitionDuration => bossData.phaseTransitionDuration;
    public float DeathDuration => bossData.deathDuration;

    public void FinishIntro()
    {
        isIntroFinished = true;

        SFXManager.Instance?.PlaySFX(bossData.bossIntroEndSFX, transform.position);

        if (BossHealthBar.Instance != null)
        {
            BossHealthBar.Instance.InitHealthBar(this);
        }

        foreach (Light2D child in spotLight2d)
        {
            if (child.TryGetComponent<Light2D>(out _))
            {
                child.gameObject.SetActive(true);
            }
        }

        OnBossIntroFinished?.Invoke();
    }

    protected override void Update()
    {
        base.Update();
        isInvulnerable = (!isIntroFinished || isTransforming);
    }

    public override void takeDmg(float damage)
    {
        if (isInvulnerable) return;

        base.takeDmg(damage);

        float healthPercent = (float)currentHP / bossData.maxHP;
        OnHealthChanged?.Invoke(healthPercent);

        if (bossData != null && bossData.phaseThresholds != null && bossData.phaseThresholds.Length > 0)
        {
            float healthPercentage = (float)currentHP / bossData.maxHP;

            if (currentPhase - 1 < bossData.phaseThresholds.Length)
            {
                float nextThreshold = bossData.phaseThresholds[currentPhase - 1];

                if (healthPercentage <= nextThreshold)
                {
                    EnterNextPhase();
                }
            }
        }
    }

    private void EnterNextPhase()
    {
        currentPhase++;
        Debug.Log($"CHUYỂN SANG PHASE {currentPhase}");

        if (animator != null)
        {
            animator.SetInteger("Phase", currentPhase);
        }

        OnPhaseChanged?.Invoke(currentPhase);

        HitStop.Instance?.Stop(data.deadHitStopDuration, true);

        if (bossData.transformSFX != null) SFXManager.Instance?.PlaySFX(bossData.transformSFX, transform.position);
        if (bossData.musicSFX != null && currentPhase - 1 < bossData.musicSFX.Count) BGMManager.Instance?.PlayBGM(bossData.musicSFX[currentPhase - 1]);
    }

    protected override void Die()
    {
        Debug.Log("Boss tèo, Spawn cổng qua màn");

        OnBossDeath?.Invoke();

        animator.SetBool("isDead", true);

        if (bossData.musicSFX != null) BGMManager.Instance?.StopBGM();

        SFXManager.Instance?.PlaySFX(data.deathSFX, transform.position);
        HitStop.Instance?.Stop(data.deadHitStopDuration, true);

        if (ObjectPooler.Instance != null)
        {
            GameObject deathParticle = ObjectPooler.Instance.SpawnFromPool(data.deathParticle, transform.position, Quaternion.identity);
            if (deathParticle != null) deathParticle.transform.localScale = transform.localScale;

            ObjectPooler.Instance.SpawnFromPool(data.deathAnimation, transform.position, transform.rotation);
        }

        Collider2D col = GetComponent<Collider2D>();
        RedCubeRanged redCubeRanged = GetComponent<RedCubeRanged>();

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

    protected override IEnumerator FadeOutAndDestroy()
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

        while (elapsed < data.deadFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / data.deadFadeDuration;

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

        if (ObjectPooler.Instance != null)
        {
            GameObject deathParticle = ObjectPooler.Instance.SpawnFromPool(bossData.bossDeathParticle, transform.position, Quaternion.identity);
            if (deathParticle != null) deathParticle.transform.localScale = transform.localScale;

            ObjectPooler.Instance.SpawnFromPool(data.itemDrop, transform.position, transform.rotation);
        }

        SFXManager.Instance?.PlaySFX(data.deathSFX, transform.position);

        Destroy(gameObject);
    }
}
