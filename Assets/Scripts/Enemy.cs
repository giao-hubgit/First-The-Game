using System.Numerics;
using System.Xml.Serialization;
using System;
using UnityEngine;
using Unity.VisualScripting;
using System.Data.Common;
using Unity.Mathematics;

public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyData data;
    private float nextDamageTime = 0f;

    protected int currentHP;
    public bool isCrashing = false;
    private bool isDead = false;

    protected Rigidbody2D rb;

    public EntityHurtsVFX enemyHurtsVFX;

    protected virtual void Awake()
    {
        if (data != null) currentHP = data.maxHP;
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void takeDmg(int damage)
    {
        if (isDead) return;

        currentHP -= damage;

        if (enemyHurtsVFX != null)
        {
            enemyHurtsVFX.PlayOnDamageVFX();
        }

        if (currentHP <= 0)
        {
            isDead = true;
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
        SFXManager.Instance?.PlaySFX(data.deathSFX, transform.position);

        GameObject deathParticle = ObjectPooler.Instance.SpawnFromPool(data.deathParticle, transform.position, UnityEngine.Quaternion.identity);
        deathParticle.transform.localScale = transform.localScale;

        ObjectPooler.Instance.SpawnFromPool(data.deathAnimation, transform.position, transform.rotation);

        ObjectPooler.Instance.SpawnFromPool(data.itemDrop, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude >= 4f && isCrashing == true)
        {
            SFXManager.Instance?.PlaySFX(data.crashSFX, transform.position);

            if (collision.gameObject.CompareTag("Wall") && !collision.gameObject.TryGetComponent<Explode>(out Explode explosion_barrel))
            {
                this.takeDmg(data.collisionDMG);
            }
            else if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)
                    && !collision.gameObject.CompareTag("Player"))
            {
                damageable.takeDmg(data.collisionDMG);
            }
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null && playerMovement != null)
            {
                if (playerMovement.isDashing != true && Time.time >= nextDamageTime)
                {
                    player.takeDmg(data.collisionDMG);
                    nextDamageTime = Time.time + data.damageRate;
                }
            }
        }

        else if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)
                && !collision.gameObject.CompareTag("Enemy")
                && !collision.gameObject.TryGetComponent<Explode>(out Explode explode))
        {
            if (damageable != null && Time.time >= nextDamageTime)
            {
                damageable.takeDmg(data.collisionDMG);
                nextDamageTime = Time.time + data.damageRate;
            }
        }
    }

    protected virtual void Update()
    {
        if (isCrashing == true)
        {
            if (rb.linearVelocity.magnitude < 4f)
            {
                isCrashing = false;
            }
        }
    }
}
