using UnityEngine;

public class RoomCamTrigger : MonoBehaviour
{
    private BoxCollider2D roomCollider;

    void Start()
    {
        roomCollider = GetComponent<BoxCollider2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CameraController.Instance.ChangeRoom(transform.position);
        }
    }
}