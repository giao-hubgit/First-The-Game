using Unity.Mathematics;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openingDir;
    private int rand;
    private bool spawned = false;

    private RoomTemplate template;

    public float waitTime = 2f;

    void Awake()
    {
        template = GameObject.FindGameObjectWithTag("Room").GetComponent<RoomTemplate>();
    }

    void Start()
    {
        Destroy(gameObject, waitTime);
        float randomDelay = 0.1f + UnityEngine.Random.Range(-0.02f, 0.02f);
        Invoke("Spawn", randomDelay);
    }

    void Spawn()
    {
        if (spawned == false)
        {
            if (!spawned && template.currentRooms < template.maxRooms)
            {
                if (openingDir == 1)
                {
                    rand = UnityEngine.Random.Range(0, template.botRoom.Length);
                    Instantiate(template.botRoom[rand], transform.position, template.botRoom[rand].transform.rotation);
                }
                else if (openingDir == 2)
                {
                    rand = UnityEngine.Random.Range(0, template.topRoom.Length);
                    Instantiate(template.topRoom[rand], transform.position, template.topRoom[rand].transform.rotation);
                }
                else if (openingDir == 3)
                {
                    rand = UnityEngine.Random.Range(0, template.leftRoom.Length);
                    Instantiate(template.leftRoom[rand], transform.position, template.leftRoom[rand].transform.rotation);
                }
                else if (openingDir == 4)
                {
                    rand = UnityEngine.Random.Range(0, template.rightRoom.Length);
                    Instantiate(template.rightRoom[rand], transform.position, template.rightRoom[rand].transform.rotation);
                }

                template.currentRooms++;
                spawned = true;
            }
            else if (!spawned && template.currentRooms >= template.maxRooms)
            {
                Instantiate(template.closedRoom, transform.position, Quaternion.identity);
            }
            spawned = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SpawnPoint"))
        {
            RoomSpawner otherSpawner = collision.GetComponent<RoomSpawner>();

            if (otherSpawner != null)
            {
                if (otherSpawner.spawned == false && spawned == false)
                {
                    Instantiate(template.closedRoom, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }

                spawned = true;
            }
        }
    }
}