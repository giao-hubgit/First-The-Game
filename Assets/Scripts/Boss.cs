using UnityEngine;
using System;
using Unity.Cinemachine;
using NUnit.Framework;
using UnityEngine.Rendering.Universal;

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

        if (bossData.transformSFX != null) SFXManager.Instance?.PlaySFX(bossData.transformSFX, transform.position);
        if (bossData.musicSFX != null && currentPhase - 1 < bossData.musicSFX.Count) BGMManager.Instance?.PlayBGM(bossData.musicSFX[currentPhase - 1]);
    }

    protected override void Die()
    {
        Debug.Log("Boss tèo, Spawn cổng qua màn");
        OnBossDeath?.Invoke();
        if (bossData.musicSFX != null) BGMManager.Instance?.StopBGM();
        base.Die();
    }
}