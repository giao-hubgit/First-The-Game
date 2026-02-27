using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 100;
    public GameObject deathEffect;
    public Entity_VFX enemyVFX;
    [SerializeField] private ParticleSystem DeathParticle; 

    public void takeDmg(int damage){
        hp -= damage;
        enemyVFX.PlayOnDamageVFX();

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
