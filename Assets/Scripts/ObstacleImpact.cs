using Unity.VisualScripting;
using UnityEngine;

public class ObstacleImpact : MonoBehaviour
{
    public bool isFlying = false;
    public int ObstacleCollisionDMG = 20;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude >= 4f && isFlying == true)
        {
            if (this.gameObject.TryGetComponent<IDamageable>(out IDamageable this_damageable))
            {
                this_damageable.takeDmg(ObstacleCollisionDMG);
            }

            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable that_damageable))
            {
                if (collision.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement pMovement))
                {
                    if (pMovement.isDashing == false)
                    {
                        that_damageable.takeDmg(ObstacleCollisionDMG);
                    }
                }
                else
                {
                    that_damageable.takeDmg(ObstacleCollisionDMG);
                }
            }
        }
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude >= 4)
        {
            if (isFlying == false)
            {
                isFlying = true;
            }
        }

        else if (rb.linearVelocity.magnitude < 4f)
        {
            if (isFlying == true)
            {
                isFlying = false;
            }
        }
    }
}
