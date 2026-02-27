using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int HP = 100;
    public int CollisionDMG = 20;

    public GameObject deathEffect;
    public Entity_VFX enemyVFX;
    [SerializeField] private ParticleSystem DeathParticle; 

    public void takeDmg(int damage){
        HP -= damage;

        if (enemyVFX != null) {
            enemyVFX.PlayOnDamageVFX();
        }

        if (HP <= 0){
            Die();
        }
    }

    protected virtual void Die(){
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

    protected virtual void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Player")){
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null && playerMovement != null){
                if (playerMovement.isDashing != true){
                    player.takeDmg(CollisionDMG);
                }
            }
        }
    }
}
