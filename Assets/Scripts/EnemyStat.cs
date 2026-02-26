using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    public int hp = 100;
    public GameObject deathEffect;
    [SerializeField] private ParticleSystem DeathParticle; 

    public void takeDmg(int damage){
        hp -= damage;

        if (hp <= 0){
            Die();
        }
    }

    void Die(){
        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);

        ParticleSystem particle = Instantiate(DeathParticle, transform.position, Quaternion.identity);
        particle.Play();

        Destroy(gameObject);
        Destroy(effect, 1f);
        Destroy(particle, 2f);
    }
}
