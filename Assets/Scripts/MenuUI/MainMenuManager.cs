using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void NewGame()
    {
        // Load your gameplay scene (change "GameScene" to your actual game scene name)
        SceneManager.LoadScene("GameScene");
    }

    public void LoadGame()
    {
        // Implement your load logic or load to checkpoint
        Debug.Log("Load Game clicked");
    }

    public void OpenSettings()
    {
        // Show settings panel
        Debug.Log("Settings clicked");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit clicked"); // Useful for testing in editor
    }
}

