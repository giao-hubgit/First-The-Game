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
    public float recoil;
    public float slashSpeed = 0.2f;
    public float cooldown = 0.5f;
    public bool isAutomatic;
    public AudioClip slashSFX;
    public AudioClip hitSFX;
    public LayerMask HitTarget;
}