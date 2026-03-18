using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private bool isKnockedBack = false;

    public Transform target;
    private NavMeshAgent agent;
    private Rigidbody2D rb;

    public float aggroRange = 12f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();

        target = Player.Instance;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude > 2f && !isKnockedBack)
        {
            StartCoroutine(IKnockback(1f));
        }

        if (target != null && !isKnockedBack && agent.enabled && agent.isOnNavMesh)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, target.position);

            if (distanceToPlayer <= aggroRange)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else
            {
                agent.isStopped = true;
            }
        }

        if (target != null && !isKnockedBack && agent.enabled)
        {
            agent.SetDestination(target.position);
        }
    }

    IEnumerator IKnockback(float duration)
    {
        isKnockedBack = true;

        agent.enabled = false;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        agent.enabled = true;
        isKnockedBack = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}