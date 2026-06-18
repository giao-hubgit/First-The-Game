using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openingDir;
    private bool spawned = false;
    private RoomTemplate template;

    void Awake()
    {
        template = FindAnyObjectByType<RoomTemplate>();
    }

    void Start()
    {
        float randomDelay = 0.1f + UnityEngine.Random.Range(-0.02f, 0.02f);
        Invoke("Spawn", randomDelay);
        Destroy(gameObject, 2f);
    }

    void Spawn()
    {
        if (spawned) return;
        spawned = true;

        if (template.currentRooms >= template.maxRooms)
        {
            if (!template.IsPositionOccupied(transform.position))
            {
                Instantiate(template.closedRoom, transform.position, Quaternion.identity);
                template.TryRegisterPosition(transform.position);
            }
            return;
        }

        if (!template.TryRegisterPosition(transform.position))
        {
            return;
        }

        GameObject[] pool = null;
        switch (openingDir)
        {
            case 1: pool = template.botRoom; break;
            case 2: pool = template.topRoom; break;
            case 3: pool = template.leftRoom; break;
            case 4: pool = template.rightRoom; break;
        }

        if (pool != null && pool.Length > 0)
        {
            int rand = UnityEngine.Random.Range(0, pool.Length);
            GameObject newRoom = Instantiate(pool[rand], transform.position, pool[rand].transform.rotation);

            RoomInfo info = newRoom.AddComponent<RoomInfo>();
            info.openingDir = openingDir;

            template.rooms.Add(newRoom);
            template.currentRooms++;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SpawnPoint"))
        {
            RoomSpawner otherSpawner = collision.GetComponent<RoomSpawner>();
            if (otherSpawner != null && !spawned && !otherSpawner.spawned)
            {
                if (!template.IsPositionOccupied(transform.position))
                {
                    Instantiate(template.closedRoom, transform.position, Quaternion.identity);
                    template.TryRegisterPosition(transform.position);
                }
                Destroy(gameObject);
            }
            spawned = true;
        }
    }
}