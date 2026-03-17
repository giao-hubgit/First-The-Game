using UnityEngine;

public class AddRooms : MonoBehaviour
{
    private RoomTemplate template;

    void Awake()
    {
        template = GameObject.FindGameObjectWithTag("Room").GetComponent<RoomTemplate>();
        template.rooms.Add(this.gameObject);
    }
}
