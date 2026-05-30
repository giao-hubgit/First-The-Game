using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerRangedWeapon : MonoBehaviour
{
    public Transform firePoint;
    [SerializeField] private GameObject weaponUIContainer;
    [SerializeField] private Image weaponIconUI;
    [SerializeField] private TextMeshProUGUI ammoTextUI;
    [SerializeField] private AudioClip outOfAmmoSFX;

    public WeaponRangedData currentWeapon;
    public WeaponRangedData nullWeapon;

    private int currentAmmo;
    private float nextFireTime = 0f;
    public bool isFiring = false;

    void Start()
    {
        if (currentWeapon != null) currentAmmo = currentWeapon.maxAmmo;
        UpdateWeaponUI();
    }

    public void Equip(WeaponRangedData newWeapon)
    {
        currentWeapon = newWeapon;
        currentAmmo = currentWeapon != null ? currentWeapon.maxAmmo : 0;
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

    void Update()
    {
        if (isFiring && currentWeapon != null && currentWeapon.isAutomatic)
        {
            if (Time.unscaledTime >= nextFireTime) Shoot();
        }
    }

    void Shoot()
    {
        if (currentWeapon == null || currentWeapon == nullWeapon || currentAmmo <= 0) return;
        if (Time.unscaledTime < nextFireTime) return;

        if (currentWeapon.recoil > 0)
        {
            PlayerMovement pm = GetComponent<PlayerMovement>();
            if (pm != null) pm.ApplyRecoil(-transform.up * currentWeapon.recoil);
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

        nextFireTime = Time.unscaledTime + currentWeapon.fireRate;
        currentAmmo--;
        UpdateWeaponUI();

        if (currentAmmo <= 0) OutOfAmmoLogic();
    }

    private void OutOfAmmoLogic()
    {
        isFiring = false;
        SFXManager.Instance?.PlaySFX(outOfAmmoSFX, transform.position);
        Equip(nullWeapon);
    }

    private void UpdateWeaponUI()
    {
        if (currentWeapon != null && currentWeapon != nullWeapon)
        {
            if (weaponUIContainer != null) weaponUIContainer.SetActive(true);
            if (weaponIconUI != null) { weaponIconUI.sprite = currentWeapon.weaponIcon; weaponIconUI.enabled = true; }
            if (ammoTextUI != null) { ammoTextUI.text = $"{currentAmmo:00}/{currentWeapon.maxAmmo:00}"; ammoTextUI.enabled = true; }
        }
        else
        {
            if (weaponUIContainer != null) weaponUIContainer.SetActive(false);
            if (weaponIconUI != null) weaponIconUI.enabled = false;
            if (ammoTextUI != null) ammoTextUI.enabled = false;
        }
    }
}