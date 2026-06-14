using System.Collections.Generic;
using UnityEngine;

public class FlameDamage : MonoBehaviour
{
    public int damage = 10;
    public float tickRate = 0.1f;

    private Dictionary<GameObject, float> lastDamageTimes = new Dictionary<GameObject, float>();

    private void OnEnable()
    {
        lastDamageTimes.Clear();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (lastDamageTimes.TryGetValue(other, out float lastTime))
        {
            if (Time.time < lastTime + tickRate)
            {
                return;
            }
        }

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.takeDmg(damage);
            lastDamageTimes[other] = Time.time;
        }
    }
}