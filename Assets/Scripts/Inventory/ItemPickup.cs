using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Tooltip("The ItemData asset this pickup represents. This MUST be assigned in the Inspector.")]
    public ItemData itemData;

    public void Interact()
    {
        // First, check if an item has been assigned to prevent errors.
        if (itemData == null)
        {
            Debug.LogError($"No ItemData assigned to the pickup object: {gameObject.name}");
            return;
        }

        // Use the reliable singleton instance to find the inventory.
        if (InventoryManager.Instance != null)
        {
            // Add the assigned item to the player's inventory.
            InventoryManager.Instance.AddItem(itemData);
            Debug.Log($"Picked up and added '{itemData.name}' to inventory.");
        }
        else
        {
            Debug.LogError("Could not find an InventoryManager instance in the scene!");
        }

        // Destroy the pickup object from the world.
        Destroy(gameObject);
    }
}