using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;

public class ObtacleSpawner : MonoBehaviour
{
    private bool spawned = false;

    private ObstacleTemplate template;
    private BoxCollider2D roomCollider;
    private RoomTemplate roomTemplate;

    private List<GameObject> rooms = new List<GameObject>();

    [Header("Collision Checks")]
    [Tooltip("Chọn layer của Tường hoặc các vật cản khác (để không đè lên nhau)")]
    public LayerMask obstacleMask;
    [Tooltip("Bán kính của chướng ngại vật để quét tìm khoảng trống (0.5 - 1f)")]
    public float obstacleRadius = 0.5f;

    void Awake()
    {
        template = GameObject.FindGameObjectWithTag("ObstacleTemplate").GetComponent<ObstacleTemplate>();
        roomTemplate = GameObject.FindGameObjectWithTag("Room").GetComponent<RoomTemplate>();
        roomCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        /*if (gameObject.CompareTag("SpecialRoom") || (transform.parent != null && transform.parent.CompareTag("SpecialRoom")))
        {
            return;
        }*/

        if (other.gameObject.CompareTag("Player") && spawned == false)
        {
            spawned = true;
            Spawn();
        }
    }

    void Spawn()
    {
        int obstacleToSpawn = UnityEngine.Random.Range(10, 14);

        for (int i = 0; i < obstacleToSpawn; i++)
        {
            GameObject lastRoom = roomTemplate.rooms[roomTemplate.rooms.Count - 1];

            if (!transform.IsChildOf(lastRoom.transform))
            {
                Vector2 randomPosition = GetValidSpawnPosition();
                Quaternion randomRotation = GetRandomRot();

                GameObject obstaclePrefab = template.Obstacles[UnityEngine.Random.Range(0, template.Obstacles.Length)];
                GameObject spawnedObstacle = Instantiate(obstaclePrefab, randomPosition, randomRotation);

                spawnedObstacle.transform.SetParent(this.transform);

                template.currentObstacle++;
            }
        }
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

            Collider2D hit = Physics2D.OverlapCircle(spawnPos, obstacleRadius, obstacleMask);

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

    Quaternion GetRandomRot()
    {
        float randomAngle = UnityEngine.Random.Range(0f, 360f);

        Quaternion randomRotation = Quaternion.Euler(0f, 0f, randomAngle);

        return randomRotation;
    }
}