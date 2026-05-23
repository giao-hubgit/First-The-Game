using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerData", menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    public int maxHP = 100;
    public float invulnerabilityTime = 0.5f;
    public int collisionDMG = 20;
    public GameObject deathEffect;
    public AudioClip hurtVFX;
    public ParticleSystem DeathParticle;
}