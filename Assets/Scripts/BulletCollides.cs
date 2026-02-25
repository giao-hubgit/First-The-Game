using UnityEngine;

public class BulletCollides : MonoBehaviour
{
    public GameObject hitEffect;
    public int damage = 20;

    void OnTriggerEnter2D(Collider2D hitInfo){
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        EnemyStat enemy = hitInfo.GetComponent<EnemyStat>();
        if (enemy != null){
            enemy.takeDmg(damage);
        }
        Destroy(effect, 0.2f);
        Destroy(gameObject);
    }
}