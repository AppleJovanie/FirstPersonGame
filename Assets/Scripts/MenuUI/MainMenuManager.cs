using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene to Load")]
    // Assign the name of your main game scene in the Inspector (e.g., "MainScene")
    public string newGameSceneName;

    [Header("UI Panels")]
    // Assign your Settings Panel GameObject in the Inspector
    public GameObject settingsPanel;

    void Start()
    {
        // Make sure the settings panel is hidden when the menu starts
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // --- Button Functions ---

    public void OnNewGameButton()
    {
        if (!string.IsNullOrEmpty(newGameSceneName))
        {
            Debug.Log($"Starting new game, loading scene: {newGameSceneName}");
            SceneManager.LoadScene(newGameSceneName);
        }
        else
        {
            Debug.LogError("New Game Scene Name is not set in the MainMenuManager Inspector!");
        }
    }

    public void OnLoadGameButton()
    {
        // Placeholder for future save/load functionality
        Debug.Log("Load Game button pressed. (Functionality not yet implemented)");
        // Example: SaveSystem.LoadGame();
    }

    public void OnSettingsButton()
    {
        if (settingsPanel != null)
        {
            // Toggle the settings panel's visibility
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
            Debug.Log($"Settings panel toggled to: {!isActive}");
        }
        else
        {
            Debug.LogError("Settings Panel is not assigned in the MainMenuManager Inspector!");
        }
    }

    public void OnExitButton()
    {
        Debug.Log("Exit button pressed. Quitting application.");
        // This will only work in a built version of the game, not in the Unity Editor.
        Application.Quit();
    }
}
