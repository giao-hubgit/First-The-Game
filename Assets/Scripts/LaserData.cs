using UnityEngine;

[CreateAssetMenu(fileName = "NewLaserData", menuName = "Projectiles/Laser Data")]
public class LaserData : ScriptableObject
{
    [Header("Damage Settings")]
    public float damage = 10f;
    public float damageTick = 0.2f;
    public string ignoreTag = "Enemy";
    public LayerMask obstacleLayer;

    [Header("Visual Settings")]
    public float length = 20f;
    public float width = 1f;
    public string particlePoolName = "None";
    public float fadeDuration = 0.2f;

    [Header("Movement Settings")]
    [Tooltip("Góc bắt đầu quay (Độ lệch tương đối so với hướng của Parent)")]
    public float startAngleOffset = -90f;

    [Tooltip("Góc kết thúc quay (Độ lệch tương đối so với hướng của Parent)")]
    public float endAngleOffset = 90f;

    [Tooltip("Tốc độ quay của tia laser (độ/giây)")]
    public float rotationSpeed = 90f;

    [Header("Audio")]
    public AudioClip laserSFX;
}