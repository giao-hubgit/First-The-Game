using System.Collections;
using UnityEngine;

public class Flamthrower_Trap : MonoBehaviour
{
    public float duration = 2f;
    public float CD = 6f;
    public float tickRate = 0.1f;

    public int damage = 10;
    public float maxFlameLength = 5f;
    public float flameWidth = 1f;
    public float timeToReachMaxLength = 0.5f;

    [SerializeField] AudioClip flamethrowerSFX;
    [SerializeField] ParticleSystem flameParticles;

    private float currentHitboxLength = 0f;

    void Start()
    {
        if (flameParticles != null) flameParticles.Stop();
        StartCoroutine(TrapCycleRoutine());
    }

    private IEnumerator TrapCycleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(CD);

            if (flameParticles != null) flameParticles.Play();
            if (flamethrowerSFX != null) SFXManager.Instance?.PlaySFX(flamethrowerSFX, transform.position);

            float timer = 0f;
            float nextTickTime = 0f;
            currentHitboxLength = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                if (timer < timeToReachMaxLength)
                {
                    currentHitboxLength = Mathf.Lerp(0f, maxFlameLength, timer / timeToReachMaxLength);
                }
                else
                {
                    currentHitboxLength = maxFlameLength;
                }

                if (timer >= nextTickTime)
                {
                    DealDamageInLine(currentHitboxLength);
                    nextTickTime += tickRate;
                }

                yield return null;
            }

            if (flameParticles != null) flameParticles.Stop();
            currentHitboxLength = 0f;
        }
    }

    private void DealDamageInLine(float currentLength)
    {
        Vector2 boxCenter = (Vector2)transform.position + (Vector2)transform.right * (currentLength / 2f);
        Vector2 boxSize = new Vector2(currentLength, flameWidth);

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, transform.eulerAngles.z);

        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.takeDmg(damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Vector2 maxBoxCenter = (Vector2)transform.position + (Vector2)transform.right * (maxFlameLength / 2f);
        Vector2 maxBoxSize = new Vector2(maxFlameLength, flameWidth);
        Gizmos.matrix = Matrix4x4.TRS(maxBoxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, maxBoxSize);

        if (Application.isPlaying)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Vector2 currentBoxCenter = (Vector2)transform.position + (Vector2)transform.right * (currentHitboxLength / 2f);
            Vector2 currentBoxSize = new Vector2(currentHitboxLength, flameWidth);
            Gizmos.matrix = Matrix4x4.TRS(currentBoxCenter, transform.rotation, Vector3.one);
            Gizmos.DrawCube(Vector3.zero, currentBoxSize);
        }
    }
}