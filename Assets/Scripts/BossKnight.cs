using UnityEngine;
using System.Collections;

public enum BossAttackType
{
    Ranged1,
    Ranged2,
    Dash,
    Laser
}

[RequireComponent(typeof(BossAI))]
[RequireComponent(typeof(Boss))]
public class KnightBoss : MonoBehaviour
{
    private BossAI locomotion;
    private Boss boss;
    private Transform player;
    private Animator animator;
    private Transform firePoint;
    [SerializeField] private LaserData laserData;
    private TrailRenderer trail;

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        firePoint = transform.GetChild(0);
        animator = GetComponent<Animator>();
        locomotion = GetComponent<BossAI>();
        boss = GetComponent<Boss>();
    }

    private void Start()
    {
        boss.isInvulnerable = true;
        trail.emitting = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(BossLogicPattern());
    }

    private IEnumerator BossLogicPattern()
    {
        while (!boss.isIntroFinished)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        while (true)
        {
            if (boss.isTransforming)
            {
                locomotion.StopMoving();
                yield return null;
                continue;
            }

            float currentMoveSpeed = boss.bossData != null ? boss.bossData.moveSpeed : 3f;
            float restCooldown = boss.bossData != null ? boss.bossData.postAttackCooldown : 1.5f;
            float preDelay = boss.bossData != null ? boss.bossData.preAttackDelay : 0.4f;

            if (boss.currentPhase == 2)
            {
                trail.emitting = true;
                currentMoveSpeed *= 1.5f;
                restCooldown *= 0.5f;
                preDelay *= 0.5f;
            }

            float minDist = boss.bossData != null ? boss.bossData.minMoveDistance : 3f;
            float maxDist = boss.bossData != null ? boss.bossData.maxMoveDistance : 6f;
            float actualDist = Random.Range(minDist, maxDist);

            Vector2 randomTarget = GetRandomPositionInRoom(actualDist);
            float timeoutTimer = 0f;
            float maxMoveTime = (actualDist / currentMoveSpeed) + 1f;
            SFXManager.Instance?.PlaySFX(boss.bossData.moveSFX, transform.position);

            while (Vector2.Distance(transform.position, randomTarget) > 0.1f && timeoutTimer < maxMoveTime && !boss.isTransforming)
            {
                locomotion.MoveTowards(randomTarget, currentMoveSpeed);
                timeoutTimer += Time.deltaTime;
                yield return null;
            }

            locomotion.StopMoving();

            if (boss.isTransforming) continue;

            yield return new WaitForSeconds(preDelay);

            int maxAttackIndex = 3;
            if (boss.currentPhase >= 2)
            {
                maxAttackIndex = 4;
            }

            BossAttackType chosenAttack = (BossAttackType)Random.Range(0, maxAttackIndex);

            switch (chosenAttack)
            {
                case BossAttackType.Ranged1:
                    yield return StartCoroutine(AttackBulletType1());
                    break;
                case BossAttackType.Ranged2:
                    yield return StartCoroutine(AttackBulletType2());
                    break;
                case BossAttackType.Dash:
                    yield return StartCoroutine(AttackDash());
                    break;
                case BossAttackType.Laser:
                    yield return StartCoroutine(AttackLaser());
                    break;
            }

            yield return new WaitForSeconds(restCooldown);
        }
    }

    private IEnumerator AttackLaser()
    {
        if (player == null) yield break;

        boss.isAttacking = true;
        locomotion.StopMoving();

        if (animator != null) animator.SetBool("isShootingLaser", true);
        SFXManager.Instance?.PlaySFX(boss.bossData.chargeSFX, transform.position);

        yield return new WaitForSeconds(1f);

        if (player == null)
        {
            boss.isAttacking = false;
            if (animator != null) animator.SetBool("isShootingLaser", false);
            yield break;
        }

        Transform launchPoint = firePoint != null ? firePoint : transform;
        Vector2 targetDir = (player.position - launchPoint.position).normalized;
        float angleToPlayer = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        float offset = laserData != null ? laserData.startAngleOffset : -60f;
        float startAngle = angleToPlayer + offset;

        Quaternion spawnRotation = Quaternion.Euler(0f, 0f, startAngle);

        string poolName = boss.bossData != null ? boss.bossData.laserPrefabS : "BossLaser";
        GameObject laserObj = ObjectPooler.Instance?.SpawnFromPool(poolName, launchPoint.position, spawnRotation);

        if (laserObj != null && firePoint != null)
        {
            laserObj.transform.SetParent(firePoint);
        }

        float sweepDuration = 2f;
        if (laserData != null)
        {
            sweepDuration = laserData.rotationAngle / laserData.rotationSpeed;
        }

        yield return new WaitForSeconds(sweepDuration);

        if (laserObj != null)
        {
            laserObj.transform.SetParent(null);
        }

        if (animator != null) animator.SetBool("isShootingLaser", false);

        boss.isAttacking = false;
    }

    private IEnumerator AttackBulletType1()
    {
        if (player == null) yield break;

        boss.isAttacking = true;

        int bulletsToSpawn = boss.bossData != null ? boss.bossData.radialBulletCount : 8;
        float speed = boss.bossData != null ? boss.bossData.bulletForce : 7f;
        string bulletName = boss.bossData != null ? boss.bossData.bulletPrefabS : "BossBullet";

        if (boss.currentPhase == 2)
        {
            speed *= 1.2f;
            bulletName = boss.bossData.bulletBossPrefabS;
        }

        float angleStep = 360f / bulletsToSpawn;
        float currentAngle = 0f;

        SFXManager.Instance?.PlaySFX(boss.bossData.bulletSFXClip, transform.position, 0.3f, true, 0.75f, 1.25f);
        animator.SetTrigger("isShooting");

        for (int i = 0; i < bulletsToSpawn; i++)
        {
            float dirX = Mathf.Sin((currentAngle * Mathf.PI) / 180f);
            float dirY = Mathf.Cos((currentAngle * Mathf.PI) / 180f);
            Vector2 bulletDir = new Vector2(dirX, dirY).normalized;

            GameObject bullet = ObjectPooler.Instance?.SpawnFromPool(bulletName, firePoint.position, Quaternion.identity);
            if (bullet != null)
            {
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = bulletDir * speed;
            }
            currentAngle += angleStep;
        }

        yield return new WaitForSeconds(0.2f);
        boss.isAttacking = false;
    }

    private IEnumerator AttackBulletType2()
    {
        if (player == null) yield break;
        boss.isAttacking = true;

        int bulletsToSpawn = boss.bossData != null ? boss.bossData.bulletsPerBurst : 3;
        float speed = boss.bossData != null ? boss.bossData.bulletForce : 7f;
        float timeBetweenBullets = boss.bossData != null ? boss.bossData.timeBetweenBullets : 0.25f;
        string bulletName = boss.bossData != null ? boss.bossData.bulletPrefabS : "BossBullet";

        if (boss.currentPhase == 2)
        {
            timeBetweenBullets *= 0.5f;
            speed *= 1.2f;
            bulletName = boss.bossData.bulletBossPrefabS;
        }

        for (int i = 0; i < bulletsToSpawn; i++)
        {
            if (player == null) yield break;
            animator.SetTrigger("isShooting");
            SFXManager.Instance?.PlaySFX(boss.bossData.bulletSFXClip, transform.position, 0.3f, true, 0.75f, 1.25f);
            Vector2 targetDir = (player.position - transform.position).normalized;
            GameObject bullet = ObjectPooler.Instance?.SpawnFromPool(bulletName, firePoint.position, Quaternion.identity);
            if (bullet != null)
            {
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = targetDir * speed;
            }
            yield return new WaitForSeconds(timeBetweenBullets);
        }

        boss.isAttacking = false;
    }

    private IEnumerator AttackDash()
    {
        if (player == null) yield break;
        boss.isAttacking = true;

        Vector2 targetDir = (player.position - transform.position).normalized;
        yield return new WaitForSeconds(0.5f);

        float currentDashSpeed = boss.bossData != null ? boss.bossData.dashSpeed : 15f;
        float dist = boss.bossData != null ? boss.bossData.dashDistance : 8f;

        if (boss.currentPhase == 2)
        {
            currentDashSpeed *= 2;
            dist *= 2;
        }

        float dashDuration = dist / currentDashSpeed;
        boss.isCrashing = true;

        if (boss.currentPhase == 2)
        {
            animator.SetBool("isDashing", true);
        }

        SFXManager.Instance?.PlaySFX(boss.currentPhase == 2 ? boss.bossData.phase2DashSFX : boss.bossData.dashSFX, transform.position, 0.3f, true, 0.75f, 1.25f);

        float timer = 0f;
        while (timer < dashDuration)
        {
            locomotion.SetVelocity(targetDir, currentDashSpeed);
            timer += Time.deltaTime;
            yield return null;
        }

        locomotion.StopMoving();
        boss.isCrashing = false;

        if (boss.currentPhase == 2)
        {
            animator.SetBool("isDashing", false);
        }

        boss.isAttacking = false;
    }

    private Vector2 GetRandomPositionInRoom(float distance)
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        return (Vector2)transform.position + randomDirection * distance;
    }
}