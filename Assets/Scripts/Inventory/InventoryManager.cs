using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    // The persistent list that stores the actual item data
    private List<ItemData> items = new List<ItemData>();


    // The list of UI slot components currently in the scene
    private List<InventorySLot> itemSlots = new List<InventorySLot>();

    private List<ItemData> equippableItems = new List<ItemData>();
    // This tracks which weapon from the list is currently active. -1 means nothing is equipped.
    private int currentEquippableIndex = -1;

    [Header("UI Prefabs & Containers")]
    public GameObject itemSlotPrefab; // Assign your ItemSlot prefab here
    public Transform slotsContainer;  // The parent object for the slots (e.g., InventorySlots panel)

    // --- The rest of your variables for UI, Player, and State ---
    public ItemData selectedItem { get; private set; }
    public GameObject InventoryMenu;
    private bool menuActivated;

    [Header("Description Panel")]
    public GameObject descriptionPanel;
    public Image descriptionItemImage;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI usePromptText;

    [Header("Player References")]
    public Transform handTransform;
    public PlayerHealthShield playerHealth;
    public Gun currentlyEquippedGun;

    [Header("External Managers")] // A new header for organization
    public Filelogmanager fileLogUIManager;
    private TextMeshProUGUI ammoText;

    private GameObject equippedItemObject;

    // --- NEW: This flag will be controlled by the QuizManager ---
    public bool inventoryLocked = false;
   

    // --- (Awake, OnEnable, OnDisable methods are for persistence) ---
    private void Awake()
    {
        InventoryManager[] managers = FindObjectsOfType<InventoryManager>();
        if (managers.Length > 1) { Destroy(gameObject); }
        else { DontDestroyOnLoad(gameObject); }
    }
    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    // Reconnects to the UI in each new scene
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject ammoTextObject = GameObject.FindGameObjectWithTag("AmmoUI");
        if (ammoTextObject != null)
        {
            ammoText = ammoTextObject.GetComponent<TextMeshProUGUI>();
            ammoText.enabled = false; // <-- ADD THIS LINE to ensure it's hidden
        }

        ClearAllKeys();

        Debug.Log("New scene loaded. Re-linking references...");

        // Find player references
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Debug.Log("Found Player object: " + playerObject.name);

            // IMPORTANT: The path must be exact.
            Transform hand = playerObject.transform.Find("Main Camera/HandTransform");
            if (hand != null)
            {
                handTransform = hand;
                Debug.Log("Successfully found and assigned HandTransform.");
            }
            else
            {
                // This error will tell you the path is wrong.
                Debug.LogError("Could not find 'HandTransform' as a child of 'Main Camera' on the Player object!");
            }

            playerHealth = playerObject.GetComponent<PlayerHealthShield>();
            if (playerHealth == null)
            {
                Debug.LogWarning("Player object does not have a PlayerHealthShield component.");
            }
        }
        else
        {
            // This error means your player is not tagged correctly.
            Debug.LogError("COULD NOT FIND ANY GAMEOBJECT WITH THE 'Player' TAG IN THE NEW SCENE!");
        }

        // (The rest of your code for finding the InventoryCanvas...)
        // ...
        // ... it's a good idea to add similar null checks for your UI elements too.
        GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
        if (inventoryCanvas != null)
        {
            // Your existing code for finding UI elements...
        }
        else
        {
            Debug.LogError("COULD NOT FIND 'InventoryCanvas' IN THE NEW SCENE!");
        }

        // Finally, repopulate the UI
        RepopulateUI();
    }

    void RepopulateUI()
    {
        if (slotsContainer == null) return;

        while (itemSlots.Count < items.Count)
        {
            GameObject newSlotObject = Instantiate(itemSlotPrefab, slotsContainer);
            InventorySLot newSlot = newSlotObject.GetComponent<InventorySLot>();
            newSlot.Setup(this);
            itemSlots.Add(newSlot);
        }

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < items.Count)
            {
                itemSlots[i].DisplayItem(items[i]);
                itemSlots[i].gameObject.SetActive(true);
            }
            else
            {
                itemSlots[i].ClearSlot();
                itemSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void AddItem(ItemData itemToAdd)
    {
        items.Add(itemToAdd);

        // NEW: If the item is a weapon, add it to our weapon list
        if (itemToAdd.type == ItemType.Equippable)
        {
            equippableItems.Add(itemToAdd);
        }

        RepopulateUI();
    }

    public void RemoveItem(ItemData itemToRemove)
    {
        items.Remove(itemToRemove);

        // NEW: If the removed item was a weapon, remove it from our weapon list
        if (itemToRemove.type == ItemType.Equippable)
        {
            equippableItems.Remove(itemToRemove);
        }

        RepopulateUI();
    }

    public bool HasItem(ItemData itemToCheck) { return items.Contains(itemToCheck); }
    public void ClearInventory() { items.Clear(); if (InventoryMenu != null) { RepopulateUI(); } }

    // Inside InventoryManager.cs

    void Update()
    {
        // --- THIS IS THE LOCK ---
        if (inventoryLocked)
        {
            return;
        }

        // --- Inventory Menu Logic ---
        if (Input.GetButtonDown("Inventory")) { ToggleInventory(); }
        if (menuActivated && selectedItem != null && Input.GetKeyDown(KeyCode.E))
        {
            UseItem(selectedItem);
        }

        // --- NEW: SCROLL WHEEL WEAPON SWITCHING ---
        // We only check for the scroll wheel if the inventory menu is CLOSED.
        if (!menuActivated)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            // Check if the player scrolled and has more than one weapon to switch between.
            if (scroll != 0 && equippableItems.Count > 1)
            {
                if (scroll > 0f) // Scrolled up
                {
                    currentEquippableIndex++;
                    if (currentEquippableIndex >= equippableItems.Count)
                    {
                        currentEquippableIndex = 0; // Wrap around to the first weapon
                    }
                }
                else if (scroll < 0f) // Scrolled down
                {
                    currentEquippableIndex--;
                    if (currentEquippableIndex < 0)
                    {
                        currentEquippableIndex = equippableItems.Count - 1; // Wrap around to the last weapon
                    }
                }

                // Call the method to physically equip the newly selected weapon
                EquipWeaponByIndex(currentEquippableIndex);
            }
        }
    }

    void UseItem(ItemData itemToUse)
    {
        if (itemToUse.type == ItemType.Equippable) { EquipItem(itemToUse); }
        else if (itemToUse.type == ItemType.Consumable) { ConsumeItem(itemToUse); }
        else if (itemToUse.type == ItemType.Readable) { ReadItem(itemToUse); }
    }

    // Inside InventoryManager.cs

    // Replace your old EquipItem method with this new version
    void EquipItem(ItemData itemToEquip)
    {
        // Find the index of this weapon in our equippable list
        int weaponIndex = equippableItems.IndexOf(itemToEquip);

        if (weaponIndex != -1)
        {
            // If it's the same weapon, do nothing to prevent re-equipping.
            if (weaponIndex == currentEquippableIndex)
            {
                ToggleInventory();
                return;
            }

            currentEquippableIndex = weaponIndex;
            EquipWeaponByIndex(currentEquippableIndex);
        }

        ToggleInventory();
    }
    void ConsumeItem(ItemData itemToConsume)
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(itemToConsume.healAmount);
            playerHealth.AddShield(itemToConsume.shieldAmount);
        }
        if (currentlyEquippedGun != null && itemToConsume.ammoAmount > 0)
        {
            currentlyEquippedGun.AddReserveAmmo(itemToConsume.ammoAmount);
        }
        RemoveItem(itemToConsume);
        ClearDescriptionPanel();
    }
    void ReadItem(ItemData itemToRead)
    {
        if (fileLogUIManager != null)
        {
            // Call the manager to show the log, passing this inventory manager instance
            fileLogUIManager.ShowLog(itemToRead, this);
        }
        else
        {
            Debug.LogError("FileLogUIManager is not assigned in the InventoryManager!");
        }
    }
    void ToggleInventory()
    {
        if (InventoryMenu == null) return;
        menuActivated = !menuActivated;
        InventoryMenu.SetActive(menuActivated);
        if (menuActivated)
        {
            Time.timeScale = 0.0001f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ClearDescriptionPanel();
        }
    }

    public void SelectItem(ItemData itemToSelect)
    {
        selectedItem = itemToSelect;
        if (descriptionPanel == null) return;
        descriptionPanel.SetActive(true);
        if (usePromptText != null) usePromptText.gameObject.SetActive(true);

        // --- MODIFY THIS PART ---
        if (itemToSelect.type == ItemType.Equippable)
        {
            usePromptText.text = "Press [E] to Equip";
        }
        else if (itemToSelect.type == ItemType.Readable) // <-- NEW
        {
            usePromptText.text = "Press [E] to Read";   // <-- NEW
        }
        else
        {
            usePromptText.text = "Press [E] to Use";
        }
        // --- END MODIFICATION ---

        descriptionItemImage.sprite = itemToSelect.itemSprite;
        itemDescriptionText.text = itemToSelect.itemDescription;
    }
    public bool HasKey()
    {
        // Checks if any item in the inventory has the type "Key"
        return items.Any(item => item.type == ItemType.Key);
    }

    public void ConsumeKey()
    {
        // Find the first item in the list that is a key
        ItemData keyToRemove = items.FirstOrDefault(item => item.type == ItemType.Key);

        // If a key was found, remove it
        if (keyToRemove != null)
        {
            Debug.Log("Consumed a key: " + keyToRemove.itemName);
            RemoveItem(keyToRemove); // Use your existing RemoveItem method
        }
    }

    public void ClearAllKeys()
    {
        // This removes ALL items of type "Key" from the inventory list
        int keysRemoved = items.RemoveAll(item => item.type == ItemType.Key);

        if (keysRemoved > 0)
        {
            Debug.Log("Cleared " + keysRemoved + " keys from inventory for the new scene.");
            RepopulateUI(); // Update the UI to show the keys are gone
        }
    }
    void ClearDescriptionPanel()
    {
        selectedItem = null;
        if (descriptionPanel != null) descriptionPanel.SetActive(false);
        if (usePromptText != null) usePromptText.gameObject.SetActive(false);
    }
    // Add this new method to InventoryManager.cs

    private void EquipWeaponByIndex(int index)
    {
        // Make sure the index is valid
        if (index < 0 || index >= equippableItems.Count) return;

        // Destroy the old weapon if one exists
        if (equippedItemObject != null)
        {
            Destroy(equippedItemObject);
        }

        // Hide the ammo UI by default
        if (ammoText != null)
        {
            ammoText.enabled = false;
        }

        // Get the ItemData for the new weapon
        ItemData weaponToEquip = equippableItems[index];

        // Create the new weapon instance
        equippedItemObject = Instantiate(weaponToEquip.itemPrefab, handTransform);
        currentlyEquippedGun = equippedItemObject.GetComponent<Gun>();

        // Pass the ammo UI reference to the new gun
        if (currentlyEquippedGun != null && ammoText != null)
        {
            currentlyEquippedGun.Initialize(ammoText);
        }
    }
}

