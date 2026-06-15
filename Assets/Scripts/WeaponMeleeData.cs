using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Melee Weapon Data")]
public class WeaponMeleeData : WeaponData
{
    public int damage = 10;
    public float size = 1f;
    public float radius = 1f;
    public float offset = 1f;
    public float reflectForce = 20f;
    public float knockback = 20f;
    public float recoil;
    public float hitImpact = 0.15f;
    public float slashSpeed = 0.2f;
    public float cooldown = 0.5f;
    public bool isAutomatic;
    public string reflectedBullet = "PlayerBullet";
    public AudioClip slashSFX;
    public AudioClip hitSFX;
    public LayerMask HitTarget;

    [Header("Attack Type")]
    public bool isThrust;
}