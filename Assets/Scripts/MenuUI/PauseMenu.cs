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

    public void SaveGame()
    {
        SaveData data = new SaveData();

        // 1. Scene
        data.sceneName = SceneManager.GetActiveScene().name;

        // 2. Player Position, Health & Shield
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("❌ No Player with tag 'Player' found in the scene!");
            return;
        }
        data.x = player.transform.position.x;
        data.y = player.transform.position.y;
        data.z = player.transform.position.z;

        PlayerHealthShield phs = player.GetComponent<PlayerHealthShield>();
        if (phs != null)
        {
            data.currentHealth = phs.currentHealth;
            data.currentShield = phs.currentShield;
        }

        // 3. Inventory
        if (InventoryManager.Instance != null)
        {
            data.inventoryItems = InventoryManager.Instance.GetAllItems().Select(i => i.name).ToList();
        }

        // 4. Used Quiz Machines
        data.usedQuizMachineIds = new List<string>();
        QuizMachine[] allQuizMachines = FindObjectsOfType<QuizMachine>();
        foreach (QuizMachine machine in allQuizMachines)
        {
            if (machine.hasBeenUsed)
            {
                data.usedQuizMachineIds.Add(machine.uniqueId);
            }
        }

        // 5. Collected Puzzle Pieces
        FourPiecePuzzleController puzzleController = FindObjectOfType<FourPiecePuzzleController>();
        if (puzzleController != null)
        {
            data.collectedPieceIds = puzzleController.GetCollectedPieceIds();
        }

        // 6. Save all collected data to a file
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
        Debug.Log("Game Exited.");
    }
}