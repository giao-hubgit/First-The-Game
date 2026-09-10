using UnityEngine;

public class BossAI : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D bossCollider;

    [Header("Wall Collision Detection")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float checkDistance = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bossCollider = GetComponent<Collider2D>();
    }

    public void MoveTowards(Vector2 targetPosition, float speed)
    {
        Vector2 direction = (targetPosition - rb.position).normalized;

        if (CanMoveInDirection(direction))
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
        else
        {
            StopMoving();
        }
    }

    public void SetVelocity(Vector2 direction, float speed)
    {
        if (CanMoveInDirection(direction))
        {
            rb.linearVelocity = direction.normalized * speed;
        }
        else
        {
            StopMoving();
        }
    }

    public void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private bool CanMoveInDirection(Vector2 direction)
    {
        if (direction == Vector2.zero || bossCollider == null) return true;

        RaycastHit2D hit = Physics2D.BoxCast(
            bossCollider.bounds.center,
            bossCollider.bounds.size,
            0f,
            direction.normalized,
            checkDistance,
            wallLayer
        );

        return hit.collider == null;
    }
}