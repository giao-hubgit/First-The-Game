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

    public GameObject[] doors;

    private List<GameObject> rooms = new List<GameObject>();
    private List<GameObject> aliveEnemies = new List<GameObject>();

    [SerializeField] float spawnDelay = 2f;

    void Awake()
    {
        template = GameObject.FindGameObjectWithTag("EnemyTemplate").GetComponent<EnemyTemplate>();
        roomTemplate = GameObject.FindGameObjectWithTag("Room").GetComponent<RoomTemplate>();
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
                Vector2 randomPosition1 = GetRandomPos();
                Vector2 randomPosition2 = GetRandomPos();

                ObjectPooler.Instance?.SpawnFromPool(spawnparticlePrefab, randomPosition1, quaternion.identity);
                ObjectPooler.Instance?.SpawnFromPool(spawnparticlePrefab, randomPosition2, quaternion.identity);

                yield return new WaitForSeconds(spawnDelay);

                GameObject meleePrefab = template.Melee[UnityEngine.Random.Range(0, template.Melee.Length)];
                GameObject spawnedMelee = Instantiate(meleePrefab, randomPosition1, Quaternion.identity);
                aliveEnemies.Add(spawnedMelee);
                template.currentEnemy++;

                GameObject rangedPrefab = template.Ranged[UnityEngine.Random.Range(0, template.Ranged.Length)];
                GameObject spawnedRanged = Instantiate(rangedPrefab, randomPosition2, Quaternion.identity);
                aliveEnemies.Add(spawnedRanged);
                template.currentEnemy++;
            }
        }

        isSpawningFinished = true;
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
                door.SetActive(false);
            }
        }
    }
}
