using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public bool isLocked = true; // Is the door currently locked?
    // Public field to specify the name of the next scene to load.
    public string nextSceneName;

    void Start()
    {
        // No initial rotation needed as the door will not visually open.
        // Ensure nextSceneName is assigned in the Inspector.
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning($"DoorController on {gameObject.name}: nextSceneName is not assigned! Please set the scene name in the Inspector.");
        }
    }

    // This method is called when another collider enters this door's trigger collider.
    private void OnTriggerEnter(Collider other)
    {
        // Assuming your player has the tag "Player" and a Rigidbody/CharacterController.
        if (other.CompareTag("Player"))
        {
            TryProceedToNextScene();
        }
    }

    // Method to attempt to proceed to the next scene.
    public void TryProceedToNextScene()
    {
        if (!isLocked)
        {
            ProceedToNextScene();
        }
        else
        {
            Debug.Log("The door is locked. You need to find something first!");
            // Optionally, display a UI message to the player: "Door is locked!"
        }
    }

    // Loads the next scene instantly.
    public void ProceedToNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"Proceeding to scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError($"DoorController on {gameObject.name}: Cannot load next scene. nextSceneName is empty or null.");
        }
    }

    // Method to unlock the door.
    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Door unlocked!");
        // Optionally, play an unlock sound or visual effect.
    }
}
