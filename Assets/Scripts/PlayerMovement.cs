using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpd = 5f;
    public Rigidbody2D rb;
    public Camera cam;
    public TrailRenderer tr;

    public float pushForce = 25f;
    public float pushRadius = 0.6f;
    public float pushOffset = 1f;
    public float pushCD = 0.5f;
    private bool canPush = true;
    public bool isPushing = false;
    [SerializeField] AudioClip pushSFX;
    [SerializeField] Image pushBar;


    private bool canDash = true;
    public bool isDashing;
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashCD = 1f;
    public int dashDMG = 20;
    public float crashTimer = 1f;
    [SerializeField] AudioClip dashCrashSFX;
    [SerializeField] AudioClip dashSFX;
    [SerializeField] Image dashBar;


    private CinemachineImpulseSource impulseSource;

    Vector2 movement;
    Vector2 mousePos;

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

    public IEnumerator Dash()
    {
        SFXManager.Instance?.PlaySFX(dashSFX, transform.position);

        Vector2 dashDir = (mousePos - rb.position).normalized;
        int originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Dashing");
        canDash = false;
        isDashing = true;
        rb.linearVelocity = dashDir * dashPower;
        if (tr != null) tr.emitting = true;
        yield return new WaitForSeconds(dashTime);
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
        while (timer < dashCD)
        {
            timer += Time.deltaTime;
            if (dashBar != null)
            {
                dashBar.fillAmount = timer / dashCD;
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

        if (pushSFX != null) SFXManager.Instance?.PlaySFX(pushSFX, transform.position);

        Vector2 pushCenter = rb.position + (Vector2)transform.up * pushOffset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(pushCenter, pushRadius);
        bool hitSomething = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            if (hit.GetComponent<Enemy>() != null) continue;

            if (hit.GetComponent<Bullet_e>() != null) continue;

            Rigidbody2D targetRb = hit.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                hitSomething = true;

                targetRb.AddForce(transform.up * pushForce * (targetRb.mass * 0.5f), ForceMode2D.Impulse);
                targetRb.AddTorque(UnityEngine.Random.Range(-6f, 6f), ForceMode2D.Impulse);
            }
        }

        if (hitSomething)
        {
            CameraShakeManager.Instance?.CameraShake(impulseSource, 0.15f);
        }

        if (pushBar != null) pushBar.fillAmount = 0f;

        float pushTimer = 0f;
        while (pushTimer < pushCD)
        {
            pushTimer += Time.deltaTime;
            if (pushBar != null) pushBar.fillAmount = pushTimer / pushCD;
            yield return null;
        }

        if (pushBar != null) pushBar.fillAmount = 1f;
        isPushing = false;
        canPush = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pushCenter = transform.position + transform.up * pushOffset;
        Gizmos.DrawWireSphere(pushCenter, pushRadius);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null && isDashing && !collision.gameObject.TryGetComponent<Explode>(out Explode explode))
        {
            HitStop.Instance?.Stop(0.1f);
            SFXManager.Instance?.PlaySFX(dashCrashSFX, transform.position);
            CameraShakeManager.Instance?.CameraShake(impulseSource, 0.25f);
            damageable.takeDmg(dashDMG);

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

        rb.MovePosition(rb.position + movement * moveSpd * Time.fixedDeltaTime);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;

        rb.MoveRotation(angle);
    }
}