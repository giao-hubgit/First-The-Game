using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI levelText; // Hiển thị "LV. 10" hoặc "LV. MAX"
    [SerializeField] private TextMeshProUGUI expText;   // Hiển thị "150 / 300"
    [SerializeField] private Image expFillImage;        // Image dùng chế độ Filled

    private void OnEnable()
    {
        LevelSystem.OnLevelUp += UpdateLevelDisplay;
        LevelSystem.OnExpChanged += UpdateExpDisplay;
    }

    private void OnDisable()
    {
        LevelSystem.OnLevelUp -= UpdateLevelDisplay;
        LevelSystem.OnExpChanged -= UpdateExpDisplay;
    }

    private void Start()
    {
        if (LevelSystem.Instance != null)
        {
            UpdateLevelDisplay(LevelSystem.Instance.CurrentLevel);
            UpdateExpDisplay(LevelSystem.Instance.CurrentExp, LevelSystem.Instance.RequiredExp);
        }
    }

    private void UpdateLevelDisplay(int newLevel)
    {
        if (levelText == null) return;

        if (LevelSystem.Instance != null && LevelSystem.Instance.IsMaxLevel)
        {
            levelText.text = "LV. MAX";
        }
        else
        {
            levelText.text = $"LV. {newLevel}";
        }
    }

    private void UpdateExpDisplay(int currentExp, int maxExp)
    {
        bool isMax = LevelSystem.Instance != null && LevelSystem.Instance.IsMaxLevel;

        // Cập nhật tỉ lệ phần trăm của Filled Image (từ 0.0 đến 1.0)
        if (expFillImage != null)
        {
            expFillImage.fillAmount = isMax ? 1f : (float)currentExp / maxExp;
        }

        // Cập nhật Text EXP
        if (expText != null)
        {
            expText.text = isMax ? "MAX" : $"{currentExp} / {maxExp}";
        }
    }
}