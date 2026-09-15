using UnityEngine;
using System.Collections;

public class EnemyRangedWeapon : MonoBehaviour
{
    public WeaponRangedData currentWeapon;
    public Transform firePoint;

    private float nextFireTime = 0f;
    private bool isShootingBurst = false;
    private IAttacker ownerAttacker;

    private void Awake()
    {
        ownerAttacker = GetComponentInParent<IAttacker>();
    }

    public bool TryShoot()
    {
        if (currentWeapon == null || isShootingBurst || Time.time < nextFireTime)
            return false;

        nextFireTime = Time.time + currentWeapon.fireRate;

        if (currentWeapon.timeBetweenBullets > 0f)
        {
            StartCoroutine(BurstRoutine());
        }
        else
        {
            FireInstant();
        }

        return true;
    }

    private void FireInstant()
    {
        for (int i = 0; i < currentWeapon.burstCount; i++)
        {
            SpawnSingleBullet();
        }
    }

    private IEnumerator BurstRoutine()
    {
        isShootingBurst = true;

        for (int i = 0; i < currentWeapon.burstCount; i++)
        {
            SpawnSingleBullet();

            if (i < currentWeapon.burstCount - 1)
            {
                yield return new WaitForSeconds(currentWeapon.timeBetweenBullets);
            }
        }

        isShootingBurst = false;
    }

    private void SpawnSingleBullet()
    {
        float randomSpread = Random.Range(-currentWeapon.spread, currentWeapon.spread);
        Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpread);

        GameObject bullet = ObjectPooler.Instance?.SpawnFromPool(
            currentWeapon.bulletTag,
            firePoint.position,
            bulletRotation
        );

        if (bullet != null)
        {
            if (bullet.TryGetComponent<Bullet>(out Bullet bulletScript))
            {
                bulletScript.Init(ownerAttacker);
            }

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = bullet.transform.up * currentWeapon.bulletForce;
            }
        }

        if (currentWeapon.shootSFX != null)
        {
            SFXManager.Instance?.PlaySFX(currentWeapon.shootSFX, transform.position);
        }
    }
}