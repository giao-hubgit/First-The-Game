using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    [Header("Data Link")]
    [SerializeField] private CurrencySO targetCurrency;

    [Header("UI Bindings")]
    [SerializeField] private Image currencyIcon;
    [SerializeField] private TextMeshProUGUI amountText;

    private void OnEnable()
    {
        if (targetCurrency != null)
        {
            // Lắng nghe sự kiện thay đổi tiền trực tiếp từ ScriptableObject
            targetCurrency.OnValueChange += UpdateUI;
            UpdateUI(targetCurrency.CurrentAmount);
        }
    }

    private void OnDisable()
    {
        if (targetCurrency != null)
        {
            targetCurrency.OnValueChange -= UpdateUI;
        }
    }

    private void Start()
    {
        if (targetCurrency != null && currencyIcon != null)
        {
            currencyIcon.sprite = targetCurrency.icon;
        }
    }

    private void UpdateUI(int newAmount)
    {
        if (amountText != null)
        {
            amountText.text = newAmount.ToString("N0"); // Định dạng số (vd: 1,000)
        }
    }
}