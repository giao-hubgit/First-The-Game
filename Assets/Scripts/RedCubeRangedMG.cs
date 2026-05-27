using UnityEngine;
using System.Collections;

public class RedCubeRangedMG : RedCubeRanged
{
    private bool isShootingBurst = false;

    protected override void Update()
    {
        base.Update();

        if (target != null && !isShootingBurst)
        {
            RotateTowardsPlayer();
            CheckLineOfSightAndShoot();
        }
    }

    protected override void Shoot(Vector2 shootDirection)
    {
        if (!isShootingBurst)
        {
            StartCoroutine(BurstRoutine());
        }
    }

    private IEnumerator BurstRoutine()
    {
        isShootingBurst = true;

        for (int i = 0; i < data.bulletsPerBurst; i++)
        {
            if (target == null) break;

            RotateTowardsPlayer();

            float randomSpreadOffset = Random.Range(-data.bulletSpread, data.bulletSpread);

            Quaternion spreadRotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpreadOffset);

            SpawnSingleBullet(spreadRotation);

            yield return new WaitForSeconds(data.timeBetweenBullets);
        }

        isShootingBurst = false;
    }

    private void SpawnSingleBullet(Quaternion spawnRotation)
    {
        GameObject bullet = ObjectPooler.Instance?.SpawnFromPool(data.bulletPrefabS, firePoint.position, spawnRotation);

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            rbBullet.linearVelocity = bullet.transform.up * data.bulletForce;
        }

        if (data.bulletSFXClip != null)
        {
            SFXManager.Instance?.PlaySFX(data.bulletSFXClip, transform.position, 0.3f, true, 0.75f, 1.25f);
        }
    }
}