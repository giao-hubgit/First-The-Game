using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerData", menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Base Stats")]
    public int maxHP = 100;
    public float invulnerabilityTime = 0.5f;
    public int collisionDMG = 20;
    public GameObject deathEffect;
    public AudioClip hurtVFX;
    public ParticleSystem DeathParticle;

    [Header("Movement Stats")]
    public float moveSpd = 5f;
    public string BulletPlayer = "PlayerBullet";
    public AudioClip Slowmo;
    public AudioClip SlowmoAlready;

    public float pushForce = 25f;
    public float pushRadius = 0.6f;
    public float pushOffset = 1f;
    public float pushCD = 0.5f;
    public AudioClip pushSFX;

    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashCD = 1f;
    public int dashDMG = 20;
    public AudioClip dashCrashSFX;
    public AudioClip dashSFX;

    [Header("Slow Motion Settings")]
    public float slowMoTimeScale = 0.2f;
    public float slowMoDuration = 4f;
    public float slowMoCooldown = 6f;

}