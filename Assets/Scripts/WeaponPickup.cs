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
            PlayerWeapon pw = other.GetComponentInChildren<PlayerWeapon>();
            if (pw != null)
            {
                pw.ChangeWeapon(weaponToGive);

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
