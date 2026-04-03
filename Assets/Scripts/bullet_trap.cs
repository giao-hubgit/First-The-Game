using UnityEngine;
using UnityEngine.Pool;

public class bullet_trap : MonoBehaviour
{
    public int damage = 5;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {

        if (hitInfo.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.takeDmg(damage);
        }

        gameObject.SetActive(false);
    }
}