using UnityEngine;

public class AddRooms : MonoBehaviour
{
    private RoomTemplate template;

    void Awake()
    {
        template = GameObject.FindGameObjectWithTag("RoomTemplate").GetComponent<RoomTemplate>();
        template.rooms.Add(this.gameObject);
    }
}
