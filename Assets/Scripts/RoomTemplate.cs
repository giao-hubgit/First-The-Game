using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class RoomTemplate : MonoBehaviour
{
    public static RoomTemplate Instance { get; private set; }

    public int maxRooms;
    public int currentRooms = 0;

    public GameObject[] botRoom;
    public GameObject[] topRoom;
    public GameObject[] leftRoom;
    public GameObject[] rightRoom;
    public GameObject closedRoom;

    public List<GameObject> rooms = new List<GameObject>();
    public GameObject boss;

    void Awake()
    {
        if (Instance == null) Instance = this;
        maxRooms = Random.Range(10, 15);
    }
}
