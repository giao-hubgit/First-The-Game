using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingMenu;
    public AudioClip clickClip;

    private void playSFX(AudioClip clip)
    {
        if (SFXManager.Instance != null && clip != null)
        {
            SFXManager.Instance.PlaySFX(clip, Vector3.zero);
        }
    }

    public void startGame()
    {
        playSFX(clickClip);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
        Application.Quit();
    }
}
