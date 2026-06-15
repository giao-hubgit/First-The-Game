using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour
{
    public float delay = 0.5f;
    public float fadeDuration = 0.3f;

    private SpriteRenderer sr;
    private TrailRenderer tr;
    private Color originalColor;
    private Coroutine fadeCoroutine;
    private Vector3 originalScale;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<TrailRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
        }

        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (sr != null) sr.color = originalColor;

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