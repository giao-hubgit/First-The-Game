using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private BoxCollider2D roomCollider;

    public GameObject roomObjects;

    void Start()
    {
        roomCollider = GetComponent<BoxCollider2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && roomCollider.bounds.Contains(player.transform.position))
        {
            if (roomObjects != null) roomObjects.SetActive(true);
        }
        else
        {
            if (roomObjects != null) roomObjects.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (roomObjects != null) roomObjects.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (roomObjects != null) roomObjects.SetActive(false);
        }
    }
}