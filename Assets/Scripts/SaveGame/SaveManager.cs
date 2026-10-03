using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public GameData CurrentData { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        // Tự động tìm tất cả Script implement ISaveable trong Scene
        var saveables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                            .OfType<ISaveable>();

        foreach (var saveable in saveables)
        {
            saveable.PopulateSaveData(CurrentData);
        }

        SaveSystem.Save(CurrentData);
    }

    public void LoadGame()
    {
        CurrentData = SaveSystem.Load();

        var saveables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                            .OfType<ISaveable>();

        foreach (var saveable in saveables)
        {
            saveable.LoadFromSaveData(CurrentData);
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame(); // Tự động Save khi người chơi thoát app
    }
}