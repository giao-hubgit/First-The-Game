using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;
    public GameObject deathEffect;
    public Entity_VFX playerVFX;
    [SerializeField] private ParticleSystem DeathParticle;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void takeDmg(int damage)
    {
        currentHP -= damage;

        if (playerVFX != null)
        {
            playerVFX.PlayOnDamageVFX();
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
}
