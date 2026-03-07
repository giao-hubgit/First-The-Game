using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private bool isKnockedBack = false;

    public Transform target;
    private NavMeshAgent agent;
    private Rigidbody2D rb;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude > 2f && !isKnockedBack)
        {
            StartCoroutine(IKnockback(1f));
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
}