using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    [SerializeField] private float maxHitStopCap = 0.25f;

    private Coroutine hitStopRoutine;
    private float lastValidTimeScale = 1f;
    private float remainingHitStop = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Stop(float duration)
    {
        if (duration <= 0) return;

        if (hitStopRoutine == null)
        {
            if (Time.timeScale > 0f)
            {
                lastValidTimeScale = Time.timeScale;
            }

            remainingHitStop = Mathf.Min(duration, maxHitStopCap);
            hitStopRoutine = StartCoroutine(HitStopWait());
        }
        else
        {
            remainingHitStop = Mathf.Min(Mathf.Max(remainingHitStop, duration), maxHitStopCap);
        }
    }

    private IEnumerator HitStopWait()
    {
        Time.timeScale = 0f;

        while (remainingHitStop > 0f)
        {
            if (PauseMenu.isPaused)
            {
                yield return null;
                continue;
            }

            remainingHitStop -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (!PauseMenu.isPaused)
        {
            Time.timeScale = lastValidTimeScale;
        }

        hitStopRoutine = null;
    }

    public void ForceRestoreTimeScale(float targetScale = 1f)
    {
        lastValidTimeScale = targetScale;
        remainingHitStop = 0f;

        if (hitStopRoutine != null)
        {
            StopCoroutine(hitStopRoutine);
            hitStopRoutine = null;
        }

        Time.timeScale = targetScale;
    }
}