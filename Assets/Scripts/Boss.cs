using UnityEngine;

public class Boss : Enemy
{
    public BossData bossData => data as BossData;
    public bool isIntroFinished = false;
    public bool isTransforming = false;
    public bool isAttacking = false;
    public bool isInvulnerable = false;
    public Animator animator;

    [HideInInspector] public int currentPhase = 1;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        isInvulnerable = (!isIntroFinished || isTransforming);
    }

    public override void takeDmg(int damage)
    {
        if (isInvulnerable) return;

        base.takeDmg(damage);

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

        if (bossData.transformSFX != null) SFXManager.Instance?.PlaySFX(bossData.transformSFX, transform.position);
    }

    protected override void Die()
    {
        Debug.Log("Boss tèo, Spawn cổng qua màn");
        base.Die();
    }
}