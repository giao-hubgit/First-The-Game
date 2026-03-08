using UnityEngine;

public class Breakable : MonoBehaviour
{
    public string brokenPrefab = "CrateShattered"; // Kéo Prefab "Thùng vỡ" vào đây
    public float explosionForce = 5f;
    public int health = 1;

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0) Break();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            Break();
        }
    }

    void Break()
    {
        // 1. Tạo ra đối tượng chứa các mảnh vỡ
        GameObject brokenObj = ObjectPooler.Instance.SpawnFromPool(brokenPrefab, transform.position, Quaternion.identity);

        // 2. Tạo lực nổ cho từng mảnh con
        Rigidbody2D[] fragments = brokenObj.GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D rb in fragments)
        {
            // Tạo hướng bay ngẫu nhiên cho mảnh vỡ
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f));
            rb.AddForce(randomDirection * (explosionForce / 100), ForceMode2D.Impulse);

            // Thêm chút xoay cho tự nhiên
            rb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
        }

        // 3. Phát âm thanh vỡ (Dùng SFXManager đã làm)
        // SFXManager.Instance?.PlaySFX(breakClip, transform.position);

        // 4. Xóa cái thùng chính
        Destroy(gameObject);
    }
}