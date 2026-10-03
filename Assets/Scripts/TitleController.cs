using System.Collections;
using UnityEngine;
using TMPro;

public class TitleController : MonoBehaviour
{
    public static TitleController Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI titleText;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (titleText != null)
            titleText.gameObject.SetActive(false);
    }

    public void SpawnTitle(
        string title,
        Color startColor,
        float fadeInDuration = 0.5f,
        float holdDuration = 1.5f,
        float fadeOutDuration = 1.0f,
        float delayBeforeSpawn = 0f)
    {
        if (titleText == null) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeTitleRoutine(title, startColor, fadeInDuration, holdDuration, fadeOutDuration, delayBeforeSpawn));
    }

    private IEnumerator FadeTitleRoutine(
        string title,
        Color startColor,
        float fadeInDuration,
        float holdDuration,
        float fadeDuration,
        float delayBeforeSpawn)
    {
        if (delayBeforeSpawn > 0f)
        {
            yield return new WaitForSeconds(delayBeforeSpawn);
        }

        titleText.text = title;

        Color zeroAlphaColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        titleText.color = zeroAlphaColor;
        titleText.gameObject.SetActive(true);

        if (fadeInDuration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                titleText.color = Color.Lerp(zeroAlphaColor, startColor, elapsed / fadeInDuration);
                yield return null;
            }
        }

        titleText.color = startColor;

        if (holdDuration > 0f)
        {
            yield return new WaitForSeconds(holdDuration);
        }

        if (fadeDuration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                titleText.color = Color.Lerp(startColor, zeroAlphaColor, elapsed / fadeDuration);
                yield return null;
            }
        }

        titleText.color = zeroAlphaColor;
        titleText.gameObject.SetActive(false);
    }
}