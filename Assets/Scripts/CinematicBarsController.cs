using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System;

public class CinematicBarsController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera bossCutsceneCam;
    [SerializeField] private Boss boss;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Canvas[] hudCanvasesToHide;

    private Coroutine activeCinematicRoutine;

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

    private void OnDestroy()
    {
        UnsubscribeBossEvents(boss);
    }

    public void SetupBossEvents(Boss targetBoss)
    {
        if (targetBoss == null) return;

        if (boss != null && boss != targetBoss)
        {
            UnsubscribeBossEvents(boss);
        }

        boss = targetBoss;

        if (bossCutsceneCam != null)
        {
            bossCutsceneCam.Target.TrackingTarget = boss.transform;
        }

        boss.OnBossIntroStarted += PlayIntroCinematic;
        boss.OnBossIntroFinished += StopCinematic;
        boss.OnPhaseChanged += HandlePhaseChanged;
        boss.OnBossDeath += HandleDeathCinematic;

        PlayIntroCinematic();
    }

    private void UnsubscribeBossEvents(Boss targetBoss)
    {
        if (targetBoss == null) return;

        targetBoss.OnBossIntroStarted -= PlayIntroCinematic;
        targetBoss.OnBossIntroFinished -= StopCinematic;
        targetBoss.OnPhaseChanged -= HandlePhaseChanged;
        targetBoss.OnBossDeath -= HandleDeathCinematic;
    }

    private void PlayIntroCinematic()
    {
        ToggleHUDs(false);
        CinematicBars.Instance?.Show();
        //playerInput?.actions?.Disable();
        if (bossCutsceneCam != null) bossCutsceneCam.gameObject.SetActive(true);
    }

    private void StopCinematic()
    {
        ToggleHUDs(true);
        CinematicBars.Instance?.Hide();
        //playerInput?.actions?.Enable();
        if (bossCutsceneCam != null) bossCutsceneCam.gameObject.SetActive(false);
    }

    private void FocusCamera()
    {
        if (bossCutsceneCam != null) bossCutsceneCam.gameObject.SetActive(true);
    }

    private void StopFocusCamera()
    {
        if (bossCutsceneCam != null) bossCutsceneCam.gameObject.SetActive(false);
    }

    private void HandlePhaseChanged(int newPhase)
    {
        if (boss == null) return;

        if (activeCinematicRoutine != null) StopCoroutine(activeCinematicRoutine);
        activeCinematicRoutine = StartCoroutine(PhaseChangeRoutine(boss.PhaseTransitionDuration));
    }

    private void HandleDeathCinematic()
    {
        if (boss == null) return;

        if (activeCinematicRoutine != null) StopCoroutine(activeCinematicRoutine);
        activeCinematicRoutine = StartCoroutine(DeathRoutine(boss.DeathDuration));
    }

    private IEnumerator PhaseChangeRoutine(float duration)
    {
        PlayIntroCinematic();

        yield return new WaitForSeconds(duration);

        StopCinematic();
    }

    private IEnumerator DeathRoutine(float duration)
    {
        FocusCamera();

        yield return new WaitForSeconds(duration);

        StopFocusCamera();
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
}