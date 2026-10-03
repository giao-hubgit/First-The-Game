using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Image loadingCube;
    [SerializeField] private RoomTemplate mapGenerator;
    [SerializeField] private Player player;

    [Header("Misc Settings after loading")]
    [SerializeField] private GameObject spawnCamera;
    [SerializeField] private string areaTitleText;

    private void Start()
    {
        StartCoroutine(LoadingRoutine());
    }

    private IEnumerator LoadingRoutine()
    {
        player.gameObject.SetActive(false);
        spawnCamera.SetActive(true);
        loadingCanvas.SetActive(true);
        AudioListener.pause = true;

        while (!mapGenerator.GenerationFinished || !mapGenerator.NavmeshFinished)
        {
            loadingCube.rectTransform.Rotate(0f, 0f, -50f * Time.unscaledDeltaTime);

            yield return null;
        }

        yield return new WaitUntil(() => mapGenerator.GenerationFinished == true);

        yield return new WaitUntil(() => mapGenerator.NavmeshFinished == true);

        yield return new WaitForSecondsRealtime(0.5f);

        loadingCanvas.SetActive(false);
        spawnCamera.SetActive(false);
        AudioListener.pause = false;
        player.gameObject.SetActive(true);

        TitleController.Instance.SpawnTitle(areaTitleText, Color.white, fadeInDuration: 0.5f, holdDuration: 2f, fadeOutDuration: 1f, delayBeforeSpawn: 2.5f);
    }
}
