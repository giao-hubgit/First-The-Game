using UnityEngine;
using System.Collections;

public class Gun_Trap : MonoBehaviour
{
    public Transform firePoint;
    public string bulletPrefab = "TrapBullet";
    public float bulletForce = 20f;
    public float duration = 2f;
    public float CD = 8f;
    public float tickRate = 0.1f;
    [SerializeField] private AudioClip bulletSFXClip;
    [SerializeField] Animator anim;

    void OnEnable()
    {
        StartCoroutine(TrapCycleRoutine());
    }

    private IEnumerator TrapCycleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(CD);

            float timer = 0f;
            float nextTickTime = 0f;
            if (anim != null) anim.SetBool("isShooting", true);

            while (timer < duration)
            {
                timer += Time.deltaTime;

                if (timer >= nextTickTime)
                {
                    SFXManager.Instance?.PlaySFX(bulletSFXClip, transform.position, 0.3f, true, 0.5f, 0.8f);

                    Vector3 randomOffset = firePoint.right * Random.Range(-0.15f, 0.15f);
                    Vector3 spawnPosition = firePoint.position + randomOffset;

                    GameObject bullet = ObjectPooler.Instance.SpawnFromPool(bulletPrefab, spawnPosition, firePoint.rotation);

                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                    rb.linearVelocity = Vector2.zero;
                    rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

                    nextTickTime += tickRate;
                }

                yield return null;
            }

            if (anim != null) anim.SetBool("isShooting", false);
        }
    }
}
