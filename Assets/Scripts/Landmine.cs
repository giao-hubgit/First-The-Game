using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Explode))]
public class Landmine : MonoBehaviour
{
    [Header("Audio & Light")]
    [SerializeField] private AudioClip tickSFX;
    [SerializeField] private Light2D spotLight;

    [Header("Sprites (Hình ảnh)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite flashSprite;

    [Header("Flash Settings")]
    [SerializeField] private float baseIntensity = 0.1f;
    [SerializeField] private float flashIntensity = 2.0f;
    [SerializeField] private float flashDuration = 0.08f;

    public bool touched = false;

    private Explode explodeComponent;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        explodeComponent = GetComponent<Explode>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spotLight == null)
        {
            spotLight = GetComponentInChildren<Light2D>();
        }
    }

    private void Start()
    {
        if (spotLight != null) spotLight.intensity = baseIntensity;
        if (spriteRenderer != null && idleSprite != null) spriteRenderer.sprite = idleSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!touched)
        {
            if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
            {
                touched = true;
                StartCoroutine(BoomRoutine());
            }
        }
    }

    private IEnumerator BoomRoutine()
    {
        int totalTicks = 16;
        float currentDelay = 1f;
        float speedUpFactor = 0.85f;

        for (int i = 0; i < totalTicks; i++)
        {
            SFXManager.Instance?.PlaySFX(tickSFX, transform.position, 0.2f, false);

            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRoutine());

            yield return new WaitForSeconds(currentDelay);

            currentDelay *= speedUpFactor;
        }

        Detonate();
    }

    private IEnumerator FlashRoutine()
    {
        if (spriteRenderer != null && flashSprite != null) spriteRenderer.sprite = flashSprite;
        if (spotLight != null) spotLight.intensity = flashIntensity;

        yield return new WaitForSeconds(flashDuration);

        if (spriteRenderer != null && idleSprite != null) spriteRenderer.sprite = idleSprite;
        if (spotLight != null) spotLight.intensity = baseIntensity;
    }

    public void BoomImmediatly()
    {
        StopAllCoroutines();
        Detonate();
    }

    private void Detonate()
    {
        if (explodeComponent != null)
        {
            explodeComponent.DoExplosion();
        }

        Destroy(gameObject);
    }
}