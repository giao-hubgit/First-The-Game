using UnityEngine;
using System.Collections;

public class FadingFragment : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [SerializeField] private float waitBeforeFade = 2f;
    [SerializeField] private float fadeDuration = 1f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void OnEnable()
    {
        spriteRenderer.color = originalColor;
        StartCoroutine(FadeOutAndDisable());
    }

    IEnumerator FadeOutAndDisable()
    {
        yield return new WaitForSeconds(waitBeforeFade);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
}