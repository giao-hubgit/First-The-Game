using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Melee Weapon Data")]
public class WeaponMeleeData : WeaponData
{
    public float reflectForce = 20f;
    public float recoil;
    public float slashSpeed = 0.2f;
    public bool isAutomatic;
    public AudioClip slashSFX;
}