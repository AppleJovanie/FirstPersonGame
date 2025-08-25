using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    
    private InventoryManager inventoryManager;
    public ItemData itemData;

   
    public string itemName = "Pistol"; // Example item name

    void Start()
    {
        
        inventoryManager = FindObjectOfType<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError("Could not find an InventoryManager in the scene!");
        }
    }

  
    public void Interact()
    {
        Debug.Log($"Interacting with {gameObject.name}");

       
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(itemData); 
        }

       
        Destroy(gameObject);
    }
}
