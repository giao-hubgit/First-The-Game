using System.Collections;
using UnityEngine;

public class CinematicBars : MonoBehaviour
{
    public static CinematicBars Instance { get; private set; }

    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private float barHeight = 120f;
    [SerializeField] private float transitionDuration = 0.5f;

    private Coroutine activeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        SetBarHeight(0f);
    }

    public void Show(float duration = -1f)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(AnimateBars(barHeight, transitionDuration));

        if (duration > 0)
        {
            Invoke(nameof(Hide), duration);
        }
    }

    public void Hide()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(AnimateBars(0f, transitionDuration));
    }

    private IEnumerator AnimateBars(float targetHeight, float duration)
    {
        float elapsed = 0f;
        float startHeight = topBar.sizeDelta.y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentHeight = Mathf.Lerp(startHeight, targetHeight, elapsed / duration);
            SetBarHeight(currentHeight);
            yield return null;
        }

        SetBarHeight(targetHeight);
    }

    private void SetBarHeight(float height)
    {
        topBar.sizeDelta = new Vector2(topBar.sizeDelta.x, height);
        bottomBar.sizeDelta = new Vector2(bottomBar.sizeDelta.x, height);
    }
}