using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class RoomTemplate : MonoBehaviour
{
    public int maxRooms;
    public int currentRooms = 0;

    public GameObject[] botRoom;
    public GameObject[] topRoom;
    public GameObject[] leftRoom;
    public GameObject[] rightRoom;
    public GameObject closedRoom;

    public List<GameObject> rooms;

    public float waitTime = 4f;
    private bool spawnedBoss;
    public GameObject boss;

    void Awake()
    {
        maxRooms = Random.Range(10, 15);
    }

    void Start()
    {
        StartCoroutine(IEspawnBoss());
    }

    IEnumerator IEspawnBoss()
    {
        yield return new WaitForSeconds(waitTime);
        Instantiate(boss, rooms[rooms.Count - 1].transform.position, Quaternion.identity);
    }
}
