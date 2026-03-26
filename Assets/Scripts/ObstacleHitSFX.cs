using UnityEngine;

public class ObstacleHitSFX : MonoBehaviour
{
    [SerializeField] AudioClip hitSFX;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Bullet>(out Bullet bullet) || other.gameObject.TryGetComponent<Bullet_e>(out Bullet_e bullet_e))
        {
            SFXManager.Instance?.PlaySFX(hitSFX, transform.position, 0.15f, true, 2f, 3f);
        }
    }
}
