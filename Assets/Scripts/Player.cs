using UnityEngine;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public GameObject deathEffect;
    public Entity_VFX playerVFX;
    [SerializeField] private ParticleSystem DeathParticle; 

    public void takeDmg(int damage){
        HP -= damage;
        if (playerVFX != null){
            playerVFX.PlayOnDamageVFX();
        }

        if (HP <= 0){
            Die();
        }
    }

    private void Die(){
        if (deathEffect != null) {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        if (DeathParticle != null) {
            ParticleSystem particle = Instantiate(DeathParticle, transform.position, Quaternion.identity);
            particle.Play();
            Destroy(particle.gameObject, 2f);
        }

        Destroy(gameObject);
    }
}
