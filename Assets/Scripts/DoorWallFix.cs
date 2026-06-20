using UnityEngine;

public class DoorWallFix : MonoBehaviour
{
    public GameObject wallBlockerPrefab;
    public LayerMask raycastHitLayer;
    public float rayDistance = 1.5f;

    public void CheckAndBlockDoor()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, rayDistance, raycastHitLayer);

        if (hit.collider != null)
        {
            Debug.Log($"[{gameObject.transform.parent.name}] Tia đập vào: {hit.collider.gameObject.name} | Tag: {hit.collider.tag}");
            if (!hit.collider.CompareTag("DoorRaycast") && !hit.collider.CompareTag("ClosedRoom"))
            {
                SpawnWallBlocker();
            }
        }
        else
        {
            SpawnWallBlocker();
        }
    }

    private void SpawnWallBlocker()
    {
        Quaternion rotationOffset = Quaternion.Euler(0, 0, 90f);

        Vector3 spawnPos = transform.position + (transform.up * 0.5f);

        Instantiate(wallBlockerPrefab, spawnPos, transform.rotation * rotationOffset);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.up * rayDistance);
    }
}