using System.Collections;
using UnityEngine;

public class Flamthrower_Trap : MonoBehaviour
{
    public float duration = 2f;
    public float CD = 6f;

    [SerializeField] AudioClip flamethrowerSFX;
    [SerializeField] ParticleSystem flameParticles;
    [SerializeField] GameObject NavMeshHitBox;

    void OnEnable()
    {
        if (flameParticles != null) flameParticles.Stop();
        if (NavMeshHitBox != null) NavMeshHitBox.SetActive(false);
        StartCoroutine(TrapCycleRoutine());
    }

    private IEnumerator TrapCycleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(CD);

            if (flameParticles != null) flameParticles.Play();
            if (flamethrowerSFX != null) SFXManager.Instance?.PlaySFX(flamethrowerSFX, transform.position, 0.2f);
            if (NavMeshHitBox != null) NavMeshHitBox.SetActive(true);

            yield return new WaitForSeconds(duration);

            if (flameParticles != null) flameParticles.Stop();
            if (NavMeshHitBox != null) NavMeshHitBox.SetActive(false);
        }
    }
}