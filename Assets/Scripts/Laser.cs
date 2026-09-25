using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Laser : MonoBehaviour
{
    [Header("Laser Configuration")]
    [SerializeField] public LaserData laserData;

    [Header("Collision Settings")]
    [SerializeField] private Transform laserHead;

    private BoxCollider2D laserCollider;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer headSpriteRenderer;

    private float currentAngleOffset = 0f;
    private float nextSFXTime = 0f;
    private bool isFading = false;

    private Dictionary<IDamageable, float> damageTimers = new Dictionary<IDamageable, float>();

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

        currentAngleOffset = laserData.startAngleOffset;
        transform.localRotation = Quaternion.Euler(0f, 0f, currentAngleOffset);

        damageTimers.Clear();

        nextSFXTime = 0f;
        isFading = false;
        ResetAlpha();

        if (spriteRenderer != null)
        {
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;
        }

        UpdateLaserLength(laserData.length);
    }

    private void Update()
    {
        if (laserData == null || isFading) return;

        currentAngleOffset = Mathf.MoveTowards(
            currentAngleOffset,
            laserData.endAngleOffset,
            laserData.rotationSpeed * Time.deltaTime
        );

        transform.localRotation = Quaternion.Euler(0f, 0f, currentAngleOffset);

        if (Time.time >= nextSFXTime)
        {
            SFXManager.Instance?.PlaySFX(laserData.laserSFX, transform.position);
            nextSFXTime = Time.time + 0.1f;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, laserData.length, laserData.obstacleLayer);
        float actualLength = laserData.length;

        if (hit.collider != null)
        {
            actualLength = hit.distance;

            if (!string.IsNullOrEmpty(laserData.particlePoolName) && laserData.particlePoolName != "None")
            {
                ObjectPooler.Instance?.SpawnFromPool(laserData.particlePoolName, hit.point, Quaternion.identity);
            }
        }

        UpdateLaserLength(actualLength);

        if (Mathf.Approximately(currentAngleOffset, laserData.endAngleOffset))
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
        if (laserData == null || isFading) return;

        if (collision.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            bool isIgnored = !string.IsNullOrEmpty(laserData.ignoreTag) && collision.gameObject.CompareTag(laserData.ignoreTag);
            if (isIgnored) return;

            if (!damageTimers.ContainsKey(damageable) || Time.time >= damageTimers[damageable])
            {
                damageable.takeDmg(laserData.damage);
                damageTimers[damageable] = Time.time + laserData.damageTick;
            }
        }
    }

    private IEnumerator FadeOutLaserRoutine()
    {
        isFading = true;
        float elapsedTime = 0f;

        Color bodyColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        Color headColor = headSpriteRenderer != null ? headSpriteRenderer.color : Color.white;

        while (elapsedTime < laserData.fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / laserData.fadeDuration);

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

        if (ObjectPooler.Instance != null)
        {
            transform.SetParent(ObjectPooler.Instance.transform);
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