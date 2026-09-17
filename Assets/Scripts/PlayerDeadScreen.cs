using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerDeadScreen : MonoBehaviour
{
    public GameObject deadScreenUI;
    public CanvasGroup deadScreenCanvasGroup;
    public AudioClip deathClip;
    public AudioClip clickClip;
    public float fadeDuration = 2f;
    public float delayBeforeAppearing = 4f;

    private void playSFX(AudioClip clip)
    {
        if (SFXManager.Instance != null && clip != null)
        {
            SFXManager.Instance.PlaySFX(clip, Vector3.zero);
        }
    }

    private void OnEnable()
    {
        Player.onPlayerDeath += ShowDeadScreen;
    }

    private void OnDisable()
    {
        Player.onPlayerDeath -= ShowDeadScreen;
    }

    private void ShowDeadScreen()
    {
        BGMManager.Instance?.StopBGM();
        StartCoroutine(ShowDeadScreenCoroutine(delayBeforeAppearing));
    }

    private IEnumerator ShowDeadScreenCoroutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        playSFX(deathClip);

        if (deadScreenUI != null)
        {
            deadScreenUI.SetActive(true);
        }

        if (deadScreenCanvasGroup != null)
        {
            deadScreenCanvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                deadScreenCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }

            deadScreenCanvasGroup.alpha = 1f;
        }
    }

    public void RestartGame()
    {
        if (deadScreenUI != null) deadScreenUI.SetActive(false);
        playSFX(clickClip);
        SceneManager.LoadScene("Game");
    }

    public void QuitToMainMenu()
    {
        if (deadScreenUI != null) deadScreenUI.SetActive(false);
        playSFX(clickClip);
        SceneManager.LoadScene("Start Menu");
    }
}