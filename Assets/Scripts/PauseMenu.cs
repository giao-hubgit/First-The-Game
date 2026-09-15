using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public PlayerInput playerInput;
    public static bool isPaused = false;

    public GameObject settingMenu;
    public AudioClip clickClip;

    public void onPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isPaused)
            {
                resumeGame();
            }
            else
            {
                pauseGame();
            }
        }
    }

    private void playSFX(AudioClip clip)
    {
        if (SFXManager.Instance != null && clip != null)
        {
            SFXManager.Instance.PlaySFX(clip, Vector3.zero);
        }
    }

    public void pauseGame()
    {
        playSFX(clickClip);
        if (pauseMenuUI != null)
        {
            if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap("UI");
            }

            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }

    }

    public void resumeGame()
    {
        playSFX(clickClip);
        if (pauseMenuUI != null)
        {
            if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap("Player");
            }
            pauseMenuUI.SetActive(false);
            settingMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    public void Setting()
    {
        playSFX(clickClip);
        if (settingMenu != null)
        {
            settingMenu.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        playSFX(clickClip);
        if (settingMenu != null)
        {
            settingMenu.SetActive(false);
        }
    }

    public void quitGame()
    {
        playSFX(clickClip);
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start Menu");
    }
}
