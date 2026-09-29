using UnityEngine;
using System.Collections.Generic;

public class ObtacleSpawner : MonoBehaviour
{
    private bool spawned = false;

    private ObstacleTemplate template;
    private BoxCollider2D roomCollider;
    private RoomTemplate roomTemplate;

    [Header("Group Spawn Settings")]
    [Tooltip("Bật để spawn vật cản theo nhóm, tắt để spawn đơn lẻ")]
    public bool spawnInGroups = true;

    [Tooltip("Khoảng cách CỐ ĐỊNH giữa các vật cản trong CÙNG 1 nhóm (không random)")]
    public float groupSpacing = 0.8f;

    [Tooltip("Khoảng cách tối thiểu GIỮA CÁC NHÓM khác nhau trong phòng")]
    public float minGroupDistance = 3.5f;

    [Header("Collision Checks")]
    [Tooltip("Chọn layer của Tường hoặc các vật cản khác (để không đè lên nhau)")]
    public LayerMask obstacleMask;
    [Tooltip("Bán kính của chướng ngại vật để quét tìm khoảng trống (0.5 - 1f)")]
    public float obstacleRadius = 0.5f;

    private List<Vector2> spawnedGroupCenters = new List<Vector2>();

    void Awake()
    {
        template = GameObject.FindGameObjectWithTag("ObstacleTemplate").GetComponent<ObstacleTemplate>();
        roomTemplate = GameObject.FindGameObjectWithTag("RoomTemplate").GetComponent<RoomTemplate>();
        roomCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && spawned == false)
        {
            spawned = true;
            Spawn();
        }
    }

    void Spawn()
    {
        GameObject lastRoom = roomTemplate.rooms[roomTemplate.rooms.Count - 1];

        if (transform.IsChildOf(lastRoom.transform)) return;

        int targetObstacleCount = UnityEngine.Random.Range(template.minObstacle, template.maxObstacle);
        int currentSpawnedCount = 0;

        spawnedGroupCenters.Clear();

        int maxGroupAttempts = 30;
        int groupAttempts = 0;

        while (currentSpawnedCount < targetObstacleCount && groupAttempts < maxGroupAttempts)
        {
            groupAttempts++;

            if (spawnInGroups)
            {
                int groupSize = UnityEngine.Random.Range(2, 6);

                Vector2 centerPos = GetValidGroupCenter();

                if (centerPos == Vector2.zero && !IsValidPosition(centerPos))
                {
                    continue;
                }

                Vector2[] offsets = new Vector2[]
                {
                    Vector2.zero,
                    new Vector2(groupSpacing, 0),
                    new Vector2(0, groupSpacing),
                    new Vector2(groupSpacing, groupSpacing),
                    new Vector2(-groupSpacing, 0)
                };

                bool hasSpawnedAny = false;

                for (int j = 0; j < groupSize && currentSpawnedCount < targetObstacleCount; j++)
                {
                    Vector2 spawnPos = centerPos + offsets[j];

                    if (IsValidPosition(spawnPos))
                    {
                        SpawnObstacleAt(spawnPos, Quaternion.identity);
                        currentSpawnedCount++;
                        hasSpawnedAny = true;
                    }
                }

                if (hasSpawnedAny)
                {
                    spawnedGroupCenters.Add(centerPos);
                }
            }
            else
            {
                Vector2 randomPosition = GetValidGroupCenter();

                if (randomPosition != Vector2.zero || IsValidPosition(randomPosition))
                {
                    SpawnObstacleAt(randomPosition, Quaternion.identity);
                    currentSpawnedCount++;
                    spawnedGroupCenters.Add(randomPosition);
                }
            }
        }
    }

    GameObject GetRandomObstaclePrefab()
    {
        if (template == null || template.obstacles == null || template.obstacles.Length == 0)
            return null;

        float totalChance = 0f;
        foreach (var item in template.obstacles)
        {
            totalChance += item.spawnChance;
        }

        if (totalChance <= 0)
            return template.obstacles[0].prefab;

        float randomValue = UnityEngine.Random.Range(0f, totalChance);
        float cumulative = 0f;

        foreach (var item in template.obstacles)
        {
            cumulative += item.spawnChance;
            if (randomValue <= cumulative)
            {
                return item.prefab;
            }
        }

        return template.obstacles[0].prefab;
    }

    void SpawnObstacleAt(Vector2 position, Quaternion rotation)
    {
        GameObject obstaclePrefab = GetRandomObstaclePrefab();
        if (obstaclePrefab == null) return;

        GameObject spawnedObstacle = Instantiate(obstaclePrefab, position, rotation);
        spawnedObstacle.transform.SetParent(this.transform);
        template.currentObstacle++;
    }

    bool IsValidPosition(Vector2 pos)
    {
        Bounds bounds = roomCollider.bounds;
        float padding = 0.5f;
        if (pos.x < bounds.min.x + padding || pos.x > bounds.max.x - padding ||
            pos.y < bounds.min.y + padding || pos.y > bounds.max.y - padding)
        {
            return false;
        }

        Collider2D hit = Physics2D.OverlapCircle(pos, obstacleRadius, obstacleMask);
        return (hit == null || hit.isTrigger);
    }

    Vector2 GetValidGroupCenter()
    {
        Vector2 spawnPos = Vector2.zero;
        bool isValid = false;
        int attempts = 0;
        int maxAttempts = 50;

        while (!isValid && attempts < maxAttempts)
        {
            spawnPos = GetRandomPos();

            if (IsValidPosition(spawnPos))
            {
                bool isFarEnoughFromOtherGroups = true;
                foreach (Vector2 center in spawnedGroupCenters)
                {
                    if (Vector2.Distance(spawnPos, center) < minGroupDistance)
                    {
                        isFarEnoughFromOtherGroups = false;
                        break;
                    }
                }

                if (isFarEnoughFromOtherGroups)
                {
                    isValid = true;
                }
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
}