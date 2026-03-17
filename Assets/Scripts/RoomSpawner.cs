using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int openingDir;
    private int rand;
    private bool spawned = false;

    private RoomTemplate template;

    void Start()
    {
        template = GameObject.FindGameObjectWithTag("Room").GetComponent<RoomTemplate>();
        Invoke("Spawn", 0.1f);
    }

    void Spawn()
    {
        if (spawned == false)
        {
            if (openingDir == 1)
            {
                rand = Random.Range(0, template.botRoom.Length);
                Instantiate(template.botRoom[rand], transform.position, template.botRoom[rand].transform.rotation);
            }
            else if (openingDir == 2)
            {
                rand = Random.Range(0, template.topRoom.Length);
                Instantiate(template.topRoom[rand], transform.position, template.topRoom[rand].transform.rotation);
            }
            else if (openingDir == 3)
            {
                rand = Random.Range(0, template.leftRoom.Length);
                Instantiate(template.leftRoom[rand], transform.position, template.leftRoom[rand].transform.rotation);
            }
            else if (openingDir == 4)
            {
                rand = Random.Range(0, template.rightRoom.Length);
                Instantiate(template.rightRoom[rand], transform.position, template.rightRoom[rand].transform.rotation);
            }
        }

        spawned = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SpawnPoint") && collision.GetComponent<RoomSpawner>().spawned == true)
        {
            Destroy(gameObject);
        }
    }
}
