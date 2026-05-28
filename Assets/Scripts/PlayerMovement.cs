using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    public PlayerData data;
    public Rigidbody2D rb;
    public Camera cam;
    public TrailRenderer tr;

    private bool canPush = true;
    public bool isPushing = false;

    private bool canDash = true;
    public bool isDashing;

    [SerializeField] Image dashBar;
    [SerializeField] Image pushBar;
    [SerializeField] Image slowmoBar;

    private CinemachineImpulseSource impulseSource;

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

        float normalDuration = data.slowMoDuration - blinkStartTime;
        yield return new WaitForSecondsRealtime(normalDuration);

        float blinkTimer = 0f;
        bool isVisible = true;

        while (blinkTimer < blinkStartTime)
        {
            isVisible = !isVisible;
            SetOverlayAlpha(isVisible ? maxOverlayAlpha : 0f);

            yield return new WaitForSecondsRealtime(blinkInterval);
            blinkTimer += blinkInterval;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (slowMoOverlay != null)
        {
            slowMoOverlay.gameObject.SetActive(false);
        }

        IsSlowMoActive = false;

        yield return new WaitForSecondsRealtime(data.slowMoCooldown);
        float timer = 0f;
        while (timer < data.slowMoCooldown)
        {
            timer += Time.deltaTime;
            if (slowmoBar != null)
            {
                slowmoBar.fillAmount = timer / data.slowMoCooldown;
            }
            yield return null;
        }

        if (slowmoBar != null) slowmoBar.fillAmount = 1f;

        SFXManager.Instance?.PlaySFX(data.SlowmoAlready, transform.position, 0.3f, true, 0.75f, 1.25f);
        isCooldown = false;
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
        SFXManager.Instance?.PlaySFX(data.dashSFX, transform.position);

        Vector2 dashDir = (mousePos - rb.position).normalized;
        int originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Dashing");
        canDash = false;
        isDashing = true;

        float adaptiveDashPower = data.dashPower / Time.timeScale;
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

    public void OnPush(InputAction.CallbackContext context)
    {
        if (canPush && context.performed)
        {
            StartCoroutine(PushFrontRoutine());
        }
    }

    private IEnumerator PushFrontRoutine()
    {
        canPush = false;
        isPushing = true;

        if (data.pushSFX != null) SFXManager.Instance?.PlaySFX(data.pushSFX, transform.position);

        Vector2 pushCenter = rb.position + (Vector2)transform.up * data.pushOffset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(pushCenter, data.pushRadius);
        bool hitSomething = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            if (hit.GetComponent<Enemy>() != null) continue;

            if (hit.GetComponent<WeaponPickup>() != null) continue;

            if (hit.GetComponent<Bullet_e>() != null)
            {

                GameObject reflected_bullet = ObjectPooler.Instance.SpawnFromPool(data.BulletPlayer, hit.transform.position, Quaternion.identity);

                Rigidbody2D bulletRb = reflected_bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.linearVelocity = Vector2.zero;
                    bulletRb.AddTorque(UnityEngine.Random.Range(-6f, 6f), ForceMode2D.Impulse);
                }

                hit.gameObject.SetActive(false);
                continue;
            }

            Rigidbody2D targetRb = hit.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                hitSomething = true;

                targetRb.AddForce(transform.up * data.pushForce * (targetRb.mass * 0.5f), ForceMode2D.Impulse);
                targetRb.AddTorque(UnityEngine.Random.Range(-6f, 6f), ForceMode2D.Impulse);
            }
        }

        if (hitSomething)
        {
            CameraShakeManager.Instance?.CameraShake(impulseSource, 0.15f);
        }

        if (pushBar != null) pushBar.fillAmount = 0f;

        float pushTimer = 0f;
        while (pushTimer < data.pushCD)
        {
            pushTimer += Time.unscaledDeltaTime;
            if (pushBar != null) pushBar.fillAmount = pushTimer / data.pushCD;
            yield return null;
        }

        if (pushBar != null) pushBar.fillAmount = 1f;
        isPushing = false;
        canPush = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pushCenter = transform.position + transform.up * data.pushOffset;
        Gizmos.DrawWireSphere(pushCenter, data.pushRadius);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null && isDashing && !collision.gameObject.TryGetComponent<Explode>(out Explode explode))
        {
            PlayerMovement playerMovement = this.GetComponent<PlayerMovement>();

            HitStop.Instance?.Stop(0.1f, playerMovement);
            SFXManager.Instance?.PlaySFX(data.dashCrashSFX, transform.position);
            CameraShakeManager.Instance?.CameraShake(impulseSource, 0.25f);
            damageable.takeDmg(data.dashDMG);

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.isCrashing = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        float adaptiveSpeed = IsSlowMoActive ? (data.moveSpd / Time.timeScale) : data.moveSpd;

        Vector2 finalMovement = (movement * adaptiveSpeed) + recoilVelocity;
        rb.MovePosition(rb.position + finalMovement * Time.fixedDeltaTime);

        recoilVelocity = Vector2.MoveTowards(recoilVelocity, Vector2.zero, recoilDecaySpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;

        rb.MoveRotation(angle);
    }
}