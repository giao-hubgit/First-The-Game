using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet", menuName = "Projectiles/Bullet Data")]
public class BulletData : ScriptableObject
{
    public int damage;
    public int knockback;
    public string bulletHitVFX;
}
