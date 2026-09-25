using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerData", menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Base Stats")]
    public float maxHP = 100;
    public float baseDMG = 1f;
    public float invulnerabilityTime = 0.5f;
    public int collisionDMG = 20;
    public AudioClip hurtSFX;
    public ParticleSystem DeathParticle;
    public GameObject spawnLaser;
    public AudioClip spawnSFX;

    [Header("Movement Stats")]
    public float moveSpd = 5f;
    public string BulletPlayer = "PlayerBullet";
    public AudioClip Slowmo;
    public AudioClip SlowmoAlready;

    public float pushForce = 25f;
    public float forceRadius = 0.6f;
    public float forceOffset = 1f;
    public float forceDrainRate = 0.4f;
    public float forceRechargeRate = 0.25f;
    public float minForceEnergyToStart = 0.1f;
    public AudioClip pushSFX;

    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashCD = 1f;
    public int dashDMG = 20;
    public float dashHitStop = 0.1f;
    public Material onDamageVFX_Mat;
    public AudioClip dashCrashSFX;
    public AudioClip dashSFX;
    public AudioClip deathSFX;

    [Header("Slow Motion Settings")]
    public float slowMoTimeScale = 0.2f;
    public float slowMoDuration = 4f;
    public float slowMoCooldown = 6f;

}