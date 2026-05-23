using UnityEngine;
using UnityEngine.Pool;

public class bullet_trap : MonoBehaviour
{
    public BulletData data;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {

        if (hitInfo.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.takeDmg(data.damage);
        }

        gameObject.SetActive(false);
    }
}