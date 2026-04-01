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
        if (collision.relativeVelocity.magnitude >= 6f || isFlying == true)
        {
            SFXManager.Instance?.PlaySFX(enemyHitSFX, transform.position, 0.3f, true, 0.5f, 1.0f);
        }
    }
}
