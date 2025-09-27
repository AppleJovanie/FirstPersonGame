using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Assign the GameOverPanel from your persistent canvas here.")]
    public GameObject gameOverPanel;

    private string currentSceneName;

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

    public void TriggerGameOver()
    {
        if (gameOverPanel != null)
        {
            Debug.Log("Game Over Triggered!");
            currentSceneName = SceneManager.GetActiveScene().name;
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // --- THIS METHOD IS MODIFIED ---
    public void RetryLevel()
    {
        if (gameOverPanel != null)
        {
            // Hide the panel before reloading
            gameOverPanel.SetActive(false);
        }

        // Re-lock the cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Unpause the game and reload the scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentSceneName);
    }

    // --- THIS METHOD IS MODIFIED ---
    public void ReturnToMainMenu()
    {
        if (gameOverPanel != null)
        {
            // Hide the panel before going to the menu
            gameOverPanel.SetActive(false);
        }

        // Unpause the game
        Time.timeScale = 1f;

        // Make sure to close any other persistent UI, like the inventory
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.CloseInventory();
        }

        // Note: The Main Menu script should handle its own cursor state (unlocked and visible)
        SceneManager.LoadScene("MainMenu");
    }
}