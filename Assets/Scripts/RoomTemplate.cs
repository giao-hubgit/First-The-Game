using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplate : MonoBehaviour
{
    public static RoomTemplate Instance { get; private set; }

    public int maxRooms;
    public int currentRooms = 0;
    public bool bossRoomSpawned = false;

    public GameObject[] botRoom;
    public GameObject[] topRoom;
    public GameObject[] leftRoom;
    public GameObject[] rightRoom;
    public GameObject closedRoom;

    public GameObject[] botBossRooms;
    public GameObject[] topBossRooms;
    public GameObject[] leftBossRooms;
    public GameObject[] rightBossRooms;

    public List<GameObject> rooms = new List<GameObject>();
    public GameObject boss;

    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        maxRooms = Random.Range(10, 15);
    }

    void Start()
    {
        TryRegisterPosition(Vector3.zero);

        StartCoroutine(SpawnBossAtTheEnd());
    }

    public bool TryRegisterPosition(Vector3 worldPos)
    {
        Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));

        if (occupiedPositions.Contains(gridPos))
        {
            return false;
        }

        occupiedPositions.Add(gridPos);
        return true;
    }

    public bool IsPositionOccupied(Vector3 worldPos)
    {
        Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
        return occupiedPositions.Contains(gridPos);
    }

    IEnumerator SpawnBossAtTheEnd()
    {
        yield return new WaitForSeconds(0.2f);

        while (FindObjectsByType<RoomSpawner>(FindObjectsSortMode.None).Length > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (rooms.Count > 0)
        {
            int lastIndex = rooms.Count - 1;
            GameObject lastRoomObj = rooms[lastIndex];

            if (lastRoomObj != null)
            {
                Vector3 lastPos = lastRoomObj.transform.position;
                RoomInfo info = lastRoomObj.GetComponent<RoomInfo>();
                int finalDir = (info != null) ? info.openingDir : 1;

                GameObject[] bossPool = null;
                switch (finalDir)
                {
                    case 1: bossPool = botBossRooms; break;
                    case 2: bossPool = topBossRooms; break;
                    case 3: bossPool = leftBossRooms; break;
                    case 4: bossPool = rightBossRooms; break;
                }

                if (bossPool != null && bossPool.Length > 0)
                {
                    rooms.RemoveAt(lastIndex);
                    Destroy(lastRoomObj);

                    int randBoss = Random.Range(0, bossPool.Length);
                    GameObject bossRoom = Instantiate(bossPool[randBoss], lastPos, bossPool[randBoss].transform.rotation);
                    rooms.Add(bossRoom);
                    Debug.Log("Boss room generated");

                    StartCoroutine(WaitAndBakeNavmesh());
                }
            }
        }
    }

    IEnumerator WaitAndBakeNavmesh()
    {
        yield return new WaitForEndOfFrame();

        DoorWallFix[] allDoors = FindObjectsByType<DoorWallFix>(FindObjectsSortMode.None);

        foreach (DoorWallFix door in allDoors)
        {
            if (door != null)
            {
                door.CheckAndBlockDoor();
            }
        }

        yield return null;

        foreach (DoorWallFix door in allDoors)
        {
            Destroy(door.gameObject);
        }

        yield return new WaitForEndOfFrame();

        NavmeshManager navManager = FindAnyObjectByType<NavmeshManager>();
        if (navManager != null)
        {
            navManager.BakeMyMap();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy NavmeshManager trong Scene");
        }
    }
}