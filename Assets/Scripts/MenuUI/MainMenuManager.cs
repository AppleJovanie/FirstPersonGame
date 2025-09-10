using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene to Load")]
    public string newGameSceneName; // Assign in Inspector

    [Header("UI Panels")]
    public GameObject settingsPanel;

    [Header("UI Buttons")]
    public GameObject loadGameButton;

    void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (loadGameButton != null)
            loadGameButton.SetActive(GameSaveManager.Instance.HasSave());
    }

    public void OnNewGameButton()
    {
        if (!string.IsNullOrEmpty(newGameSceneName))
        {
            if (InventoryManager.Instance != null)
            {
                Destroy(InventoryManager.Instance.gameObject);
            }

            GameSaveManager.Instance.ClearSave();
            SceneManager.LoadScene(newGameSceneName);
        }
        else
        {
            Debug.LogError("❌ New Game Scene Name is not set in the Inspector!");
        }
    }

    public void OnLoadGameButton()
    {
        SaveData data = GameSaveManager.Instance.LoadGame();
        if (data != null)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(data.sceneName);
        }
        else
        {
            Debug.LogWarning("⚠ No saved game to load!");
        }
    }

    // --- THIS METHOD IS MODIFIED WITH THE NEW LOGS ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        SaveData data = GameSaveManager.Instance.LoadGame();
        if (data != null)
        {
            // Load Player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(data.x, data.y, data.z);
                PlayerHealthShield phs = player.GetComponent<PlayerHealthShield>();
                if (phs != null)
                {
                    phs.currentHealth = data.currentHealth;
                    phs.currentShield = data.currentShield;
                    phs.SendMessage("UpdateUI");
                }
            }

            // Load Inventory
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.LoadInventoryFromSave(data.inventoryItems);
            }

            // Remove Duplicate Item Pickups from Scene
            if (data.inventoryItems != null)
            {
                ItemPickup[] allPickupsInScene = FindObjectsOfType<ItemPickup>();
                foreach (ItemPickup pickup in allPickupsInScene)
                {
                    if (pickup.itemData != null && data.inventoryItems.Contains(pickup.itemData.name))
                    {
                        Destroy(pickup.gameObject);
                    }
                }
            }

            // --- ADD THIS ENTIRE BLOCK TO DEBUG QUIZ MACHINES ---

            // Load Quiz Machine States
            if (data.usedQuizMachineIds != null && data.usedQuizMachineIds.Count > 0)
            {
                // Log 1: Shows you all the used IDs loaded from the file.
                Debug.Log($"<color=cyan>LOADED used machine IDs: {string.Join(", ", data.usedQuizMachineIds)}</color>");

                QuizMachine[] allQuizMachines = FindObjectsOfType<QuizMachine>();
                foreach (QuizMachine machine in allQuizMachines)
                {
                    // Log 2: Shows you which machine it's checking in the scene.
                    Debug.Log($"Checking machine in scene with ID: '{machine.uniqueId}'");

                    if (data.usedQuizMachineIds.Contains(machine.uniqueId))
                    {
                        // Log 3: Confirms when a match is found.
                        Debug.Log($"<color=green>MATCH FOUND! Marking '{machine.uniqueId}' as completed.</color>");
                        machine.MarkAsCompleted();
                    }
                }
            }
            else
            {
                Debug.Log("<color=yellow>Loaded save data, but no used quiz machine IDs were found.</color>");
            }

            // --- END OF NEW BLOCK ---
        }

        // Final scene setup
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void OnSettingsButton()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("🚪 Game closed.");
    }
}