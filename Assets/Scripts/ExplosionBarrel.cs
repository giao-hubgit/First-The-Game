using UnityEngine;

[RequireComponent(typeof(Explode))]
public class ExplosionBarrel : MonoBehaviour, IDamageable
{
    public float health = 100f;
    private int state = 0;

    public GameObject FireUp;
    public GameObject SmokeUp;

    private Explode explodeComponent;

    private void Awake()
    {
        explodeComponent = GetComponent<Explode>();
    }

    public void takeDmg(float dmg)
    {
        if (explodeComponent != null && explodeComponent.IsExploded) return;

        health -= dmg;
        if (health <= 0)
        {
            Boom();
        }
    }

    private void Boom()
    {
        DetachAndDestroyVFX(SmokeUp);
        DetachAndDestroyVFX(FireUp);

        if (explodeComponent != null)
        {
            explodeComponent.DoExplosion();
        }

        Destroy(gameObject);
    }

    private void Update()
    {
        if (health <= 60 && health > 30 && state == 0)
        {
            state = 1;
            if (SmokeUp != null) SmokeUp.SetActive(true);
        }

        if (health <= 30 && state != 2)
        {
            state = 2;
            if (FireUp != null) FireUp.SetActive(true);
        }
    }

    private void DetachAndDestroyVFX(GameObject vfx)
    {
        if (vfx != null && vfx.activeSelf)
        {
            vfx.transform.SetParent(null);

            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
            }

            Destroy(vfx, 2f);
        }
    }
}