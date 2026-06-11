using UnityEngine;

public class BossAI : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveTowards(Vector2 targetPosition, float speed)
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    public void SetVelocity(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    public void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
    }
}