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
    public bool isAttacking = false;

    void Start()
    {
        UpdateWeaponUI();
    }

    public void Equip(WeaponMeleeData newWeapon)
    {
        currentWeapon = newWeapon;
        UpdateWeaponUI();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (currentWeapon == null) return;

        if (currentWeapon.isAutomatic)
        {
            if (context.started || context.performed) isAttacking = true;
            if (context.canceled) isAttacking = false;
        }
        else
        {
            isAttacking = false;
            if (context.performed) Attack();
        }
    }

    void Update()
    {
        if (isAttacking && currentWeapon != null && currentWeapon.isAutomatic)
        {
            if (Time.unscaledTime >= nextSlashTime) Attack();
        }
    }

    private void Attack()
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

        if (currentWeapon.isThrust)
        {
            StartCoroutine(PerformThrust());
        }
        else
        {
            StartCoroutine(PerformSwingArc());
        }
    }

    private IEnumerator PerformThrust()
    {
        isSwinging = true;

        GameObject pivot = new GameObject("MeleeThrustPivot");
        pivot.transform.SetParent(transform);
        pivot.transform.localPosition = Vector3.zero;
        pivot.transform.localScale = new Vector3(currentWeapon.size, currentWeapon.size, currentWeapon.size);

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Vector3 mouseScreenPos = UnityEngine.InputSystem.Mouse.current != null ?
                (Vector3)UnityEngine.InputSystem.Mouse.current.position.ReadValue() : Input.mousePosition;

            Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -mainCam.transform.position.z));
            mouseWorldPos.z = 0f;

            Vector3 dirToMouse = (mouseWorldPos - transform.position).normalized;
            if (dirToMouse.sqrMagnitude < 0.01f) dirToMouse = transform.up;

            float angleToMouse = Mathf.Atan2(dirToMouse.y, dirToMouse.x) * Mathf.Rad2Deg;
            pivot.transform.rotation = Quaternion.Euler(0, 0, angleToMouse - 90f);
        }

        GameObject weaponInstance = Instantiate(currentWeapon.weaponPrefab, pivot.transform);
        weaponInstance.transform.localRotation = Quaternion.identity;

        MeleeHitbox hitbox = weaponInstance.AddComponent<MeleeHitbox>();
        hitbox.damage = currentWeapon.damage;
        hitbox.hitImpact = currentWeapon.hitImpact;
        hitbox.hitSFX = currentWeapon.hitSFX;
        hitbox.reflectForce = currentWeapon.reflectForce;
        hitbox.hitTargetMask = currentWeapon.HitTarget;

        Collider2D col = weaponInstance.GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
        Rigidbody2D rb = weaponInstance.GetComponent<Rigidbody2D>();
        if (rb != null) rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        float duration = currentWeapon.slashSpeed;
        float elapsed = 0f;
        float reach = currentWeapon.radius;

        float facingMultiplier = Mathf.Sign(transform.lossyScale.x);

        Vector3 startPos = new Vector3(0.2f * facingMultiplier, -0.1f, 0);
        Vector3 controlPos = new Vector3(0.4f * facingMultiplier, reach * 0.4f, 0);
        Vector3 endPos = new Vector3(0, reach, 0);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            Vector3 m1 = Vector3.Lerp(startPos, controlPos, t);
            Vector3 m2 = Vector3.Lerp(controlPos, endPos, t);
            weaponInstance.transform.localPosition = Vector3.Lerp(m1, m2, t);

            Vector3 moveDir = (m2 - m1).normalized;
            if (moveDir.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;

                weaponInstance.transform.localRotation = Quaternion.Euler(0, 0, angle - 45f);
            }

            yield return null;
        }

        if (col != null) col.enabled = false;

        float holdDuration = 0.05f;
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
            float fadeDuration = 0.1f;
            float fadeElapsed = 0f;
            while (fadeElapsed < fadeDuration)
            {
                fadeElapsed += Time.unscaledDeltaTime;
                float t = fadeElapsed / fadeDuration;
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
                yield return null;
            }
        }

        Destroy(pivot);
        isSwinging = false;
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
        hitbox.hitImpact = currentWeapon.hitImpact;
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