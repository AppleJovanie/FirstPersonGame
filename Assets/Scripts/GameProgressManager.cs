using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    // This list will store the DoorTypes of the paths the player has completed.
    private List<DoorType> completedDoors = new List<DoorType>();

    private void Awake()
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

    public static void CompleteDoor(DoorType door)
    {
        if (Instance != null && !Instance.completedDoors.Contains(door))
        {
            Instance.completedDoors.Add(door);
        }
    }

    public static bool IsDoorCompleted(DoorType door)
    {
        return Instance != null && Instance.completedDoors.Contains(door);
    }

    // --- NEW METHODS FOR SAVE/LOAD ---

    /// <summary>
    /// Returns the list of completed doors for saving.
    /// </summary>
    public List<DoorType> GetCompletedDoorsForSave()
    {
        return completedDoors;
    }

    /// <summary>
    /// Loads the list of completed doors from a save file.
    /// </summary>
    public void LoadProgressFromSave(List<DoorType> loadedDoors)
    {
        if (loadedDoors != null)
        {
            completedDoors = loadedDoors;
        }
    }
}

