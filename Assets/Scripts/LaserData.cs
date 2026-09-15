using UnityEngine;

[CreateAssetMenu(fileName = "NewLaserData", menuName = "Projectiles/Laser Data")]
public class LaserData : ScriptableObject
{
    [Header("Damage Settings")]
    public float damage = 10f;
    public float damageTick = 0.2f;

    [Header("Visual Settings")]
    public float length = 20f;
    public float width = 1f;
    public string particlePoolName = "None";

    [Header("Movement Settings")]
    [Tooltip("Góc quay")]
    public float rotationAngle = 180f;
    public float startAngleOffset = -90f;

    [Tooltip("Tốc độ quay của tia laser (độ/giây)")]
    public float rotationSpeed = 90f;

    [Header("Audio")]
    public AudioClip laserSFX;
}