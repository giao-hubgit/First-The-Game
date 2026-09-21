using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System;

public class CinematicBarsController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera bossCutsceneCam;
    [SerializeField] private float phaseTransitionDuration = 2f;
    [SerializeField] private Boss boss;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Canvas[] hudCanvasesToHide;

    private void Start()
    {
        if (boss == null)
        {
            boss = FindAnyObjectByType<Boss>();
        }

        if (boss != null)
        {
            SetupBossEvents(boss);
        }
    }

    public void SetupBossEvents(Boss targetBoss)
    {
        if (targetBoss == null) return;

        boss = targetBoss;

        if (bossCutsceneCam != null)
        {
            bossCutsceneCam.Target.TrackingTarget = boss.transform;
        }

        boss.OnBossIntroStarted += PlayIntroCinematic;
        boss.OnBossIntroFinished += StopCinematic;
        boss.OnPhaseChanged += HandlePhaseChanged;
        boss.OnBossDeath += StopCinematic;

        PlayIntroCinematic();
    }

    private void PlayIntroCinematic()
    {
        ToggleHUDs(false);
        CinematicBars.Instance?.Show();
        playerInput.actions.Disable();
        if (bossCutsceneCam != null) bossCutsceneCam.gameObject.SetActive(true);
    }

    private void StopCinematic()
    {
        ToggleHUDs(true);
        CinematicBars.Instance?.Hide();
        playerInput.actions.Enable();
        if (bossCutsceneCam != null) bossCutsceneCam.gameObject.SetActive(false);
    }

    private void HandlePhaseChanged(int newPhase)
    {
        StartCoroutine(PhaseChangeRoutine());
    }

    private void ToggleHUDs(bool isVisible)
    {
        if (hudCanvasesToHide == null) return;

        foreach (Canvas canvas in hudCanvasesToHide)
        {
            if (canvas != null)
            {
                canvas.enabled = isVisible;
            }
        }
    }

    private IEnumerator PhaseChangeRoutine()
    {
        PlayIntroCinematic();

        yield return new WaitForSeconds(phaseTransitionDuration);

        StopCinematic();
    }
}