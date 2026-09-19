using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerMeleeWeapon : MonoBehaviour
{
    public WeaponMeleeData currentWeapon;
    public Image weaponIconUI;
    [SerializeField] Image meleeCooldown;

    private float nextSlashTime = 0f;
    public bool isHoldingAttack = false;
    private bool isSwinging = false;
    public bool isAttacking = false;

    public RectTransform hudContainerTransform;
    private Coroutine refreshCoroutine;

    private float WeaponDeltaTime => Time.timeScale > 0f ? Time.unscaledDeltaTime : 0f;

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

        nextSlashTime = Time.unscaledTime + currentWeapon.cooldown;

        if (meleeCooldown != null) meleeCooldown.fillAmount = 0f;

        StartCoroutine(meleeCooldownIE());

        if (currentWeapon.isThrust)
        {
            StartCoroutine(PerformThrust());
        }
        else
        {
            StartCoroutine(PerformSwingArc());
        }

        if (currentWeapon.delayStart >= 0.5f)
        {
            SFXManager.Instance?.PlaySFX(currentWeapon.appearSFX, transform.position, 0.15f, true, 1.5f, 2f);
        }
    }

    private IEnumerator meleeCooldownIE()
    {
        float timer = 0f;
        while (timer < currentWeapon.cooldown)
        {
            timer += WeaponDeltaTime;
            if (meleeCooldown != null)
            {
                meleeCooldown.fillAmount = timer / currentWeapon.cooldown;
            }
            yield return null;
        }
        if (meleeCooldown != null) meleeCooldown.fillAmount = 1f;
    }

    private IEnumerator PerformThrust()
    {
        isSwinging = true;

        GameObject pivot = new GameObject("MeleeThrustPivot");
        pivot.transform.SetParent(transform);

        pivot.transform.localPosition = new Vector3(currentWeapon.spawnOffset.x, currentWeapon.spawnOffset.y, 0f);
        pivot.transform.localScale = new Vector3(currentWeapon.size, currentWeapon.size, currentWeapon.size);
        pivot.transform.localRotation = Quaternion.Euler(0, 0, 0);

        GameObject weaponInstance = Instantiate(currentWeapon.weaponPrefab, pivot.transform);

        TrailRenderer[] components = weaponInstance.GetComponentsInChildren<TrailRenderer>(true);
        List<GameObject> childObjects = new List<GameObject>();

        foreach (TrailRenderer comp in components)
        {
            childObjects.Add(comp.gameObject);
        }

        float reach = currentWeapon.radius;
        Vector3 startPos = new Vector3(0.2f, -0.1f, 0);
        Vector3 controlPos = new Vector3(0.4f, reach * 0.4f, 0);
        Vector3 endPos = new Vector3(0, reach, 0);

        weaponInstance.transform.localPosition = startPos;
        Vector3 initialMoveDir = (controlPos - startPos).normalized;
        if (initialMoveDir.sqrMagnitude > 0.001f)
        {
            float initialAngle = Mathf.Atan2(initialMoveDir.y, initialMoveDir.x) * Mathf.Rad2Deg;
            weaponInstance.transform.localRotation = Quaternion.Euler(0, 0, initialAngle + currentWeapon.thrustStartAngle);
        }

        SetupHitbox(weaponInstance);
        Collider2D col = weaponInstance.GetComponent<Collider2D>();

        if (col != null) col.enabled = false;

        StartCoroutine(HandleDetach(pivot));

        yield return StartCoroutine(PerformFadeIn(weaponInstance));

        yield return StartCoroutine(DelayWeapon(currentWeapon.delayStart));

        foreach (GameObject childObject in childObjects)
        {
            childObject.SetActive(true);
        }

        if (currentWeapon.recoil > 0)
        {
            PlayerMovement pm = GetComponent<PlayerMovement>();
            if (pm != null) pm.ApplyRecoil(-transform.up * currentWeapon.recoil);
        }

        if (col != null) col.enabled = true;

        float progress = 0f;

        while (progress < 1f)
        {
            float currentSpeed = Mathf.Lerp(currentWeapon.startSpeed, currentWeapon.endSpeed, progress);

            progress += currentSpeed * WeaponDeltaTime;

            float t = Mathf.Clamp01(progress);

            Vector3 m1 = Vector3.Lerp(startPos, controlPos, t);
            Vector3 m2 = Vector3.Lerp(controlPos, endPos, t);
            weaponInstance.transform.localPosition = Vector3.Lerp(m1, m2, t);

            Vector3 moveDir = (m2 - m1).normalized;
            if (moveDir.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                float currentAngleOffset = Mathf.Lerp(currentWeapon.thrustStartAngle, currentWeapon.thrustEndAngle, t);
                weaponInstance.transform.localRotation = Quaternion.Euler(0, 0, angle + currentAngleOffset);
            }

            yield return null;
        }

        if (col != null) col.enabled = false;

        DetachTrails(weaponInstance);

        yield return StartCoroutine(FadeAndDestroy(weaponInstance, pivot));
    }

    private IEnumerator PerformSwingArc()
    {
        isSwinging = true;

        GameObject pivot = new GameObject("MeleeSwingPivot");
        pivot.transform.SetParent(transform);

        pivot.transform.localPosition = new Vector3(currentWeapon.spawnOffset.x, currentWeapon.spawnOffset.y, 0f);
        pivot.transform.localScale = new Vector3(currentWeapon.size, currentWeapon.size, currentWeapon.size);

        GameObject weaponInstance = Instantiate(currentWeapon.weaponPrefab, pivot.transform);

        TrailRenderer[] components = weaponInstance.GetComponentsInChildren<TrailRenderer>(true);
        List<GameObject> childObjects = new List<GameObject>();

        foreach (TrailRenderer comp in components)
        {
            childObjects.Add(comp.gameObject);
        }

        weaponInstance.transform.localPosition = new Vector3(0, currentWeapon.radius, 0);
        weaponInstance.transform.localRotation = Quaternion.Euler(0, 0, 45f);

        pivot.transform.localRotation = Quaternion.Euler(0, 0, currentWeapon.swingStartAngle);

        SetupHitbox(weaponInstance);
        Collider2D col = weaponInstance.GetComponent<Collider2D>();

        if (col != null) col.enabled = false;

        StartCoroutine(HandleDetach(pivot));

        yield return StartCoroutine(PerformFadeIn(weaponInstance));

        yield return StartCoroutine(DelayWeapon(currentWeapon.delayStart));

        foreach (GameObject childObject in childObjects)
        {
            childObject.SetActive(true);
        }

        if (currentWeapon.recoil > 0)
        {
            PlayerMovement pm = GetComponent<PlayerMovement>();
            if (pm != null) pm.ApplyRecoil(-transform.up * currentWeapon.recoil);
        }

        if (col != null) col.enabled = true;

        float progress = 0f;
        Quaternion baseRotation = pivot.transform.rotation;

        while (progress < 1f)
        {
            float currentSpeed = Mathf.Lerp(currentWeapon.startSpeed, currentWeapon.endSpeed, progress);

            progress += currentSpeed * WeaponDeltaTime;

            float t = Mathf.Clamp01(progress);

            float currentSwingAngle = Mathf.Lerp(currentWeapon.swingStartAngle, currentWeapon.swingEndAngle, t);

            if (pivot.transform.parent != null)
            {
                baseRotation = pivot.transform.rotation;
            }

            if (pivot.transform.parent != null)
            {
                pivot.transform.localRotation = Quaternion.Euler(0, 0, currentSwingAngle);
            }
            else
            {
                pivot.transform.rotation = baseRotation * Quaternion.Euler(0, 0, currentSwingAngle - currentWeapon.swingStartAngle);
            }
            yield return null;
        }

        if (col != null) col.enabled = false;

        DetachTrails(weaponInstance);

        yield return StartCoroutine(FadeAndDestroy(weaponInstance, pivot));
    }

    private void DetachTrails(GameObject weaponInstance)
    {
        if (weaponInstance == null) return;

        TrailRenderer[] trails = weaponInstance.GetComponentsInChildren<TrailRenderer>();
        foreach (TrailRenderer trail in trails)
        {
            if (trail == null) continue;

            trail.transform.SetParent(null);
            Destroy(trail.gameObject, trail.time);
        }
    }

    private void SetupHitbox(GameObject weaponInstance)
    {
        MeleeHitbox hitbox = weaponInstance.AddComponent<MeleeHitbox>();
        hitbox.damage = currentWeapon.damage;
        hitbox.hitImpact = currentWeapon.hitImpact;
        hitbox.hitStop = currentWeapon.hitStop;
        hitbox.hitSFX = currentWeapon.hitSFX;
        hitbox.knockback = currentWeapon.knockback;
        hitbox.canReflectBullets = currentWeapon.reflectForce > 0;
        hitbox.reflectedBullet = currentWeapon.reflectedBullet;
        hitbox.reflectForce = currentWeapon.reflectForce;
        hitbox.hitTargetMask = currentWeapon.HitTarget;
        hitbox.vulnerabilityIgnore = currentWeapon.vulnerabilityIgnore;

        Collider2D col = weaponInstance.GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        Rigidbody2D rb = weaponInstance.GetComponent<Rigidbody2D>();
        if (rb != null) rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private IEnumerator DelayWeapon(float delayTime)
    {
        if (currentWeapon.delayStart > 0f)
        {
            float delayElapsed = 0f;
            while (delayElapsed < delayTime)
            {
                delayElapsed += WeaponDeltaTime;
                yield return null;
            }
        }

        SFXManager.Instance?.PlaySFX(currentWeapon.slashSFX, transform.position, 0.3f, true, 0.75f, 1.25f);
    }

    private IEnumerator PerformFadeIn(GameObject weaponInstance)
    {
        SpriteRenderer spriteRenderer = weaponInstance.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        float fadeElapsed = 0f;
        while (fadeElapsed < currentWeapon.fadeInDuration)
        {
            fadeElapsed += WeaponDeltaTime;
            float t = fadeElapsed / currentWeapon.fadeInDuration;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0f, 1f, t));
            yield return null;
        }

        spriteRenderer.color = originalColor;
    }

    private IEnumerator FadeAndDestroy(GameObject weaponInstance, GameObject pivot)
    {
        float holdDuration = 0.05f;
        float holdElapsed = 0f;
        while (holdElapsed < holdDuration)
        {
            holdElapsed += WeaponDeltaTime;
            yield return null;
        }

        SpriteRenderer spriteRenderer = weaponInstance.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            float fadeDuration = currentWeapon.fadeOutDuration;
            float fadeElapsed = 0f;
            while (fadeElapsed < fadeDuration)
            {
                fadeElapsed += WeaponDeltaTime;
                float t = fadeElapsed / fadeDuration;
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
                yield return null;
            }
        }

        Destroy(pivot);
        isSwinging = false;
    }

    private IEnumerator HandleDetach(GameObject pivot)
    {
        if (currentWeapon.detachTime <= 0f) yield break;

        float strikeElapsed = 0f;
        while (strikeElapsed < currentWeapon.detachTime)
        {
            if (pivot == null) yield break;

            strikeElapsed += WeaponDeltaTime;
            yield return null;
        }

        if (pivot != null)
        {
            pivot.transform.SetParent(null);
        }
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

        ForceInstantLayoutRefresh();
    }

    private void ForceInstantLayoutRefresh()
    {
        if (refreshCoroutine != null) StopCoroutine(refreshCoroutine);

        refreshCoroutine = StartCoroutine(RefreshLayoutRoutine());
    }

    private IEnumerator RefreshLayoutRoutine()
    {
        yield return new WaitForEndOfFrame();

        Canvas.ForceUpdateCanvases();

        if (hudContainerTransform != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(hudContainerTransform);
        }
    }
}