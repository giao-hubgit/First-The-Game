using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    private List<WeaponPickup> nearbyWeapons = new List<WeaponPickup>();

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && nearbyWeapons.Count > 0)
        {
            WeaponPickup weaponToPick = nearbyWeapons[nearbyWeapons.Count - 1];

            RemoveInteractableWeapon(weaponToPick);

            weaponToPick.Interact(gameObject);
        }
    }

    public void AddInteractableWeapon(WeaponPickup weapon)
    {
        if (!nearbyWeapons.Contains(weapon))
        {
            nearbyWeapons.Add(weapon);
        }
        UpdateOutlines();
    }

    public void RemoveInteractableWeapon(WeaponPickup weapon)
    {
        if (nearbyWeapons.Contains(weapon))
        {
            weapon.ToggleOutline(false);
            nearbyWeapons.Remove(weapon);
        }
        UpdateOutlines();
    }

    private void UpdateOutlines()
    {
        foreach (WeaponPickup w in nearbyWeapons)
        {
            if (w != null) w.ToggleOutline(false);
        }

        if (nearbyWeapons.Count > 0)
        {
            WeaponPickup topWeapon = nearbyWeapons[nearbyWeapons.Count - 1];
            if (topWeapon != null)
            {
                topWeapon.ToggleOutline(true);
            }
        }
    }
}