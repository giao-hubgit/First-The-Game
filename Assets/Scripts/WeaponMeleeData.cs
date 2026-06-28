using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Melee Weapon Data")]
public class WeaponMeleeData : WeaponData
{
    public int damage = 10;
    public float size = 1f;
    public float radius = 1f;

    [Header("Spawn Settings")]
    public Vector2 spawnOffset = Vector2.zero;
    public float fadeInDuration = 0.1f;
    public float delayStart = 0f;

    [Header("Destroy Settings")]
    public float fadeOutDuration = 0.1f;
    public float detachTime = 0.5f;

    [Header("Combat Stats")]
    public float reflectForce = 20f;
    public float knockback = 20f;
    public float recoil;
    public float hitImpact = 0.15f;
    public float startSpeed = 5f;
    public float endSpeed = 5f;
    public float cooldown = 0.5f;
    public bool isAutomatic;
    public bool vulnerabilityIgnore = false;
    public string reflectedBullet = "PlayerBullet";
    public AudioClip slashSFX;
    public AudioClip hitSFX;
    public LayerMask HitTarget;

    [Header("Attack Type: Thrust")]
    public bool isThrust;
    public float thrustStartAngle = -45f;
    public float thrustEndAngle = -45f;

    [Header("Attack Type: Swing")]
    public float swingStartAngle = -90f;
    public float swingEndAngle = 90f;
}