using UnityEngine;

// It now implements the IInteractable interface
public class GunPickup : MonoBehaviour, IInteractable
{
    // Assign your GunData asset in the Inspector
    public ItemData itemData;
    private InventoryManager inventoryManager;

    void Start()
    {
        // Find the inventory manager when the gun spawns
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("GunPickup could not find an InventoryManager!");
        }
    }

    // This is the method that PlayerInteraction will call when you look at the gun and press E
    public void Interact()
    {
        Debug.Log("Gun picked up via raycast and added to inventory!");

        // Add the item to the player's inventory
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(itemData);
        }

        // We disable the object so your GameFlowManager can reuse it later
        gameObject.SetActive(false);
    }

    // This method is still needed by your GameFlowManager to make the gun visible
    public void ResetGunForSpawn()
    {
        // This method can be empty, its main purpose is to be called by the manager
    }
}