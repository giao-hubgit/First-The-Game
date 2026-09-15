using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamageable, IAttacker
{
    [SerializeField] PlayerData data;
    private bool isInvulnerable = false;
    private float currentHP;

    public EntityHurtsVFX playerHurtsVFX;

    [SerializeField] Image hpBar;

    public static Transform Instance;

    private void Awake()
    {
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

        if (currentHP <= 0)
        {
            HitStop.Instance?.Stop(0.075f, null);
            Die();
        }
    }

    public float dmgDealt(float damage)
    {
        return damage * data.baseDMG;
    }

    private void Die()
    {
        if (data.deathEffect != null)
        {
            GameObject effect = Instantiate(data.deathEffect, transform.position, transform.rotation);
            Destroy(effect, 1f);
        }

        if (data.DeathParticle != null)
        {
            ParticleSystem particle = Instantiate(data.DeathParticle, transform.position, Quaternion.identity);
            particle.Play();
            Destroy(particle.gameObject, 2f);
        }

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
