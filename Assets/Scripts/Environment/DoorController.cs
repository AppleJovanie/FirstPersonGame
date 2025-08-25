using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    // The specific item required to open this door (e.g., GunData)
    public ItemData requiredItem;
    public string nextSceneName;

    private InventoryManager inventoryManager;

    void Start()
    {
        // Find the inventory manager when the scene starts
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("DoorController could not find an InventoryManager in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryProceedToNextScene();
        }
    }

    public void TryProceedToNextScene()
    {
        // The new logic: Check the inventory directly!
        if (inventoryManager != null && inventoryManager.HasItem(requiredItem))
        {
            ProceedToNextScene();
        }
        else
        {
            Debug.Log("The door is locked. You are missing the required item!");
            // You can show a UI message here, like "Requires Gun"
        }
    }

    public void ProceedToNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}