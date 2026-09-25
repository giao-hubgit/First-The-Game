using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject settingMenu;
    public GameObject LoadingScreen;
    public UnityEngine.UI.Image LoadingCube;
    public AudioClip clickClip;

    private void playSFX(AudioClip clip)
    {
        if (SFXManager.Instance != null && clip != null)
        {
            SFXManager.Instance.PlaySFX(clip, Vector3.zero);
        }
    }

    public void startGame(int sceneID)
    {
        playSFX(clickClip);
        SceneManager.LoadSceneAsync(sceneID);
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
