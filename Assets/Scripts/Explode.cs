using UnityEngine;

public class Explode : MonoBehaviour, IDamageable
{
    public float explosionForce = 5f;
    public int health = 100;

    [SerializeField] AudioClip breakSFX;

    public void takeDmg(int dmg)
    {
        health -= dmg;
        if (health <= 0) Boom();
    }

    void Boom()
    {

        SFXManager.Instance?.PlaySFX(breakSFX, transform.position);

        Destroy(gameObject);
    }
}
