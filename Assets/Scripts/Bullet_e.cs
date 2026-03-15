using UnityEngine;
using UnityEngine.Pool;

public class Bullet_e : MonoBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {

        if (hitInfo.TryGetComponent<IDamageable>(out IDamageable damageable) && !hitInfo.gameObject.CompareTag("Enemy"))
        {
            damageable.takeDmg(damage);
        }

        gameObject.SetActive(false);
    }
}