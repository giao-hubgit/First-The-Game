using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;
using NUnit.Framework.Internal;

public class PlayerMovement : MonoBehaviour
{
    public PlayerData data;
    public Rigidbody2D rb;
    public Camera cam;
    public TrailRenderer tr;

    public bool isHoldingForce = false;
    public bool isPushing = false;

    private float currentForceEnergy = 1f;

    private bool canDash = true;
    public bool isDashing;

    [SerializeField] Image dashBar;
    [SerializeField] Image forceBar;
    [SerializeField] Image slowmoBar;

    [SerializeField] CinemachineImpulseSource impulseSource;

    Vector2 movement;
    Vector2 mousePos;

    private Vector2 recoilVelocity;
    [SerializeField] float recoilDecaySpeed = 25f;

    [SerializeField] private float blinkStartTime = 1f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("UI References")]
    [SerializeField] private UnityEngine.UI.Image slowMoOverlay;
    [SerializeField] private float maxOverlayAlpha = 0.25f;

    public bool IsSlowMoActive { get; private set; } = false;
    private bool isCooldown = false;

    public void ApplyRecoil(Vector2 force)
    {
        recoilVelocity += force;
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        HandleForceMechanic();
    }

    public void OnForce(InputAction.CallbackContext context)
    {
        if (context.started && currentForceEnergy >= data.minForceEnergyToStart)
        {
            isHoldingForce = true;
        }
        else if (context.canceled)
        {
            if (isHoldingForce)
            {
                ReleaseForce();
            }
            isHoldingForce = false;
        }
    }

    private void HandleForceMechanic()
    {
        if (isHoldingForce)
        {
            isPushing = true;
            currentForceEnergy -= data.forceDrainRate * Time.deltaTime;

            if (forceBar != null) forceBar.fillAmount = Mathf.Clamp01(currentForceEnergy);

            MaintainForceAoE();

            if (currentForceEnergy <= 0f)
            {
                currentForceEnergy = 0f;
                ReleaseForce();
                isHoldingForce = false;
            }
        }
        else
        {
            if (currentForceEnergy < 1f)
            {
                currentForceEnergy += data.forceRechargeRate * Time.deltaTime;
                currentForceEnergy = Mathf.Clamp01(currentForceEnergy);
                if (forceBar != null) forceBar.fillAmount = currentForceEnergy;
            }
        }
    }

    private void MaintainForceAoE()
    {
        Vector2 pushCenter = rb.position + (Vector2)transform.up * data.forceOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(pushCenter, data.forceRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            if (hit.GetComponent<Enemy>() != null) continue;
            if (hit.GetComponent<WeaponPickup>() != null) continue;

            if (hit.GetComponent<Bullet>() != null && hit.CompareTag("EnemyBullet"))
            {
                Rigidbody2D bulletRb = hit.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.linearVelocity = Vector2.zero;
                    bulletRb.AddTorque(UnityEngine.Random.Range(-10f, 10f), ForceMode2D.Force);
                }
                continue;
            }

            Rigidbody2D targetRb = hit.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                targetRb.AddTorque(UnityEngine.Random.Range(-6f, 6f), ForceMode2D.Force);
            }
        }
    }

    private void ReleaseForce()
    {
        isPushing = false;
        Vector2 pushCenter = rb.position + (Vector2)transform.up * data.forceOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(pushCenter, data.forceRadius);
        bool hitSomething = false;

        if (data.pushSFX != null) SFXManager.Instance?.PlaySFX(data.pushSFX, transform.position);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            if (hit.GetComponent<Enemy>() != null) continue;
            if (hit.GetComponent<WeaponPickup>() != null) continue;

            if (hit.GetComponent<Bullet>() != null && hit.CompareTag("EnemyBullet"))
            {
                GameObject reflected_bullet = ObjectPooler.Instance.SpawnFromPool(data.BulletPlayer, hit.transform.position, Quaternion.identity);
                reflected_bullet.transform.localScale = hit.transform.localScale;

                if (hit.transform.TryGetComponent<Bullet>(out Bullet enemyBullet) &&
                    hit.transform.TryGetComponent<TrailRenderer>(out TrailRenderer enemyTrail))
                {
                    Bullet playerBullet = reflected_bullet.GetComponent<Bullet>();
                    TrailRenderer playerTrail = reflected_bullet.GetComponent<TrailRenderer>();

                    if (enemyBullet.data != null)
                    {
                        BulletData clonedData = Instantiate(enemyBullet.data);
                        clonedData.damage *= 1;

                        playerBullet.data.damage = clonedData.damage;
                        playerBullet.data.knockback = clonedData.knockback;
                    }

                    if (playerTrail != null)
                    {
                        playerTrail.widthCurve = enemyTrail.widthCurve;
                        playerTrail.widthMultiplier = enemyTrail.widthMultiplier;
                        playerTrail.Clear();
                    }
                }

                Rigidbody2D bulletRb = reflected_bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.linearVelocity = (Vector2)transform.up * data.pushForce;
                    bulletRb.AddTorque(UnityEngine.Random.Range(-6f, 6f), ForceMode2D.Impulse);
                }

                hit.gameObject.SetActive(false);
                hitSomething = true;
                continue;
            }

            Rigidbody2D targetRb = hit.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                hitSomething = true;
                targetRb.AddForce(transform.up * data.pushForce * (targetRb.mass >= 6 ? targetRb.mass * 0.75f : targetRb.mass), ForceMode2D.Impulse);
                targetRb.AddTorque(UnityEngine.Random.Range(-6f, 6f), ForceMode2D.Impulse);
            }
        }

        if (hitSomething)
        {
            CameraShakeManager.Instance?.CameraShake(impulseSource, 0.15f);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 screenMousePos = context.ReadValue<Vector2>();

        if (cam != null)
        {
            mousePos = cam.ScreenToWorldPoint(screenMousePos);
        }
    }

    public void onDash(InputAction.CallbackContext context)
    {
        if (canDash && context.performed)
        {
            StartCoroutine(Dash());
        }
    }

    public void onSlowMotion(InputAction.CallbackContext context)
    {
        if (context.performed && !IsSlowMoActive && !isCooldown)
        {
            StartCoroutine(SlowMotion());
        }
    }

    public IEnumerator SlowMotion()
    {
        if (PauseMenu.isPaused) yield break;

        IsSlowMoActive = true;
        isCooldown = true;
        if (slowmoBar != null) slowmoBar.fillAmount = 0f;

        SFXManager.Instance?.PlaySFX(data.Slowmo, transform.position, 0.3f, true, 0.75f, 1.25f);

        if (slowMoOverlay != null)
        {
            slowMoOverlay.gameObject.SetActive(true);
            SetOverlayAlpha(maxOverlayAlpha);
        }

        Time.timeScale = data.slowMoTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        HitStop.Instance?.ForceRestoreTimeScale(data.slowMoTimeScale);

        float normalDuration = data.slowMoDuration - blinkStartTime;

        yield return StartCoroutine(StartCountdown(normalDuration));

        float blinkTimer = 0f;
        bool isVisible = true;

        while (blinkTimer < blinkStartTime)
        {
            isVisible = !isVisible;
            SetOverlayAlpha(isVisible ? maxOverlayAlpha : 0f);

            yield return StartCoroutine(StartCountdown(blinkInterval));
            blinkTimer += blinkInterval;
        }

        if (!PauseMenu.isPaused)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            HitStop.Instance?.ForceRestoreTimeScale(1f);
        }

        if (slowMoOverlay != null)
        {
            slowMoOverlay.gameObject.SetActive(false);
        }

        IsSlowMoActive = false;

        float timer = 0f;
        while (timer < data.slowMoCooldown)
        {
            if (!PauseMenu.isPaused)
            {
                timer += Time.unscaledDeltaTime;
                if (slowmoBar != null)
                {
                    slowmoBar.fillAmount = timer / data.slowMoCooldown;
                }
            }
            yield return null;
        }

        if (slowmoBar != null) slowmoBar.fillAmount = 1f;

        SFXManager.Instance?.PlaySFX(data.SlowmoAlready, transform.position, 0.3f, true, 0.75f, 1.25f);
        isCooldown = false;
    }

    private IEnumerator StartCountdown(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            if (!PauseMenu.isPaused)
            {
                timer += Time.unscaledDeltaTime;
            }
            yield return null;
        }
    }

    private void SetOverlayAlpha(float alpha)
    {
        if (slowMoOverlay == null) return;
        Color c = slowMoOverlay.color;
        c.a = alpha;
        slowMoOverlay.color = c;
    }

    public IEnumerator Dash()
    {
        Vector2 dashDir = (mousePos - rb.position).normalized;
        if (dashDir == Vector2.zero) dashDir = transform.up;

        SFXManager.Instance?.PlaySFX(data.dashSFX, transform.position);

        int originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Dashing");
        canDash = false;
        isDashing = true;

        float currentTimeScale = Time.timeScale > 0 ? Time.timeScale : 1f;
        float adaptiveDashPower = data.dashPower / currentTimeScale;

        rb.linearVelocity = dashDir * adaptiveDashPower;
        if (tr != null) tr.emitting = true;
        yield return new WaitForSecondsRealtime(data.dashTime);

        Player player = GetComponent<Player>();
        if (player != null)
        {
            player.TriggerInvulnerability(0.2f);
        }

        if (tr != null) tr.emitting = false;
        rb.linearVelocity = Vector2.zero;
        gameObject.layer = originalLayer;

        if (dashBar != null) dashBar.fillAmount = 0f;
        isDashing = false;

        float timer = 0f;
        while (timer < data.dashCD)
        {
            timer += Time.unscaledDeltaTime;
            if (dashBar != null)
            {
                dashBar.fillAmount = timer / data.dashCD;
            }
            yield return null;
        }

        if (dashBar != null) dashBar.fillAmount = 1f;
        canDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pushCenter = transform.position + transform.up * data.forceOffset;
        Gizmos.DrawWireSphere(pushCenter, data.forceRadius);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null && isDashing && !collision.gameObject.TryGetComponent<Explode>(out Explode explode))
        {
            PlayerMovement playerMovement = this.GetComponent<PlayerMovement>();

            HitStop.Instance?.Stop(data.dashHitStop);
            SFXManager.Instance?.PlaySFX(data.dashCrashSFX, transform.position);
            CameraShakeManager.Instance?.CameraShake(impulseSource, 0.25f);
            damageable.takeDmg(data.dashDMG * data.baseDMG);

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.isCrashing = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing || Time.timeScale <= 0f) return;

        float safeTimeScale = Mathf.Max(Time.timeScale, 0.0001f);
        float adaptiveSpeed = IsSlowMoActive ? (data.moveSpd / safeTimeScale) : data.moveSpd;

        Vector2 finalMovement = (movement * adaptiveSpeed) + recoilVelocity;
        rb.MovePosition(rb.position + finalMovement * Time.fixedDeltaTime);

        recoilVelocity = Vector2.MoveTowards(recoilVelocity, Vector2.zero, recoilDecaySpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mousePos - rb.position;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.MoveRotation(angle);
        }
    }
}