using System.Collections;
using UnityEngine;

public class RoomDestroyer : MonoBehaviour
{
    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        Destroy(other.gameObject);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
