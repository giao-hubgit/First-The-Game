using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObtacleSpawner : MonoBehaviour
{
    private bool spawned = false;

    private ObstacleTemplate template;
    private BoxCollider2D roomCollider;
    private RoomTemplate roomTemplate;


    private List<GameObject> rooms = new List<GameObject>();

    void Awake()
    {
        template = GameObject.FindGameObjectWithTag("ObstacleTemplate").GetComponent<ObstacleTemplate>();
        roomTemplate = GameObject.FindGameObjectWithTag("Room").GetComponent<RoomTemplate>();
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
        int obstacleToSpawn = UnityEngine.Random.Range(10, 14);

        for (int i = 0; i < obstacleToSpawn; i++)
        {
            GameObject lastRoom = roomTemplate.rooms[roomTemplate.rooms.Count - 1];

            if (!transform.IsChildOf(lastRoom.transform))
            {
                Vector2 randomPosition = GetRandomPos();

                GameObject obstaclePrefab = template.Obstacles[UnityEngine.Random.Range(0, template.Obstacles.Length)];
                GameObject spawnedObstacle = Instantiate(obstaclePrefab, randomPosition, Quaternion.identity);
                template.currentObstacle++;
            }
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
