using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SaveFileName = "savegame.json";
    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);
    private static string TempSavePath => SavePath + ".tmp";

    private static readonly string EncryptionKey = "key";

    private static string EncryptDecrypt(string data)
    {
        char[] result = new char[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            result[i] = (char)(data[i] ^ EncryptionKey[i % EncryptionKey.Length]);
        }
        return new string(result);
    }

    public static void Save(GameData data)
    {
        try
        {
            data.lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string json = JsonUtility.ToJson(data, true);

            // 1. Ghi ra file tạm thời trước
            File.WriteAllText(TempSavePath, json);

            // 2. Nếu ghi thành công, ghi đè thay thế file Save chính
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }
            File.Move(TempSavePath, SavePath);

            Debug.Log($"[SaveSystem] Saved successfully to: {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save data: {e.Message}");
        }
    }

    public static GameData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[SaveSystem] Save file not found. Creating new GameData.");
            return new GameData();
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            return data ?? new GameData();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to load data (Corrupted?): {e.Message}");
            return new GameData(); // Tạo mới nếu file lỗi
        }
    }

    public static void DeleteSaveData()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
        if (File.Exists(TempSavePath)) File.Delete(TempSavePath);
    }
}