using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public string bulletTag;
    public float bulletForce = 20f;
    public float recoil;
    public int maxAmmo = 30;
    public float fireRate = 0.2f;
    public int burstCount = 1;
    public float spread = 0f;
    public bool isAutomatic;
    public AudioClip shootSFX;
    public Sprite weaponSprite;
}