using UnityEngine;

public class BulletCollides : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f); 
    }

    public GameObject hitEffect;
    public int damage = 20;

    void OnTriggerEnter2D(Collider2D hitInfo){
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Enemy enemy = hitInfo.GetComponent<Enemy>();
        if (enemy != null){
            enemy.takeDmg(damage);
        }
        Destroy(effect, 0.2f);
        Destroy(gameObject);
    }
}