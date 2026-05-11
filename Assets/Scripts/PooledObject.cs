using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour
{
    public float delay = 0.5f;
    public float fadeDuration = 0.3f;

    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
        }
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

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
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