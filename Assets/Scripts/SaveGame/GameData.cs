using System;
using System.Collections.Generic;

[Serializable]
public struct CurrencySaveData
{
    public string currencyID; // Lưu tên file ScriptableObject (vd: "ScrapMetal", "Gem")
    public int amount;
}

[Serializable]
public class GameData
{
    // Danh sách lưu trữ các loại tiền vĩnh viễn
    public List<CurrencySaveData> savedCurrencies = new List<CurrencySaveData>();

    // Các dữ liệu khác...
    public List<string> purchasedUpgradeIDs = new List<string>();
    public string lastSaveTime;

    public GameData()
    {
        savedCurrencies = new List<CurrencySaveData>();
        purchasedUpgradeIDs = new List<string>();
    }
}