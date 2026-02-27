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

    public void Stop(float duration)
    {
        if (waiting || duration <= 0) return;
        StartCoroutine(HitStopWait(duration));
    }

    private IEnumerator HitStopWait(float duration)
    {
        waiting = true;
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(duration);
        waiting = false;
        Time.timeScale = 1f;
    }
}
