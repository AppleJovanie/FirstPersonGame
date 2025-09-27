using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }

    // Keep last loaded save in memory (optional)
    private SaveData lastLoadedData;

    // Track quiz machine states
    public HashSet<string> usedQuizMachineIds = new HashSet<string>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
        lastLoadedData = data;
    }

    public SaveData LoadGame()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        lastLoadedData = data;

        // Rebuild runtime machine states
        usedQuizMachineIds = new HashSet<string>(data.usedQuizMachineIds);
        return data;
    }

    public bool HasSave()
    {
        return File.Exists(Application.persistentDataPath + "/save.json");
    }

    public void ClearSave()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        // Reset runtime state
        lastLoadedData = null;
        usedQuizMachineIds.Clear();

        Debug.Log("🗑 Save data fully cleared (file + memory).");
    }
}
