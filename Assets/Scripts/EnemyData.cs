using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Base Stats")]
    public int maxHP = 100;
    public int collisionDMG = 20;
    public float damageRate = 1f;
    public float aggroRange = 12f;
    public float moveSpeed = 4f;

    [Header("Misc")]
    public string deathParticle = "EnemyDeathParticle";
    public string deathAnimation = "EnemyDeathAnimation";
    public AudioClip deathSFX;
    public AudioClip crashSFX;
    public string itemDrop = "Pistol Pickup";

    [Header("Ranged Stats")]
    public string bulletPrefabS = "EnemyBullet";
    public float bulletForce = 10f;
    public AudioClip bulletSFXClip;
    public float visionRange = 8f;
    public float fireRate = 1.5f;
    public LayerMask lineOfSightLayer;

    [Header("Shotgun Stats")]
    public int maxBullets = 5;
    public float spreadAngle = 45f;

    [Header("Machine Gun Stats")]
    public int bulletsPerBurst = 3;
    public float timeBetweenBullets = 0.1f;
    public float bulletSpread = 10f;
}