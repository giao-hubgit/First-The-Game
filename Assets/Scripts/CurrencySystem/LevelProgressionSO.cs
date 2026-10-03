using UnityEngine;

[CreateAssetMenu(fileName = "LevelProgressionConfig", menuName = "Currency System/Level Progression Config")]
public class LevelProgressionSO : ScriptableObject
{
    [Header("Level Cap")]
    [Tooltip("Cấp độ tối đa nhân vật có thể đạt được")]
    public int maxLevel = 50;

    [Header("Config Mode")]
    public bool useAnimationCurve = false;

    [Header("Formula Mode (Mũ)")]
    [SerializeField] private int baseExp = 100;
    [SerializeField] private float expMultiplier = 1.25f;

    [Header("Custom Curve Mode")]
    [Tooltip("Trục X: Level, Trục Y: EXP cần để up cấp")]
    [SerializeField] private AnimationCurve expCurve = AnimationCurve.Linear(1, 100, 100, 10000);

    public int GetRequiredExpForLevel(int level)
    {
        if (useAnimationCurve)
        {
            return Mathf.RoundToInt(expCurve.Evaluate(level));
        }

        return Mathf.RoundToInt(baseExp * Mathf.Pow(expMultiplier, level - 1));
    }
}