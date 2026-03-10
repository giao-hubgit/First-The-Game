using System;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public string brokenPrefab = "CrateShattered";
    public float explosionForce = 5f;
    public int health = 1;

    [SerializeField] AudioClip breakSFX;

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0) Break();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude >= 4f)
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                Break();
            }
        }
    }

    void Break()
    {
        // Tạo ra đối tượng chứa các mảnh
        GameObject brokenObj = ObjectPooler.Instance.SpawnFromPool(brokenPrefab, transform.position, Quaternion.identity);

        // Tạo lực nổ cho từng mảnh
        Rigidbody2D[] fragments = brokenObj.GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D rb in fragments)
        {
            // bay ngẫu nhiên
            Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.5f, 1f));
            rb.AddForce(randomDirection * (explosionForce / 100), ForceMode2D.Impulse);

            // Xoayyyyyy
            rb.AddTorque(UnityEngine.Random.Range(-10f, 10f), ForceMode2D.Impulse);
        }

        SFXManager.Instance?.PlaySFX(breakSFX, transform.position);

        // Xóa cái thùng chính
        Destroy(gameObject);
    }
}