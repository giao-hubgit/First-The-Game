using UnityEngine;

public class ObstacleHitSFX : ObstacleImpact
{
    [SerializeField] AudioClip hitSFX;
    [SerializeField] AudioClip enemyHitSFX;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Bullet>(out Bullet bullet) || other.gameObject.TryGetComponent<Bullet_e>(out Bullet_e bullet_e))
        {
            SFXManager.Instance?.PlaySFX(hitSFX, transform.position, 0.15f, true, 2f, 3f);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude >= 4f && isFlying == true)
        {
            if (this.gameObject.TryGetComponent<IDamageable>(out IDamageable this_damageable))
            {
                SFXManager.Instance?.PlaySFX(enemyHitSFX, transform.position, 0.15f, true, 2f, 3f);
            }

            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable that_damageable))
            {
                SFXManager.Instance?.PlaySFX(enemyHitSFX, transform.position, 0.15f, true, 2f, 3f);
            }
        }
    }
}
