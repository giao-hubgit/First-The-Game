using System.Xml.Serialization;
using System;
using UnityEngine;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour, IDamageable
{
    public int maxHP = 100;
    public int CollisionDMG = 20;
    protected int currentHP;
    protected bool isCrashing = false;
    public string DeathParticle = "EnemyDeathParticle";
    public string DeathAnimation = "EnemyDeathAnimation";

    protected Rigidbody2D rb;

    public EntityHurtsVFX enemyHurtsVFX;

    [SerializeField] protected AudioClip deathSFX;
    [SerializeField] protected AudioClip crashSFX;

    private void Awake()
    {
        currentHP = maxHP;

        rb = GetComponent<Rigidbody2D>();
    }

    public void takeDmg(int damage)
    {
        currentHP -= damage;

        if (enemyHurtsVFX != null)
        {
            enemyHurtsVFX.PlayOnDamageVFX();
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        /*if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        if (DeathParticle != null)
        {
            ParticleSystem particle = Instantiate(DeathParticle, transform.position, Quaternion.identity);
            particle.Play();
            Destroy(particle.gameObject, 2f);
        }*/
        SFXManager.Instance?.PlaySFX(deathSFX, transform.position);

        ObjectPooler.Instance.SpawnFromPool(DeathParticle, transform.position, Quaternion.identity);

        ObjectPooler.Instance.SpawnFromPool(DeathAnimation, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null && playerMovement != null)
            {
                if (playerMovement.isDashing != true)
                {
                    player.takeDmg(CollisionDMG);
                }
                else
                {
                    isCrashing = true;
                }
            }
        }
        if (isCrashing == true)
        {
            SFXManager.Instance?.PlaySFX(crashSFX, transform.position);

            if (collision.gameObject.CompareTag("Wall"))
            {
                this.takeDmg(CollisionDMG);
                isCrashing = false;
            }
            else if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable) && collision.gameObject != this.gameObject)
            {
                damageable.takeDmg(CollisionDMG);
            }
        }
    }

    protected virtual void Update()
    {
        if (isCrashing == true)
        {
            if (rb.linearVelocity.magnitude <= 2f)
            {
                isCrashing = false;
            }
        }
    }
}
