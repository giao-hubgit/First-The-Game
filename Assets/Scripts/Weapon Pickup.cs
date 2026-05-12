using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponToGive;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShooting playerShooting = other.GetComponentInChildren<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.ChangeWeapon(weaponToGive);
                Destroy(gameObject);
            }
        }
    }
}
