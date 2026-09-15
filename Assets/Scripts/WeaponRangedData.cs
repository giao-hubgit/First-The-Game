using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Ranged Weapon Data")]
public class WeaponRangedData : WeaponData
{
    [Header("--- GENERAL STATS ---")]
    public string bulletTag;
    public float bulletForce = 20f;
    public float fireRate = 1.5f;
    public int burstCount = 3;
    public float timeBetweenBullets = 0.1f;
    public float spread = 0f;
    public AudioClip shootSFX;

    [Header("--- PLAYER ONLY STATS ---")]
    public bool isAutomatic;
    public int maxAmmo = 30;
    public float recoil;
    public float shakeForce;
}