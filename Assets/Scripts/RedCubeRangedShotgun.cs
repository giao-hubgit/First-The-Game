using UnityEngine;

public class RedCubeRangedShotgun : RedCubeRanged
{
    protected int maxBullets = 5;
    protected float spreadAngle = 45f;

    protected override void Update()
    {
        base.Update();

        if (target != null)
        {
            RotateTowardsPlayer();
            CheckLineOfSightAndShoot();
        }
    }

    protected override void Shoot(Vector2 shootDirection)
    {
        if (bulletSFXClip != null)
        {
            SFXManager.Instance?.PlaySFX(bulletSFXClip, transform.position);
        }

        float baseAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        float angleStep = maxBullets > 1 ? spreadAngle / (maxBullets - 1) : 0f;
        float startAngle = baseAngle - (spreadAngle / 2f);

        for (int i = 0; i < maxBullets; i++)
        {
            float currentAngle = startAngle + (angleStep * i);

            Vector2 bulletDir = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad)
            ).normalized;

            Quaternion bulletRotation = Quaternion.Euler(0, 0, currentAngle);

            GameObject spawnedBullet = ObjectPooler.Instance?.SpawnFromPool(bulletPrefabS, firePoint.position, bulletRotation);

            if (spawnedBullet != null)
            {
                Rigidbody2D rbBullet = spawnedBullet.GetComponent<Rigidbody2D>();
                if (rbBullet != null)
                {
                    rbBullet.linearVelocity = bulletDir * bulletForce;
                }
            }
        }
    }
}