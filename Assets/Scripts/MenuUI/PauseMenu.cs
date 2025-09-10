using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public static bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    void Update()
    {
        if (QuizManager.IsQuizActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ResumeGame() => TogglePause();

    public void LoadGame()
    {
        Debug.Log("Returning to Main Menu to load game.");
        ReturnToMainMenu();
    }

    // --- THIS METHOD IS CORRECTED ---
    public void SaveGame()
    {
        SaveData data = new SaveData();

        // Scene
        data.sceneName = SceneManager.GetActiveScene().name;

        // Player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("❌ No Player with tag 'Player' found in the scene!");
            return;
        }
        data.x = player.transform.position.x;
        data.y = player.transform.position.y;
        data.z = player.transform.position.z;

        // Health/Shield
        PlayerHealthShield phs = player.GetComponent<PlayerHealthShield>();
        if (phs == null)
        {
            Debug.LogWarning("⚠️ Player has no PlayerHealthShield component. Health/Shield won't be saved.");
        }
        else
        {
            data.currentHealth = phs.currentHealth;
            data.currentShield = phs.currentShield;
        }

        // Inventory
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("⚠️ No InventoryManager instance found. Inventory won't be saved.");
        }
        else
        {
            data.inventoryItems = InventoryManager.Instance.GetAllItems().Select(i => i.name).ToList();
            Debug.Log($"<color=orange>Saving {data.inventoryItems.Count} items: {string.Join(", ", data.inventoryItems)}</color>");
        }

        // --- THIS BLOCK WAS MOVED ---
        // Quiz Machines
        data.usedQuizMachineIds = new List<string>();
        QuizMachine[] allQuizMachines = FindObjectsOfType<QuizMachine>();
        Debug.Log($"Found {allQuizMachines.Length} quiz machines in the scene to check."); // New Log 1


        foreach (QuizMachine machine in allQuizMachines)
        {
            if (machine.hasBeenUsed)
            {
                data.usedQuizMachineIds.Add(machine.uniqueId);
                // New Log 2: Tells you exactly which machine ID is being saved.
                Debug.Log($"<color=orange>SAVING quiz machine ID: {machine.uniqueId}</color>");
            }
        }
        Debug.Log($"Saving the IDs of {data.usedQuizMachineIds.Count} used quiz machines.");
        // --- END OF MOVED BLOCK ---


        // Save the file AFTER all data has been collected.
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.SaveGame(data);
            Debug.Log("<color=green>Game Saved Successfully!</color>");
        }
        else
        {
            Debug.LogError("❌ GameSaveManager.Instance is NULL! Did you place it in your scene?");
        }
    }

    public void ReturnToMainMenu()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.CloseInventory();
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}