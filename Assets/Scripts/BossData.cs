using UnityEngine;

[CreateAssetMenu(fileName = "New Boss Data", menuName = "Data/Boss Data")]
public class BossData : EnemyData
{
    [Header("Boss Timing Settings")]
    [Tooltip("Thời gian Boss đi lại loanh quanh trước khi tung đòn")]
    public float movementDuration = 3f;
    [Tooltip("Thời gian Boss đứng nghỉ/thở sau khi vừa tấn công xong")]
    public float postAttackCooldown = 1.5f;
    [Tooltip("Thời gian Boss khựng lại nhắm bắn/báo hiệu trước khi tung đòn")]
    public float preAttackDelay = 0.2f;
    [Tooltip("Khoảng cách ngắn nhất Boss có thể đi")]
    public float minMoveDistance = 3f;
    [Tooltip("Khoảng cách xa nhất Boss có thể đi")]
    public float maxMoveDistance = 6f;

    [Header("Boss Locomotion & Attacks")]
    public float dashSpeed = 15f;
    public float dashDistance = 8f;
    public int laserDamage = 20;

    [Header("Bullet Settings")]
    public int radialBulletCount = 8;

    [Header("Phase Settings")]
    public float phase2HealthThreshold = 0.5f;
}