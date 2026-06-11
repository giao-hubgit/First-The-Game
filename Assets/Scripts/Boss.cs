using UnityEngine;

public class Boss : Enemy
{
    public BossData bossData => data as BossData;

    [HideInInspector] public bool isPhase2 = false;

    protected override void Awake()
    {
        base.Awake();
        //thanh máu UI
    }

    public override void takeDmg(int damage)
    {
        base.takeDmg(damage);

        if (!isPhase2 && bossData != null)
        {
            float healthPercentage = (float)currentHP / bossData.maxHP;
            if (healthPercentage <= bossData.phase2HealthThreshold)
            {
                EnterPhase2();
            }
        }
    }

    private void EnterPhase2()
    {
        isPhase2 = true;
        Debug.Log("Phase 2");
        // Particle, SFX, Sprite change or sth
    }

    protected override void Die()
    {
        Debug.Log("Boss tèo, Spawn cổng qua màn");
        base.Die();
    }
}