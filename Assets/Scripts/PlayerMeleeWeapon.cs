using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Splines;

public class PlayerMeleeWeapon : MonoBehaviour
{
    public WeaponMeleeData currentWeapon;
    public Image weaponIconUI;

    private float nextSlashTime = 0f;
    public bool isHoldingAttack = false;
    private bool isSwinging = false;
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

        if (Time.unscaledTime < nextSlashTime || isSwinging) return;

        if (currentWeapon.recoil > 0)
        {
            PlayerMovement pm = GetComponent<PlayerMovement>();
            if (pm != null) pm.ApplyRecoil(-transform.up * currentWeapon.recoil);
        }

        SFXManager.Instance?.PlaySFX(currentWeapon.slashSFX, transform.position, 0.3f, true, 0.75f, 1.25f);

        nextSlashTime = Time.unscaledTime + currentWeapon.cooldown;

        StartCoroutine(PerformSwingArc());
    }

    private IEnumerator PerformSwingArc()
    {
        isSwinging = true;

        GameObject pivot = new GameObject("MeleeSwingPivot");
        pivot.transform.SetParent(transform);
        pivot.transform.localPosition = Vector3.zero;
        pivot.transform.localScale = new Vector3(currentWeapon.size, currentWeapon.size, currentWeapon.size);

        GameObject weaponInstance = Instantiate(currentWeapon.weaponPrefab, pivot.transform);
        weaponInstance.transform.localPosition = new Vector3(0, currentWeapon.radius, 0);
        weaponInstance.transform.localRotation = Quaternion.Euler(0, 0, 45f);

        MeleeHitbox hitbox = weaponInstance.AddComponent<MeleeHitbox>();
        hitbox.damage = currentWeapon.damage;
        hitbox.hitSFX = currentWeapon.hitSFX;
        hitbox.reflectForce = currentWeapon.reflectForce;
        hitbox.hitTargetMask = currentWeapon.HitTarget;

        Collider2D col = weaponInstance.GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        float duration = currentWeapon.slashSpeed;
        float elapsed = 0f;
        float startAngle = -90f;
        float endAngle = 90f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            pivot.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(startAngle, endAngle, t));
            yield return null;
        }

        if (col != null) col.enabled = false;

        float holdDuration = 0.1f;
        float holdElapsed = 0f;

        while (holdElapsed < holdDuration)
        {
            holdElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        SpriteRenderer spriteRenderer = weaponInstance.GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            float fadeDuration = 0.15f;
            float fadeElapsed = 0f;

            while (fadeElapsed < fadeDuration)
            {
                fadeElapsed += Time.unscaledDeltaTime;
                float t = fadeElapsed / fadeDuration;

                float alpha = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                yield return null;
            }
        }

        Destroy(pivot);
        isSwinging = false;
    }

    private void UpdateWeaponUI()
    {
        if (currentWeapon != null && weaponIconUI != null)
        {
            weaponIconUI.sprite = currentWeapon.weaponIcon;
            weaponIconUI.enabled = true;
        }
        else if (weaponIconUI != null)
        {
            weaponIconUI.enabled = false;
        }
    }
}