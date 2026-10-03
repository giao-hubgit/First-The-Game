using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Currency", menuName = "Currency System/Currency Asset")]
public class CurrencySO : ScriptableObject
{
    [Header("Display Info")]
    public string currencyName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Settings")]
    [Tooltip("Nếu true, tiền này sẽ mất khi Player chết (vd: Gold)")]
    public bool isLostOnDeath = true;

    [Tooltip("Nếu true, tiền này lưu lại vĩnh viễn (vd: Scrap Metal)")]
    public bool isPersistent = false;

    // Event thông báo khi số lượng tiền thay đổi
    public event Action<int> OnValueChange;

    // Giá trị lưu trong bộ nhớ tạm (Run-time)
    [NonSerialized] private int _currentAmount;

    public int CurrentAmount => _currentAmount;

    public void SetAmount(int amount)
    {
        _currentAmount = Mathf.Max(0, amount);
        OnValueChange?.Invoke(_currentAmount);
    }

    public void AddAmount(int amount)
    {
        if (amount <= 0) return;
        _currentAmount += amount;
        OnValueChange?.Invoke(_currentAmount);
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0 || _currentAmount < amount) return false;
        
        _currentAmount -= amount;
        OnValueChange?.Invoke(_currentAmount);
        return true;
    }

    public void ResetRuntime()
    {
        if (isLostOnDeath)
        {
            SetAmount(0);
        }
    }
}