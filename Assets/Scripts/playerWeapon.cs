using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // ⭐ Bắt buộc thêm dòng này để dùng List

public class PlayerWeapon : MonoBehaviour
{
    public Transform firePoint;
    [SerializeField] private GameObject weaponUIContainer;
    [SerializeField] private Image weaponIconUI;
    [SerializeField] private TextMeshProUGUI ammoTextUI;

    public WeaponData currentWeapon;
    public WeaponData nullWeapon;

    private List<WeaponPickup> nearbyWeapons = new List<WeaponPickup>();
    private int currentAmmo;

    private float nextFireTime = 0f;
    public bool isFiring = false;

    void Start()
    {
        currentAmmo = currentWeapon.maxAmmo;
        UpdateWeaponUI();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (currentWeapon == null || currentWeapon == nullWeapon) return;

        if (currentWeapon.isAutomatic)
        {
            if (context.started || context.performed) isFiring = true;
            if (context.canceled) isFiring = false;
        }
        else
        {
            isFiring = false;
            if (context.performed) Shoot();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && nearbyWeapons.Count > 0)
        {
            WeaponPickup weaponToPick = nearbyWeapons[nearbyWeapons.Count - 1];

            RemoveInteractableWeapon(weaponToPick);

            weaponToPick.Interact(this);
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

    void Shoot()
    {
        if (currentWeapon == null) return;
        if (currentAmmo <= 0) return;
        if (!currentWeapon.isAutomatic && Time.time < nextFireTime) return;

        if (currentWeapon.recoil > 0)
        {
            PlayerMovement pm = GetComponent<PlayerMovement>();
            if (pm != null)
            {
                Vector2 recoilForce = -transform.up * currentWeapon.recoil;
                pm.ApplyRecoil(recoilForce);
            }
        }

        for (int i = 0; i < currentWeapon.burstCount; i++)
        {
            float randomSpread = Random.Range(-currentWeapon.spread, currentWeapon.spread);
            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpread);

            GameObject bullet = ObjectPooler.Instance.SpawnFromPool(
                currentWeapon.bulletTag,
                firePoint.position,
                bulletRotation
            );

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = bullet.transform.up * currentWeapon.bulletForce;
        }

        SFXManager.Instance?.PlaySFX(currentWeapon.shootSFX, transform.position);

        nextFireTime = Time.time + currentWeapon.fireRate;
        currentAmmo--;
        UpdateWeaponUI();

        if (currentAmmo <= 0)
        {
            OutOfAmmoLogic();
        }
    }

    private void OutOfAmmoLogic()
    {
        isFiring = false;
        ChangeWeapon(nullWeapon);
        UpdateWeaponUI();
    }

    void Update()
    {
        if (isFiring && currentWeapon != null && currentWeapon.isAutomatic)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
            }
        }
    }

    public void ChangeWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        currentAmmo = currentWeapon.maxAmmo;
        UpdateWeaponUI();
    }

    private void UpdateWeaponUI()
    {
        if (currentWeapon != null && currentWeapon != nullWeapon)
        {
            if (weaponUIContainer != null) weaponUIContainer.SetActive(true);

            if (weaponIconUI != null)
            {
                weaponIconUI.sprite = currentWeapon.weaponSprite;
                weaponIconUI.enabled = true;
            }

            if (ammoTextUI != null)
            {
                ammoTextUI.text = $"{currentAmmo:00}/{currentWeapon.maxAmmo:00}";
                ammoTextUI.enabled = true;
            }
        }
        else
        {
            if (weaponUIContainer != null) weaponUIContainer.SetActive(false);
            if (weaponIconUI != null) weaponIconUI.enabled = false;
            if (ammoTextUI != null) ammoTextUI.enabled = false;
        }
    }
}