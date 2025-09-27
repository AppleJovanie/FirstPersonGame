using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // Scene
    public string sceneName;

    // Player position
    public float x, y, z;

    // Player stats
    public int currentHealth;
    public int currentShield;

    // Inventory
    public List<string> inventoryItems;

    // --- ADD THIS LINE ---
    // A list to store the unique IDs of all quiz machines that have been used.
    public List<string> usedQuizMachineIds;

    public List<string> collectedPieceIds;
}