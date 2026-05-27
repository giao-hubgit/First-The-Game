using UnityEngine;

public class RedCubeRangedShotgun : RedCubeRanged
{
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
        if (data.bulletSFXClip != null)
        {
            SFXManager.Instance?.PlaySFX(data.bulletSFXClip, transform.position, 0.3f, true, 0.75f, 1.25f);
        }

        float baseAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        float angleStep = data.maxBullets > 1 ? data.spreadAngle / (data.maxBullets - 1) : 0f;
        float startAngle = baseAngle - (data.spreadAngle / 2f);

        for (int i = 0; i < data.maxBullets; i++)
        {
            float currentAngle = startAngle + (angleStep * i);

            Vector2 bulletDir = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad)
            ).normalized;

            Quaternion bulletRotation = Quaternion.Euler(0, 0, currentAngle);

            GameObject spawnedBullet = ObjectPooler.Instance?.SpawnFromPool(data.bulletPrefabS, firePoint.position, bulletRotation);

            if (spawnedBullet != null)
            {
                Rigidbody2D rbBullet = spawnedBullet.GetComponent<Rigidbody2D>();
                if (rbBullet != null)
                {
                    rbBullet.linearVelocity = bulletDir * data.bulletForce;
                }
            }
        }
    }
}