using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IDamageable
{
    public int maxHP = 100;
    public float invulnerabilityTime = 0.5f;
    private bool isInvulnerable = false;
    private int currentHP;
    public GameObject deathEffect;
    public EntityHurtsVFX playerHurtsVFX;
    [SerializeField] AudioClip hurtVFX;
    [SerializeField] private ParticleSystem DeathParticle;

    public static Transform Instance;

    private void Awake()
    {
        Instance = this.transform;
        currentHP = maxHP;
    }

    public void takeDmg(int damage)
    {
        currentHP -= damage;

        if (isInvulnerable) return;

        if (playerHurtsVFX != null)
        {
            playerHurtsVFX.PlayOnDamageVFX();
            SFXManager.Instance?.PlaySFX(hurtVFX, transform.position);
        }

        if (currentHP <= 0)
        {
            HitStop.Instance?.Stop(0.075f);
            Die();
        }
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        if (DeathParticle != null)
        {
            ParticleSystem particle = Instantiate(DeathParticle, transform.position, Quaternion.identity);
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
