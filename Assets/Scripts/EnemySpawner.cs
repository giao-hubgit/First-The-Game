using System;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    private bool spawned = false;
    private EnemyTemplate template;
    private BoxCollider2D roomCollider;
    private RoomTemplate roomTemplate;
    private string spawnparticlePrefab = "EnemySpawnParticle";

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
            StartCoroutine(Spawn());
            spawned = true;
        }
    }

    IEnumerator Spawn()
    {


        int enemyToSpawn = UnityEngine.Random.Range(3, 4);
        for (int i = 0; i < enemyToSpawn; i++)
        {
            GameObject lastRoom = roomTemplate.rooms[roomTemplate.rooms.Count - 1];
            //if (template.currentEnemy < template.maxEnemy)
            //{
            if (!transform.IsChildOf(lastRoom.transform))
            {

                Vector2 randomPosition1 = GetRandomPos();
                Vector2 randomPosition2 = GetRandomPos();

                ObjectPooler.Instance?.SpawnFromPool(spawnparticlePrefab, randomPosition1, quaternion.identity);
                ObjectPooler.Instance?.SpawnFromPool(spawnparticlePrefab, randomPosition2, quaternion.identity);

                yield return new WaitForSeconds(spawnDelay);

                GameObject meleePrefab = template.Melee[UnityEngine.Random.Range(0, template.Melee.Length)];
                Instantiate(meleePrefab, randomPosition1, Quaternion.identity);

                template.currentEnemy++;

                GameObject rangedPrefab = template.Ranged[UnityEngine.Random.Range(0, template.Ranged.Length)];
                Instantiate(rangedPrefab, randomPosition2, Quaternion.identity);

                template.currentEnemy++;
            }
            //}
        }
    }

    Vector2 GetRandomPos()
    {
        Bounds bounds = roomCollider.bounds;

        float padding = 1.5f;

        float randomX = UnityEngine.Random.Range(bounds.min.x + padding, bounds.max.x - padding);
        float randomY = UnityEngine.Random.Range(bounds.min.y + padding, bounds.max.y - padding);

        return new Vector2(randomX, randomY);
    }
}
