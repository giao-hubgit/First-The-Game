using System;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance { get; private set; }

    [Header("Data Asset")]
    [SerializeField] private LevelProgressionSO progressionConfig;

    public static event Action<int> OnLevelUp;
    public static event Action<int, int> OnExpChanged; // (CurrentExp, MaxExp)

    public int CurrentLevel { get; private set; } = 1;
    public int CurrentExp { get; private set; } = 0;

    public int MaxLevel => progressionConfig != null ? progressionConfig.maxLevel : 99;
    public bool IsMaxLevel => CurrentLevel >= MaxLevel;
    public int RequiredExp => progressionConfig.GetRequiredExpForLevel(CurrentLevel);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddExp(int amount)
    {
        // Nếu đã Max Level hoặc amount <= 0 thì không cộng EXP
        if (amount <= 0 || IsMaxLevel) return;

        CurrentExp += amount;

        while (CurrentExp >= RequiredExp && !IsMaxLevel)
        {
            CurrentExp -= RequiredExp;
            CurrentLevel++;

            OnLevelUp?.Invoke(CurrentLevel);

            // Nếu vừa đạt Max Level thì dừng vòng lặp và giữ EXP max
            if (IsMaxLevel)
            {
                CurrentExp = RequiredExp;
                break;
            }
        }

        OnExpChanged?.Invoke(CurrentExp, RequiredExp);
    }

    public void ResetLevelProgress()
    {
        CurrentLevel = 1;
        CurrentExp = 0;
        OnExpChanged?.Invoke(CurrentExp, RequiredExp);
    }
}