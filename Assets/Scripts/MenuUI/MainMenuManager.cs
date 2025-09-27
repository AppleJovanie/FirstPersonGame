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

        if (loadGameButton != null && GameSaveManager.Instance != null)
            loadGameButton.SetActive(GameSaveManager.Instance.HasSave());
    }

    public void OnNewGameButton()
    {
        if (!string.IsNullOrEmpty(newGameSceneName))
        {
            // ✅ Clean up old managers to prevent broken references
            if (InventoryManager.Instance != null)
                Destroy(InventoryManager.Instance.gameObject);

            if (GameFlowManager.Instance != null)
                Destroy(GameFlowManager.Instance.gameObject);

            if (GameProgressManager.Instance != null)
                Destroy(GameProgressManager.Instance.gameObject);

            // ❌ Removed SettingsManager + AudioManager cleanup 
            // (they don’t exist in your project)

            // ✅ Clear save so it really starts fresh
            if (GameSaveManager.Instance != null)
                GameSaveManager.Instance.ClearSave();

            // ✅ Reset quiz machines in the current scene (safe cleanup before reload)
            QuizMachine[] allMachines = FindObjectsOfType<QuizMachine>();
            foreach (QuizMachine machine in allMachines)
            {
                machine.ResetMachine();
            }

            // ✅ Load the main game scene fresh
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
                InventoryManager.Instance.LoadInventoryFromSave(data.inventoryItems);

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

            // Load Quiz Machine States
            if (data.usedQuizMachineIds != null && data.usedQuizMachineIds.Count > 0)
            {
                Debug.Log($"<color=cyan>LOADED used machine IDs: {string.Join(", ", data.usedQuizMachineIds)}</color>");

                QuizMachine[] allQuizMachines = FindObjectsOfType<QuizMachine>();
                foreach (QuizMachine machine in allQuizMachines)
                {
                    Debug.Log($"Checking machine in scene with ID: '{machine.uniqueId}'");

                    if (data.usedQuizMachineIds.Contains(machine.uniqueId))
                    {
                        Debug.Log($"<color=green>MATCH FOUND! Marking '{machine.uniqueId}' as completed.</color>");
                        machine.MarkAsCompleted();
                    }
                }
            }
            else
            {
                Debug.Log("<color=yellow>Loaded save data, but no used quiz machine IDs were found.</color>");
            }
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
