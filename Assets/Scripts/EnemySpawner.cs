using System.ComponentModel;
using System;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    private bool spawned = false;
    private bool roomCleared = false;
    private bool isSpawningFinished = false;

    private EnemyTemplate template;
    private BoxCollider2D roomCollider;
    private RoomTemplate roomTemplate;

    private string spawnparticlePrefab = "EnemySpawnParticle";
    private string spawnparticlePrefab1 = "EnemySpawnParticle2";
    [SerializeField] AudioClip spawnSFX;
    [SerializeField] AudioClip doorSFX;

    public GameObject[] doors;

    private List<GameObject> rooms = new List<GameObject>();
    private List<GameObject> aliveEnemies = new List<GameObject>();

    [SerializeField] float spawnDelay = 2f;

    [Header("Collision Checks")]
    [Tooltip("Chọn layer của Tường và Chướng ngại vật")]
    public LayerMask obstacleMask;
    [Tooltip("Độ lớn của quái để quét tìm khoảng trống (0.5 - 1f)")]
    public float enemyRadius = 0.5f;

    void Awake()
    {
        template = GameObject.FindGameObjectWithTag("EnemyTemplate").GetComponent<EnemyTemplate>();
        roomTemplate = GameObject.FindGameObjectWithTag("RoomTemplate").GetComponent<RoomTemplate>();
        roomCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && spawned == false)
        {
            spawned = true;
            closeDoors();
            StartCoroutine(Spawn());
        }
    }

    void Update()
    {
        if (isSpawningFinished == true && roomCleared == false)
        {
            CheckEnemiesStatus();
        }
    }

    IEnumerator Spawn()
    {
        if (RoomTemplate.Instance.rooms.Count > 0)
        {
            GameObject lastRoom = RoomTemplate.Instance.rooms[RoomTemplate.Instance.rooms.Count - 1];

            if (transform.IsChildOf(lastRoom.transform))
            {
                yield return new WaitForSeconds(1f);

                GameObject bossPrefab = RoomTemplate.Instance.boss;

                Vector2 spawnPos = bossPrefab.transform.position;
                spawnPos.y += 1f * bossPrefab.transform.localScale.y;

                GameObject p1 = ObjectPooler.Instance?.SpawnFromPool(spawnparticlePrefab, spawnPos, quaternion.identity);
                if (p1 != null)
                {
                    p1.transform.localScale = Vector3.Scale(p1.transform.localScale, bossPrefab.transform.localScale);
                }

                yield return new WaitForSeconds(spawnDelay);

                SFXManager.Instance?.PlaySFX(spawnSFX, spawnPos);
                SFXManager.Instance?.PlaySFX(spawnSFX, spawnPos);

                GameObject laser1 = ObjectPooler.Instance?.SpawnFromPool(spawnparticlePrefab1, spawnPos, quaternion.identity);
                if (laser1 != null)
                {
                    laser1.transform.localScale = Vector3.Scale(laser1.transform.localScale, bossPrefab.transform.localScale);
                }

                GameObject spawnedBoss = Instantiate(bossPrefab, transform.position, Quaternion.identity);

                aliveEnemies.Add(spawnedBoss);

                isSpawningFinished = true;

                yield break;
            }
        }

        int enemyToSpawn = UnityEngine.Random.Range(3, 4);

        for (int i = 0; i < enemyToSpawn; i++)
        {
            GameObject lastRoom = roomTemplate.rooms[roomTemplate.rooms.Count - 1];

            if (!transform.IsChildOf(lastRoom.transform))
            {
                Vector2 randomPosition1 = GetValidSpawnPosition();
                Vector2 randomPosition2 = GetValidSpawnPosition();

                GameObject meleePrefab = template.Melee[UnityEngine.Random.Range(0, template.Melee.Length)];
                GameObject rangedPrefab = template.Ranged[UnityEngine.Random.Range(0, template.Ranged.Length)];

                Vector2 randomPos1SpawnLaser = randomPosition1;
                randomPos1SpawnLaser.y += 1f * meleePrefab.transform.localScale.y;
                Vector2 randomPos2SpawnLaser = randomPosition2;
                randomPos2SpawnLaser.y += 1f * rangedPrefab.transform.localScale.y;

                GameObject p1 = ObjectPooler.Instance?.SpawnFromPool(spawnparticlePrefab, randomPosition1, quaternion.identity);
                if (p1 != null)
                {
                    p1.transform.localScale = Vector3.Scale(p1.transform.localScale, meleePrefab.transform.localScale);
                }

                GameObject p2 = ObjectPooler.Instance?.SpawnFromPool(spawnparticlePrefab, randomPosition2, quaternion.identity);
                if (p2 != null)
                {
                    p2.transform.localScale = Vector3.Scale(p2.transform.localScale, rangedPrefab.transform.localScale);
                }

                yield return new WaitForSeconds(spawnDelay);

                SFXManager.Instance?.PlaySFX(spawnSFX, randomPosition1);
                SFXManager.Instance?.PlaySFX(spawnSFX, randomPosition2);

                GameObject laser1 = ObjectPooler.Instance?.SpawnFromPool(spawnparticlePrefab1, randomPos1SpawnLaser, quaternion.identity);
                if (laser1 != null)
                {
                    laser1.transform.localScale = Vector3.Scale(laser1.transform.localScale, meleePrefab.transform.localScale);
                }

                GameObject laser2 = ObjectPooler.Instance?.SpawnFromPool(spawnparticlePrefab1, randomPos2SpawnLaser, quaternion.identity);
                if (laser2 != null)
                {
                    laser2.transform.localScale = Vector3.Scale(laser2.transform.localScale, rangedPrefab.transform.localScale);
                }

                GameObject spawnedMelee = Instantiate(meleePrefab, randomPosition1, Quaternion.identity);
                aliveEnemies.Add(spawnedMelee);
                template.currentEnemy++;

                GameObject spawnedRanged = Instantiate(rangedPrefab, randomPosition2, Quaternion.identity);
                aliveEnemies.Add(spawnedRanged);
                template.currentEnemy++;
            }
        }

        isSpawningFinished = true;
    }

    Vector2 GetValidSpawnPosition()
    {
        Vector2 spawnPos = Vector2.zero;
        bool isValid = false;
        int attempts = 0;
        int maxAttempts = 50;

        while (!isValid && attempts < maxAttempts)
        {
            spawnPos = GetRandomPos();

            Collider2D hit = Physics2D.OverlapCircle(spawnPos, enemyRadius, obstacleMask);

            if (hit == null || hit.isTrigger)
            {
                isValid = true;
            }

            attempts++;
        }

        return spawnPos;
    }

    Vector2 GetRandomPos()
    {
        Bounds bounds = roomCollider.bounds;
        float padding = 1.5f;

        float randomX = UnityEngine.Random.Range(bounds.min.x + padding, bounds.max.x - padding);
        float randomY = UnityEngine.Random.Range(bounds.min.y + padding, bounds.max.y - padding);

        return new Vector2(randomX, randomY);
    }

    void CheckEnemiesStatus()
    {
        aliveEnemies.RemoveAll(item => item == null);

        if (aliveEnemies.Count == 0)
        {
            roomCleared = true;
            openDoors();
        }
    }

    void closeDoors()
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
            {
                SFXManager.Instance?.PlaySFX(doorSFX, door.transform.position);
                door.SetActive(true);
            }
        }
    }

    void openDoors()
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
            {
                SFXManager.Instance?.PlaySFX(doorSFX, door.transform.position);
                door.SetActive(false);
            }
        }
    }
}