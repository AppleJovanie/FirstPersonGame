using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [System.Serializable] // Makes it visible in the Inspector for debugging
    public struct InventoryItem
    {
        public ItemData itemData;
        public int quantity;

        public InventoryItem(ItemData data, int qty)
        {
            itemData = data;
            quantity = qty;
        }
    }

    public class InventoryManager : MonoBehaviour
    {
        // Singleton pattern: Ensures only one instance of the InventoryManager exists.
        public static InventoryManager Instance { get; private set; }

        // The actual inventory storage (list of slots).
        [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();
        public int inventorySize = 10; // Total number of inventory slots (e.g., 10 for 2 rows of 5)

        // Equipped Weapon properties
        public ItemData equippedWeaponData; // The ItemData of the currently equipped weapon
        public int currentWeaponAmmo;       // Current ammo of the equipped weapon

        // Event to notify UI when the general inventory changes.
        // Parameters: ItemData (the item that changed), int (its new quantity), int (the slot index).
        public event Action<ItemData, int, int> OnInventoryChanged;
        // Event to notify UI when the equipped weapon changes.
        // Parameters: ItemData (the new weapon), int (its current ammo).
        public event Action<ItemData, int> OnEquippedWeaponChanged;

        private void Awake()
        {
            // Implement the singleton pattern.
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Keep the inventory manager across scenes
            }
            else
            {
                Destroy(gameObject); // Destroy duplicate instances
            }
        }

        private void Start()
        {
            // Populate inventory with empty slots initially
            for (int i = 0; i < inventorySize; i++)
            {
                inventory.Add(new InventoryItem(null, 0)); // null ItemData signifies an empty slot
            }
            // Trigger initial UI update for all slots
            for (int i = 0; i < inventorySize; i++)
            {
                OnInventoryChanged?.Invoke(inventory[i].itemData, inventory[i].quantity, i);
            }
            // Trigger initial UI update for equipped weapon (starts null/empty)
            OnEquippedWeaponChanged?.Invoke(equippedWeaponData, currentWeaponAmmo);
        }

        // Method to add an item to the inventory.
        public bool AddItem(ItemData itemToAdd, int quantityToAdd = 1)
        {
            if (itemToAdd == null)
            {
                Debug.LogWarning("Attempted to add a null item.");
                return false;
            }

            // Try to stack the item if it's stackable and already exists.
            if (itemToAdd.isStackable)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    // Check if the slot contains the same item and has space.
                    if (inventory[i].itemData == itemToAdd && inventory[i].quantity < itemToAdd.maxStackSize)
                    {
                        int spaceLeft = itemToAdd.maxStackSize - inventory[i].quantity;
                        int amountToStack = Mathf.Min(quantityToAdd, spaceLeft);

                        inventory[i] = new InventoryItem(itemToAdd, inventory[i].quantity + amountToStack);
                        quantityToAdd -= amountToStack;

                        // Notify UI about the change in this specific slot.
                        OnInventoryChanged?.Invoke(inventory[i].itemData, inventory[i].quantity, i);

                        if (quantityToAdd <= 0)
                        {
                            Debug.Log($"Added {amountToStack} x {itemToAdd.itemName}.");
                            return true; // All quantity added.
                        }
                    }
                }
            }

            // If not stackable, or no existing stack has space, find an empty slot.
            if (quantityToAdd > 0) // Still have items to add
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i].itemData == null) // Found an empty slot
                    {
                        // For stackable items, add up to maxStackSize in this new slot.
                        int amountToAdd = itemToAdd.isStackable ? Mathf.Min(quantityToAdd, itemToAdd.maxStackSize) : 1;

                        inventory[i] = new InventoryItem(itemToAdd, amountToAdd);
                        quantityToAdd -= amountToAdd;

                        // Notify UI.
                        OnInventoryChanged?.Invoke(inventory[i].itemData, inventory[i].quantity, i);

                        if (quantityToAdd <= 0)
                        {
                            Debug.Log($"Added {amountToAdd} x {itemToAdd.itemName} to a new slot.");
                            return true; // All quantity added.
                        }
                    }
                }
            }

            Debug.LogWarning($"Inventory is full or could not add all {itemToAdd.itemName} (remaining: {quantityToAdd}).");
            return false; // Could not add all items.
        }

        // Method to remove an item from the inventory.
        public bool RemoveItem(ItemData itemToRemove, int quantityToRemove = 1)
        {
            if (itemToRemove == null) return false;

            // Iterate backwards to safely remove items or reduce quantity.
            for (int i = inventory.Count - 1; i >= 0; i--)
            {
                if (inventory[i].itemData == itemToRemove)
                {
                    if (inventory[i].quantity > quantityToRemove)
                    {
                        // Reduce quantity in the slot.
                        inventory[i] = new InventoryItem(itemToRemove, inventory[i].quantity - quantityToRemove);
                        quantityToRemove = 0;
                    }
                    else
                    {
                        // Remove the entire stack/item from the slot.
                        quantityToRemove -= inventory[i].quantity;
                        inventory[i] = new InventoryItem(null, 0); // Empty the slot
                    }

                    // Notify UI about the change.
                    OnInventoryChanged?.Invoke(inventory[i].itemData, inventory[i].quantity, i);

                    if (quantityToRemove <= 0)
                    {
                        Debug.Log($"Removed {itemToRemove.itemName}.");
                        return true; // All quantity removed.
                    }
                }
            }

            Debug.LogWarning($"Could not remove all {itemToRemove.itemName}. Not enough found.");
            return false; // Could not remove all requested quantity.
        }

        // Method to check if the inventory contains a specific item (at least one).
        public bool HasItem(ItemData itemToCheck)
        {
            foreach (var item in inventory)
            {
                if (item.itemData == itemToCheck && item.quantity > 0)
                {
                    return true;
                }
            }
            return false;
        }

        // Method to get the quantity of a specific item.
        public int GetItemQuantity(ItemData itemToCheck)
        {
            int totalQuantity = 0;
            foreach (var item in inventory)
            {
                if (item.itemData == itemToCheck)
                {
                    totalQuantity += item.quantity;
                }
            }
            return totalQuantity;
        }

        // Method to get the item data and quantity at a specific slot index.
        public InventoryItem GetItemInSlot(int index)
        {
            if (index >= 0 && index < inventory.Count)
            {
                return inventory[index];
            }
            return new InventoryItem(null, 0); // Return empty if index is out of bounds
        }

        // Method to equip a weapon
        public void EquipWeapon(ItemData weaponData)
        {
            if (weaponData == null || !weaponData.isWeapon)
            {
                Debug.LogWarning("Attempted to equip a non-weapon item or null item.");
                return;
            }

            equippedWeaponData = weaponData;
            currentWeaponAmmo = weaponData.maxAmmo; // Start with full ammo when equipped
            Debug.Log($"Equipped weapon: {equippedWeaponData.itemName} with {currentWeaponAmmo} ammo.");
            OnEquippedWeaponChanged?.Invoke(equippedWeaponData, currentWeaponAmmo);
        }

        // Method to reduce equipped weapon ammo
        public void ReduceEquippedWeaponAmmo(int amount)
        {
            if (equippedWeaponData == null)
            {
                Debug.LogWarning("No weapon equipped to reduce ammo from.");
                return;
            }
            currentWeaponAmmo = Mathf.Max(0, currentWeaponAmmo - amount);
            Debug.Log($"Reduced ammo. Current: {currentWeaponAmmo}");
            OnEquippedWeaponChanged?.Invoke(equippedWeaponData, currentWeaponAmmo); // Update UI
        }
    }
}