using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class SimpleKeyDoor : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [Tooltip("The name of the scene to load when the door is opened.")]
    public string sceneToLoad;

    // The InventoryManager reference is no longer needed.
    // private InventoryManager inventoryManager;

    void Start()
    {
        // The code that finds the InventoryManager has been removed.

        // This check is still useful to avoid errors.
        if (string.IsNullOrEmpty(sceneToLoad))
            Debug.LogError($"Door '{gameObject.name}' is missing a 'sceneToLoad' name!");
    }

    public void Interact()
    {
        // The entire key-checking logic has been removed.
        // Now, it just checks if a scene name has been provided and loads it immediately.
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Transitioning to scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}