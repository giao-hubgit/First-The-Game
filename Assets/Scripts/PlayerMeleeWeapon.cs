using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMeleeWeapon : MonoBehaviour
{
    public WeaponMeleeData currentWeapon;
    public Image weaponIconUI;

    private float nextSlashTime = 0f;
    public bool isSlashing = false;

    void Start()
    {
        UpdateWeaponUI();
    }

    public void Equip(WeaponMeleeData newWeapon)
    {
        currentWeapon = newWeapon;
        UpdateWeaponUI();
    }

    public void OnSlash(InputAction.CallbackContext context)
    {
        if (currentWeapon == null) return;

        if (currentWeapon.isAutomatic)
        {
            if (context.started || context.performed) isSlashing = true;
            if (context.canceled) isSlashing = false;
        }
        else
        {
            isSlashing = false;
            if (context.performed) Slash();
        }
    }

    void Update()
    {
        if (isSlashing && currentWeapon != null && currentWeapon.isAutomatic)
        {
            if (Time.unscaledTime >= nextSlashTime) Slash();
        }
    }

    private void Slash()
    {
        if (currentWeapon == null) return;
        if (Time.unscaledTime < nextSlashTime) return; // Cooldown chém

        if (currentWeapon.recoil > 0)
        {
            PlayerMovement pm = GetComponent<PlayerMovement>();
            if (pm != null) pm.ApplyRecoil(-transform.up * currentWeapon.recoil);
        }

        SFXManager.Instance?.PlaySFX(currentWeapon.slashSFX, transform.position, 0.3f, true, 0.75f, 1.25f);

        nextSlashTime = Time.unscaledTime + currentWeapon.slashSpeed;
    }

    private void UpdateWeaponUI()
    {
        if (currentWeapon != null)
        {
            if (weaponIconUI != null)
            {
                weaponIconUI.sprite = currentWeapon.weaponIcon; // Lấy icon chung từ WeaponData
                weaponIconUI.enabled = true;
            }
        }
        else
        {
            if (weaponIconUI != null) weaponIconUI.enabled = false;
        }
    }
}