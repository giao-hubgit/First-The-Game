using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }
    private bool waiting;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Stop(float duration, PlayerMovement playerMovement)
    {
        if (waiting || duration <= 0) return;
        StartCoroutine(HitStopWait(duration, playerMovement));
    }

    private IEnumerator HitStopWait(float duration, PlayerMovement playerMovement)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);

        if (playerMovement != null && playerMovement.IsSlowMoActive)
        {
            Time.timeScale = playerMovement.data.slowMoTimeScale;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
