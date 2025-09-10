using UnityEngine;
using System.IO;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }

    void Awake()
    {
        // This checks if another GameSaveManager already exists.
        if (Instance != null && Instance != this)
        {
            // If one does exist, this new one is a duplicate, so it destroys itself.
            // This does NOT destroy the original manager.
            Destroy(gameObject);
        }
        else
        {
            // If this is the first and only instance, it registers itself.
            Instance = this;

            // This is the crucial line that prevents it from being destroyed on scene load.
            DontDestroyOnLoad(gameObject);
        }
    }

    // --- The rest of your script is perfectly fine ---

    public void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
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
    }
}