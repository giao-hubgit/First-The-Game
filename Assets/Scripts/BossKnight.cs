using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BossAI))]
[RequireComponent(typeof(Boss))]
public class KnightBoss : MonoBehaviour
{
    private BossAI locomotion;
    private Boss boss;
    private Transform player;

    private void Awake()
    {
        locomotion = GetComponent<BossAI>();
        boss = GetComponent<Boss>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(BossLogicPattern());
    }

    private IEnumerator BossLogicPattern()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            float currentMoveSpeed = boss.bossData != null ? boss.bossData.moveSpeed : 3f;
            float restCooldown = boss.bossData != null ? boss.bossData.postAttackCooldown : 1.5f;
            float preDelay = boss.bossData != null ? boss.bossData.preAttackDelay : 0.4f;

            if (boss.isPhase2)
            {
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

            while (Vector2.Distance(transform.position, randomTarget) > 0.1f && timeoutTimer < maxMoveTime)
            {
                locomotion.MoveTowards(randomTarget, currentMoveSpeed);
                timeoutTimer += Time.deltaTime;
                yield return null;
            }

            locomotion.StopMoving();
            yield return new WaitForSeconds(preDelay);

            int randomAttack = Random.Range(0, 4);
            switch (randomAttack)
            {
                //case 0: yield return StartCoroutine(AttackLaser()); break;
                case 1: yield return StartCoroutine(AttackBulletType1()); break;
                case 2: yield return StartCoroutine(AttackBulletType2()); break;
                case 3: yield return StartCoroutine(AttackDash()); break;
            }

            yield return new WaitForSeconds(restCooldown);
        }
    }

    private IEnumerator AttackLaser()
    {
        int dmg = boss.bossData != null ? boss.bossData.laserDamage : 20;
        Debug.Log($"Lazer!");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator AttackBulletType1()
    {
        Debug.Log("đạn tỏa");
        int bulletsToSpawn = boss.bossData != null ? boss.bossData.radialBulletCount : 8;
        float speed = boss.bossData != null ? boss.bossData.bulletForce : 7f;
        string bulletName = boss.bossData != null ? boss.bossData.bulletPrefabS : "BossBullet";

        float angleStep = 360f / bulletsToSpawn;
        float currentAngle = 0f;

        for (int i = 0; i < bulletsToSpawn; i++)
        {
            float dirX = Mathf.Sin((currentAngle * Mathf.PI) / 180f);
            float dirY = Mathf.Cos((currentAngle * Mathf.PI) / 180f);
            Vector2 bulletDir = new Vector2(dirX, dirY).normalized;

            GameObject bullet = ObjectPooler.Instance?.SpawnFromPool(bulletName, transform.position, Quaternion.identity);
            if (bullet != null)
            {
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = bulletDir * speed;
            }
            currentAngle += angleStep;
        }
        yield return null;
    }

    private IEnumerator AttackBulletType2()
    {
        Debug.Log("bùm bùm bùm");
        int bulletsToSpawn = boss.bossData != null ? boss.bossData.bulletsPerBurst : 3;
        float speed = boss.bossData != null ? boss.bossData.bulletForce : 7f;
        string bulletName = boss.bossData != null ? boss.bossData.bulletPrefabS : "BossBullet";

        for (int i = 0; i < bulletsToSpawn; i++)
        {
            Vector2 targetDir = (player.position - transform.position).normalized;
            GameObject bullet = ObjectPooler.Instance?.SpawnFromPool(bulletName, transform.position, Quaternion.identity);
            if (bullet != null)
            {
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = targetDir * speed;
            }
            yield return new WaitForSeconds(boss.bossData.timeBetweenBullets);
        }
    }

    private IEnumerator AttackDash()
    {
        Debug.Log("dash");
        Vector2 targetDir = (player.position - transform.position).normalized;
        yield return new WaitForSeconds(0.5f);

        float currentDashSpeed = boss.bossData != null ? boss.bossData.dashSpeed : 15f;
        float dist = boss.bossData != null ? boss.bossData.dashDistance : 8f;

        float dashDuration = dist / currentDashSpeed;

        boss.isCrashing = true;

        float timer = 0f;
        while (timer < dashDuration)
        {
            locomotion.SetVelocity(targetDir, currentDashSpeed);
            timer += Time.deltaTime;
            yield return null;
        }

        locomotion.StopMoving();
        boss.isCrashing = false;
    }

    private Vector2 GetRandomPositionInRoom(float distance)
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        return (Vector2)transform.position + randomDirection * distance;
    }
}