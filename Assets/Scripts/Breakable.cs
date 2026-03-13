using System;
using UnityEngine;

public class Breakable : MonoBehaviour, IDamageable
{
    public string brokenPrefab = "CrateShattered";
    public float explosionForce = 5f;
    public int health = 100;

    [SerializeField] AudioClip breakSFX;

    public void takeDmg(int dmg)
    {
        health -= dmg;
        if (health <= 0) Break(transform.position);
    }

    public void Break(Vector2 explosionSource)
    {
        GameObject brokenObj = ObjectPooler.Instance.SpawnFromPool(brokenPrefab, transform.position, Quaternion.identity);
        Rigidbody2D[] fragments = brokenObj.GetComponentsInChildren<Rigidbody2D>();

        foreach (Rigidbody2D rb in fragments)
        {
            Vector2 direction = (Vector2)rb.transform.position - explosionSource;

            if (direction == Vector2.zero)
                direction = UnityEngine.Random.insideUnitCircle;

            rb.AddForce(direction.normalized * explosionForce, ForceMode2D.Impulse);

            rb.AddTorque(UnityEngine.Random.Range(-10f, 10f), ForceMode2D.Impulse);
        }

        SFXManager.Instance?.PlaySFX(breakSFX, transform.position);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude >= 4f)
        {
            if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            {
                Break(collision.transform.position);
            }
        }
    }
}
