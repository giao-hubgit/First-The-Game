using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;

    private float elapsedTime = 0f;
    private bool isRunning = false;

    void Start()
    {
        isRunning = true;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;

            DisplayTime(elapsedTime);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        int milliSeconds = Mathf.FloorToInt((timeToDisplay % 1) * 100);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliSeconds);
    }

    public void SetTimerActive(bool state)
    {
        isRunning = state;
    }
}
