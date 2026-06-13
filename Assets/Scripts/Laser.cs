using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Laser : MonoBehaviour
{
    [Header("Laser Configuration")]
    [SerializeField] private LaserData laserData;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private string ignoreTag = "Enemy";
    [SerializeField] private Transform laserHead;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 2f;

    private BoxCollider2D laserCollider;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer headSpriteRenderer;

    private float currentSweptAngle = 0f;
    private float nextDamageTime = 0f;
    private float nextSFXTime = 0f;

    private bool isFading = false;

    private void Awake()
    {
        laserCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (laserHead != null)
        {
            headSpriteRenderer = laserHead.GetComponent<SpriteRenderer>();
        }

        if (laserCollider != null) laserCollider.isTrigger = true;
        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }
    }

    private void OnEnable()
    {
        if (laserData == null) return;

        currentSweptAngle = 0f;
        nextDamageTime = 0f;

        isFading = false;
        ResetAlpha();

        if (spriteRenderer != null)
        {
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;
        }

        transform.Rotate(0f, 0f, laserData.startAngleOffset);

        UpdateLaserLength(laserData.length);
    }

    private void Update()
    {
        if (laserData == null) return;

        if (isFading) return;

        float angleToRotate = laserData.rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, 0f, angleToRotate);
        currentSweptAngle += Mathf.Abs(angleToRotate);

        if (Time.time >= nextSFXTime)
        {
            SFXManager.Instance?.PlaySFX(laserData.laserSFX, transform.position);
            nextSFXTime = Time.time + 0.1f;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, laserData.length, obstacleLayer);

        float actualLength = laserData.length;

        if (hit.collider != null)
        {
            actualLength = hit.distance;
        }

        UpdateLaserLength(actualLength);

        if (currentSweptAngle >= laserData.rotationAngle)
        {
            StartCoroutine(FadeOutLaserRoutine());
        }
    }

    private void UpdateLaserLength(float length)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.size = new Vector2(length, laserData.width);
        }

        if (laserCollider != null)
        {
            laserCollider.size = new Vector2(length, laserData.width);
            laserCollider.offset = new Vector2(length / 2f, 0f);
        }

        if (laserHead != null)
        {
            laserHead.localPosition = new Vector3(length, 0f, 0f);
            laserHead.localScale = new Vector3(laserData.width * 1.5f, laserData.width * 1.5f, 1f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (laserData == null) return;

        if (isFading) return;

        if (Time.time >= nextDamageTime)
        {
            if (collision.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                bool isIgnored = !string.IsNullOrEmpty(ignoreTag) && collision.gameObject.CompareTag(ignoreTag);

                if (!isIgnored)
                {
                    damageable.takeDmg(laserData.damage);
                    nextDamageTime = Time.time + laserData.damageTick;
                }
            }
        }
    }

    private IEnumerator FadeOutLaserRoutine()
    {
        isFading = true;
        float elapsedTime = 0f;

        Color bodyColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        Color headColor = headSpriteRenderer != null ? headSpriteRenderer.color : Color.white;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            if (spriteRenderer != null)
            {
                bodyColor.a = alpha;
                spriteRenderer.color = bodyColor;
            }

            if (headSpriteRenderer != null)
            {
                headColor.a = alpha;
                headSpriteRenderer.color = headColor;
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void ResetAlpha()
    {
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        if (headSpriteRenderer != null)
        {
            Color c = headSpriteRenderer.color;
            c.a = 1f;
            headSpriteRenderer.color = c;
        }
    }
}