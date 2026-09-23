using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class PooledObject : MonoBehaviour
{
    public float delay = 0.5f;
    public float fadeDuration = 0.3f;

    private SpriteRenderer sr;
    private TrailRenderer tr;
    private Color originalColor;
    private Coroutine fadeCoroutine;
    private Vector3 originalScale;
    private Light2D light2D;
    private float lightIntensity;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<TrailRenderer>();
        light2D = GetComponent<Light2D>();

        if (sr != null)
        {
            originalColor = sr.color;
        }

        if (light2D != null)
        {
            lightIntensity = light2D.intensity;
        }

        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (sr != null) sr.color = originalColor;
        if (light2D != null) light2D.intensity = lightIntensity;

        fadeCoroutine = StartCoroutine(FadeAndDeactivate());
    }

    private IEnumerator FadeAndDeactivate()
    {
        yield return new WaitForSeconds(delay);

        if (sr != null)
        {
            float timer = 0f;
            Color startColor = sr.color;
            transform.localScale = originalScale;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;

                float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

                if (light2D != null)
                {
                    float instensity = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                    light2D.intensity = instensity;
                }

                yield return null;
            }
        }

        if (tr != null)
        {
            tr.Clear();
        }

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
    }
}