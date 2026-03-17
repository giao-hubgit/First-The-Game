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

    void Awake()
    {
        maxRooms = Random.Range(10, 15);
    }
}
