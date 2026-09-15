using UnityEngine;

public class RedCubeRanged : Enemy
{
    public Transform target;
    [SerializeField] private EnemyRangedWeapon rangedWeapon;

    protected virtual void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        if (rangedWeapon == null)
        {
            rangedWeapon = GetComponent<EnemyRangedWeapon>();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (target != null)
        {
            RotateTowardsPlayer();
            CheckLineOfSightAndShoot();
        }
    }

    protected virtual void RotateTowardsPlayer()
    {
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected virtual void CheckLineOfSightAndShoot()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer <= data.visionRange)
        {
            Vector2 directionToPlayer = (target.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, data.visionRange, data.lineOfSightLayer);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                rangedWeapon?.TryShoot();
            }
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.visionRange);
    }
}