using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private bool spawned = false;
    private EnemyTemplate template;
    private BoxCollider2D roomCollider;
    private RoomTemplate roomTemplate;

    void Awake()
    {
        if (roomCollider == null) print("roomCollider == null");
        if (template == null) print("template == null");
        template = GameObject.FindGameObjectWithTag("EnemyTemplate").GetComponent<EnemyTemplate>();
        roomTemplate = GameObject.FindGameObjectWithTag("Room").GetComponent<RoomTemplate>();
        roomCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && spawned == false)
        {
            Spawn();
            spawned = true;
        }
    }

    void Spawn()
    {
        int enemyToSpawn = Random.Range(3, 4);
        for (int i = 0; i < enemyToSpawn; i++)
        {
            GameObject lastRoom = roomTemplate.rooms[roomTemplate.rooms.Count - 1];
            //if (template.currentEnemy < template.maxEnemy)
            //{
            if (transform.IsChildOf(lastRoom.transform)) return;

            Vector2 randomPosition = GetRandomPos();

            GameObject meleePrefab = template.Melee[Random.Range(0, template.Melee.Length)];
            Instantiate(meleePrefab, randomPosition, Quaternion.identity);

            template.currentEnemy++;

            GameObject rangedPrefab = template.Ranged[Random.Range(0, template.Ranged.Length)];
            Instantiate(rangedPrefab, randomPosition, Quaternion.identity);

            template.currentEnemy++;
            //}
        }
    }

    Vector2 GetRandomPos()
    {
        Bounds bounds = roomCollider.bounds;

        float padding = 1.5f;

        float randomX = Random.Range(bounds.min.x + padding, bounds.max.x - padding);
        float randomY = Random.Range(bounds.min.y + padding, bounds.max.y - padding);

        return new Vector2(randomX, randomY);
    }
}
