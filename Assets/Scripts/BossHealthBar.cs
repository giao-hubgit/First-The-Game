using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public static BossHealthBar Instance;

    [Header("UI References")]
    [SerializeField] private RectTransform barContainer;
    [SerializeField] private Image fillBar;
    [SerializeField] private Image easeBar;
    [SerializeField] private TextMeshProUGUI bossNameText;

    [Header("Animation Settings")]
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private float animDuration = 0.6f;
    private Boss targetBoss;
    private float targetFillAmount = 1f;
    private Coroutine animCoroutine;

    private void Awake()
    {
        Instance = this;
        if (barContainer == null) barContainer = GetComponent<RectTransform>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void InitHealthBar(Boss boss)
    {
        targetBoss = boss;

        targetBoss.OnHealthChanged += HandleHealthChanged;
        targetBoss.OnBossDeath += HandleBossDeath;

        if (bossNameText != null && boss.bossData != null)
        {
            bossNameText.text = boss.bossData.bossName;
        }

        targetFillAmount = 1f;
        fillBar.fillAmount = 1f;
        easeBar.fillAmount = 1f;

        gameObject.SetActive(true);

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimateShow());
    }

    private IEnumerator AnimateShow()
    {
        barContainer.pivot = new Vector2(0f, 0.5f);
        barContainer.localScale = new Vector3(0f, 1f, 1f);

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animDuration;

            float scaleX = Mathf.SmoothStep(0f, 1f, t);
            barContainer.localScale = new Vector3(scaleX, 1f, 1f);
            yield return null;
        }

        barContainer.localScale = Vector3.one;
    }

    private IEnumerator AnimateHide()
    {
        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animDuration;

            float scaleX = Mathf.SmoothStep(1f, 0f, t);
            barContainer.localScale = new Vector3(scaleX, 1f, 1f);
            yield return null;
        }

        barContainer.localScale = new Vector3(0f, 1f, 1f);
        gameObject.SetActive(false);
    }

    private void HandleHealthChanged(float healthPercent)
    {
        targetFillAmount = healthPercent;
        fillBar.fillAmount = targetFillAmount;
    }

    private void Update()
    {
        if (easeBar != null && fillBar != null && easeBar.fillAmount > fillBar.fillAmount)
        {
            easeBar.fillAmount = Mathf.Lerp(easeBar.fillAmount, fillBar.fillAmount, Time.deltaTime * lerpSpeed);
        }
    }

    private void HandleBossDeath()
    {
        if (targetBoss != null)
        {
            targetBoss.OnHealthChanged -= HandleHealthChanged;
            targetBoss.OnBossDeath -= HandleBossDeath;
        }

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimateHide());
    }
}