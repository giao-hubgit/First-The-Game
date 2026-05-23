using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponToGive;
    public GameObject floatingTextPrefab;
    public AudioClip chestSFX;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShooting playerShooting = other.GetComponentInChildren<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.ChangeWeapon(weaponToGive);

                SFXManager.Instance?.PlaySFX(chestSFX, transform.position);

                SpawnFloatingText();

                Destroy(gameObject);
            }
        }
    }

    void SpawnFloatingText()
    {
        if (floatingTextPrefab != null)
        {

            GameObject popup = Instantiate(floatingTextPrefab, transform.position + Vector3.up, Quaternion.identity);

            FloatingText ftScript = popup.GetComponent<FloatingText>();
            ftScript.SetText(weaponToGive.weaponName, Color.white);
        }
    }
}
