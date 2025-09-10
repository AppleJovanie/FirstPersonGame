using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public DoorType doorType;
    public string firstSceneToLoad;
    public ItemData requiredItem;
    public DoorTriggerArea myTriggerArea;

    // --- THIS LINE IS MODIFIED ---
    public bool isCompleted { get; private set; } = false;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;
        isCompleted = GameProgressManager.IsDoorCompleted(doorType);

        if (isCompleted)
        {
            GetComponent<Renderer>().material.color *= 0.5f;
        }

        if (myTriggerArea == null)
        {
            Debug.LogError($"DoorController on '{gameObject.name}' is missing its 'myTriggerArea' reference!");
        }
    }

    public void Interact()
    {
        if (isCompleted)
        {
            Debug.Log($"The {doorType} path has already been completed.");
            return;
        }

        if (GameFlowManager.Instance == null || GameFlowManager.Instance.GetActiveDoorTriggerArea() != myTriggerArea)
        {
            Debug.Log("This door is not the one that is currently active.");
            return;
        }

        if (inventoryManager != null && inventoryManager.HasItem(requiredItem))
        {
            Debug.Log($"Entering the {doorType} door.");
            if (!string.IsNullOrEmpty(firstSceneToLoad))
            {
                SceneManager.LoadScene(firstSceneToLoad);
            }
        }
        else
        {
            Debug.Log($"You need the {requiredItem.itemName} to enter this door!");
        }
    }
}