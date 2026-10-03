using System.Collections.Generic;
using UnityEngine;

public class PlayerWallet : MonoBehaviour, ISaveable
{
    public static PlayerWallet Instance { get; private set; }

    [Header("Registered Currencies")]
    [Tooltip("Kéo tất cả các file CurrencySO (Gold, ScrapMetal, Gem...) vào đây")]
    [SerializeField] private List<CurrencySO> registeredCurrencies = new List<CurrencySO>();

    private void Awake()
    {
        // 1. Sửa lỗi Singleton: Kiểm tra và Return NGAY LẬP TỨC nếu đã có Instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ==========================================
    // IMPLEMENT INTERFACE ISAVEABLE
    // ==========================================

    public void PopulateSaveData(GameData data)
    {
        data.savedCurrencies.Clear();

        foreach (var currency in registeredCurrencies)
        {
            // Chỉ lưu những loại tiền đánh dấu là vĩnh viễn (isPersistent = true)
            if (currency != null && currency.isPersistent)
            {
                data.savedCurrencies.Add(new CurrencySaveData
                {
                    currencyID = currency.name,
                    amount = currency.CurrentAmount
                });
            }
        }
    }

    public void LoadFromSaveData(GameData data)
    {
        foreach (var currency in registeredCurrencies)
        {
            if (currency == null) continue;

            if (currency.isPersistent)
            {
                // Tìm dữ liệu đã lưu tương ứng với ID/Tên của CurrencySO
                var saved = data.savedCurrencies.Find(x => x.currencyID == currency.name);

                // Nếu tìm thấy thì nạp số tiền đó, nếu chưa có (file save mới) thì mặc định 0
                currency.SetAmount(saved.amount);
            }
            else
            {
                // Tiền tạm thời trong Dungeon (Gold, EXP...) luôn reset về 0 khi nạp game
                currency.SetAmount(0);
            }
        }
    }

    // ==========================================
    // GAMEPLAY LOGIC
    // ==========================================

    /// <summary>
    /// Gọi hàm này khi Player chết hoặc kết thúc Run trong Dungeon
    /// </summary>
    public void OnPlayerDeath()
    {
        // 1. Reset các loại tiền mất khi chết (vd: Gold)
        foreach (var currency in registeredCurrencies)
        {
            if (currency != null && currency.isLostOnDeath)
            {
                currency.ResetRuntime();
            }
        }

        // 2. Kích hoạt SaveGame để lưu số Scrap Metal thu thập được vào ổ đĩa
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }
    }

    /// <summary>
    /// Hàm tiện ích để lấy ScriptableObject theo tên nếu cần gọi từ Script khác
    /// </summary>
    public CurrencySO GetCurrency(string currencyName)
    {
        return registeredCurrencies.Find(c => c != null && c.name == currencyName);
    }

    public void AddCurrency(string currencyName, int amount)
    {
        CurrencySO target = GetCurrency(currencyName);
        if (target != null)
        {
            target.AddAmount(amount);
        }
    }

    public bool TrySpendCurrency(string currencyName, int amount)
    {
        CurrencySO target = GetCurrency(currencyName);
        return target != null && target.TrySpend(amount);
    }
}