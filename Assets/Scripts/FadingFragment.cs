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
        // 1. Đứng yên một lúc cho người chơi thấy mảnh vỡ
        yield return new WaitForSeconds(waitBeforeFade);

        // 2. Bắt đầu mờ dần
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 3. Sau khi mờ hẳn, nếu cha của nó không còn tác dụng, ta có thể tắt nó đi.
        // Lưu ý: Vì bạn đang dùng Object Pool cho cả Cụm (brokenObj), 
        // nên ta cần báo cho cụm cha biết khi nào nên tắt.
    }
}