using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    public int hp = 100;
    public GameObject deathEffect;

    public void takeDmg(int damage){
        hp -= damage;

        if (hp <= 0){
            Die();
        }
    }

    void Die(){
        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(effect, 1f);
    }
}
