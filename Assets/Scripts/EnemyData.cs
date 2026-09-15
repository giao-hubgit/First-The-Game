using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Base Stats")]
    public float maxHP = 100f;
    public float baseDMG = 1f;
    public int collisionDMG = 20;
    public float damageRate = 1f;
    public float aggroRange = 12f;
    public float moveSpeed = 4f;

    [Header("Misc")]
    public string deathParticle = "EnemyDeathParticle";
    public string deathAnimation = "EnemyDeathAnimation";
    public string spawnAnimation = "EnemySpawnAnimation";
    public AudioClip spawnSFX;
    public AudioClip deathSFX;
    public AudioClip crashSFX;
    public string itemDrop = "Pistol Pickup";

    [Header("Ranged Stats")]
    public float visionRange = 8f;
    public LayerMask lineOfSightLayer;
}