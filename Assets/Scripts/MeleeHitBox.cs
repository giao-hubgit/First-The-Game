using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;


public class MeleeHitbox : MonoBehaviour
{
    public int damage;
    public float reflectForce;
    public float hitImpact;
    public LayerMask hitTargetMask;
    public AudioClip hitSFX;
    private CinemachineImpulseSource impulseSource;


    private List<Collider2D> alreadyHit = new List<Collider2D>();

    private void Start()
    {
        impulseSource = GetComponent<Unity.Cinemachine.CinemachineImpulseSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || alreadyHit.Contains(other)) return;

        if (((1 << other.gameObject.layer) & hitTargetMask) != 0)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                SFXManager.Instance?.PlaySFX(hitSFX, transform.position, 0.3f, true, 0.75f, 1.5f);
                CameraShakeManager.Instance?.CameraShake(impulseSource, hitImpact);
                damageable.takeDmg(damage);
                alreadyHit.Add(other);
            }

            if (reflectForce > 10)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null) enemy.isCrashing = true;
            }

            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(transform.up * reflectForce * rb.mass, ForceMode2D.Impulse);
            }
        }
    }
}