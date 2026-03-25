using UnityEngine;

public class RedCubeRanged : Enemy
{
    public Transform target;
    public Transform firePoint;
    public string bulletPrefabS = "EnemyBullet";
    public GameObject bulletPrefab;
    public float bulletForce = 10f;

    [SerializeField] AudioClip bulletSFXClip;

    public float visionRange = 8f;
    public float fireRate = 1.5f;
    private float nextFireTime = 0f;

    public LayerMask lineOfSightLayer;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
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

    void RotateTowardsPlayer()
    {
        Vector2 direction = target.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void CheckLineOfSightAndShoot()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer <= visionRange)
        {
            Vector2 directionToPlayer = (target.position - transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, visionRange, lineOfSightLayer);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                if (Time.time >= nextFireTime)
                {
                    Shoot(directionToPlayer);
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
    }

    void Shoot(Vector2 shootDirection)
    {
        GameObject bullet = ObjectPooler.Instance?.SpawnFromPool(bulletPrefabS, firePoint.position, firePoint.rotation);

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            rbBullet.linearVelocity = shootDirection * bulletForce;
        }

        if (bulletSFXClip != null)
        {
            SFXManager.Instance?.PlaySFX(bulletSFXClip, transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}