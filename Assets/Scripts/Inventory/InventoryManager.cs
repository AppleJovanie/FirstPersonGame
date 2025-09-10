using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    // --- ADDED THIS LINE ---
    public static InventoryManager Instance { get; private set; }

    public static bool IsMenuOpen { get; private set; }

    [Header("Save/Load Settings")]
    [Tooltip("You MUST assign every possible ItemData asset in the game here for the load system to work.")]
    public List<ItemData> allPossibleItemsInGame;

    // --- Item Data ---
    private List<ItemData> items = new List<ItemData>();
    private List<ItemData> equippableItems = new List<ItemData>();
    private int currentEquippableIndex = -1;

    // --- UI References ---
    [Header("UI Prefabs & Containers")]
    public GameObject itemSlotPrefab;
    public Transform slotsContainer;
    private List<InventorySLot> itemSlots = new List<InventorySLot>();

    [Header("Menu & Description")]
    public GameObject InventoryMenu;
    public GameObject descriptionPanel;
    public Image descriptionItemImage;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI usePromptText;
    private TextMeshProUGUI ammoText;

    [Header("Player References")]
    public Transform handTransform;
    public PlayerHealthShield playerHealth;
    public Gun currentlyEquippedGun;

    [Header("External Managers")]
    public Filelogmanager fileLogUIManager;

    // --- State ---
    private GameObject equippedItemObject;
    public ItemData selectedItem { get; private set; }
    private bool menuActivated;
    public bool inventoryLocked = false;

    // --- MODIFIED AWAKE METHOD ---
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        // When a new scene loads, our references to the old player are invalid. Clear them first.
        handTransform = null;
        playerHealth = null;
        // --- END OF NEW BLOCK ---

        ClearAllKeys();
        FindPlayerReferences();
       // FindUICanvasReferences();

        GameObject ammoTextObject = GameObject.FindGameObjectWithTag("AmmoUI");
        if (ammoTextObject != null)
        {
            ammoText = ammoTextObject.GetComponent<TextMeshProUGUI>();
            if (ammoText != null) ammoText.enabled = false;
        }
        // Now that we have found the new HandTransform, we can safely re-equip the weapon.
        if (currentEquippableIndex != -1)
        {
            EquipWeaponByIndex(currentEquippableIndex);
        }
        RepopulateUI();
    }

    void Update()
    {
        if (inventoryLocked) return;
        if (Input.GetButtonDown("Inventory")) { ToggleInventory(); }
        if (menuActivated && selectedItem != null && Input.GetKeyDown(KeyCode.E)) { UseItem(selectedItem); }
        if (!menuActivated)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0 && equippableItems.Count > 1)
            {
                if (scroll > 0f)
                {
                    currentEquippableIndex++;
                    if (currentEquippableIndex >= equippableItems.Count) currentEquippableIndex = 0;
                }
                else if (scroll < 0f)
                {
                    currentEquippableIndex--;
                    if (currentEquippableIndex < 0) currentEquippableIndex = equippableItems.Count - 1;
                }
                EquipWeaponByIndex(currentEquippableIndex);
            }
        }
    }

    public void AddItem(ItemData itemToAdd)
    {
        items.Add(itemToAdd);
        if (itemToAdd.type == ItemType.Equippable)
        {
            equippableItems.Add(itemToAdd);
            if (currentEquippableIndex == -1)
            {
                currentEquippableIndex = 0;
                EquipWeaponByIndex(0);
            }
        }
        RepopulateUI();
    }

    public void RemoveItem(ItemData itemToRemove)
    {
        items.Remove(itemToRemove);
        if (itemToRemove.type == ItemType.Equippable)
        {
            equippableItems.Remove(itemToRemove);
        }
        RepopulateUI();
    }

    public bool HasItem(ItemData itemToCheck) => items.Contains(itemToCheck);

    public void ClearInventory()
    {
        items.Clear();
        equippableItems.Clear();
        currentEquippableIndex = -1;
        if (equippedItemObject != null)
        {
            Destroy(equippedItemObject);
        }
        if (ammoText != null)
        {
            ammoText.enabled = false;
        }
        RepopulateUI();
        Debug.Log("Inventory Cleared.");
    }

    void UseItem(ItemData itemToUse)
    {
        if (itemToUse.type == ItemType.Equippable) { EquipItem(itemToUse); }
        else if (itemToUse.type == ItemType.Consumable) { ConsumeItem(itemToUse); }
        else if (itemToUse.type == ItemType.Readable) { ReadItem(itemToUse); }
    }

    void EquipItem(ItemData itemToEquip)
    {
        int weaponIndex = equippableItems.IndexOf(itemToEquip);
        if (weaponIndex != -1)
        {
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

    private void EquipWeaponByIndex(int index)
    {
        if (index < 0 || index >= equippableItems.Count) return;
        if (equippedItemObject != null) Destroy(equippedItemObject);
        if (ammoText != null) ammoText.enabled = false;
        ItemData weaponToEquip = equippableItems[index];
        if (handTransform != null)
        {
            equippedItemObject = Instantiate(weaponToEquip.itemPrefab, handTransform);
            currentlyEquippedGun = equippedItemObject.GetComponent<Gun>();
            if (currentlyEquippedGun != null && ammoText != null)
            {
                currentlyEquippedGun.Initialize(ammoText);
            }
        }
        else { Debug.LogError("Cannot equip weapon because HandTransform reference is missing!"); }
    }

    void ConsumeItem(ItemData itemToConsume)
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(itemToConsume.healAmount);
            playerHealth.AddShield(itemToConsume.shieldAmount);
        }
        if (itemToConsume.ammoType != AmmoType.None && itemToConsume.ammoAmount > 0)
        {
            if (currentlyEquippedGun != null && currentlyEquippedGun.ammoType == itemToConsume.ammoType)
            {
                currentlyEquippedGun.AddReserveAmmo(itemToConsume.ammoAmount);
            }
            else { Debug.Log("Wrong ammo type for the currently equipped gun!"); }
        }
        RemoveItem(itemToConsume);
        ClearDescriptionPanel();
    }

    void ReadItem(ItemData itemToRead)
    {
        if (fileLogUIManager != null) { fileLogUIManager.ShowLog(itemToRead, this); }
        else { Debug.LogError("FileLogUIManager is not assigned in the InventoryManager!"); }
    }

    public bool HasKey() => items.Any(item => item.type == ItemType.Key);

    public void ConsumeKey()
    {
        ItemData keyToRemove = items.FirstOrDefault(item => item.type == ItemType.Key);
        if (keyToRemove != null) { RemoveItem(keyToRemove); }
    }

    public void ClearAllKeys()
    {
        int keysRemoved = items.RemoveAll(item => item.type == ItemType.Key);
        if (keysRemoved > 0) { RepopulateUI(); }
    }

    void ToggleInventory()
    {
        if (InventoryMenu == null) return;
        menuActivated = !menuActivated;
        InventoryMenu.SetActive(menuActivated);
        IsMenuOpen = menuActivated;
        if (menuActivated)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ClearDescriptionPanel();
        }
    }

    // --- Add this entire new method to your InventoryManager.cs script ---

    public void CloseInventory()
    {
        // Check if the menu is actually open before trying to close it.
        if (InventoryMenu != null && menuActivated)
        {
            menuActivated = false;
            InventoryMenu.SetActive(false);
            IsMenuOpen = false;

            // Reset cursor and time for the main menu.
            // We don't need to lock the cursor in the main menu.
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            ClearDescriptionPanel();
            Debug.Log("Inventory forcibly closed for scene change.");
        }
    }
    public void SelectItem(ItemData itemToSelect)
    {
        // --- ADDED LOGS FOR DEBUGGING ---
        Debug.Log($"--- SelectItem method entered. Attempting to display: {itemToSelect.name} ---");

        selectedItem = itemToSelect;

        if (descriptionPanel == null)
        {
            Debug.LogError("FAILURE: The 'descriptionPanel' reference is NULL in the InventoryManager!");
            return;
        }
        Debug.Log("SUCCESS: 'descriptionPanel' reference is valid.");
        descriptionPanel.SetActive(true);

        if (descriptionItemImage != null)
        {
            descriptionItemImage.sprite = itemToSelect.itemSprite;
        }

        if (itemDescriptionText == null)
        {
            Debug.LogError("FAILURE: The 'itemDescriptionText' reference is NULL in the InventoryManager!");
            return;
        }
        Debug.Log("SUCCESS: 'itemDescriptionText' reference is valid.");
        itemDescriptionText.text = itemToSelect.itemDescription;

        if (usePromptText != null)
        {
            usePromptText.gameObject.SetActive(true);
            if (itemToSelect.type == ItemType.Equippable) { usePromptText.text = "Press [E] to Equip"; }
            else if (itemToSelect.type == ItemType.Readable) { usePromptText.text = "Press [E] to Read"; }
            else { usePromptText.text = "Press [E] to Use"; }
        }

        Debug.Log("--- SelectItem UI update complete. ---");
    }

    void ClearDescriptionPanel()
    {
        selectedItem = null;
        if (descriptionPanel != null) descriptionPanel.SetActive(false);
        if (usePromptText != null) usePromptText.gameObject.SetActive(false);
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

    private void FindPlayerReferences()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            handTransform = playerObject.transform.Find("Main Camera/HandTransform");
            playerHealth = playerObject.GetComponent<PlayerHealthShield>();
        }
    }

    private void FindUICanvasReferences()
    {
        GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
        if (inventoryCanvas == null) { return; }
        Transform menuTransform = inventoryCanvas.transform.Find("InventoryMenu");
        if (menuTransform != null)
        {
            InventoryMenu = menuTransform.gameObject;
            Transform slots = menuTransform.Find("InventorySlots");
            if (slots != null) { slotsContainer = slots; }
            Transform descPanel = menuTransform.Find("InventoryDescriptionPanel");
            if (descPanel != null)
            {
                descriptionPanel = descPanel.gameObject;
                descriptionItemImage = descPanel.Find("ItemImage")?.GetComponent<Image>();
                itemDescriptionText = descPanel.Find("ItemDescriptionText")?.GetComponent<TextMeshProUGUI>();
                usePromptText = descPanel.Find("EquipPromptText")?.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    public List<ItemData> GetAllItems()
    {
        return new List<ItemData>(items);
    }

    // This debugging method is correctly implemented.
    public void LoadInventoryFromSave(List<string> savedItems)
    {
        ClearInventory();
        Debug.Log("Inventory cleared. Attempting to load items from save file...");
        foreach (string itemName in savedItems)
        {
            ItemData item = allPossibleItemsInGame.Find(i => i.name == itemName);
            if (item != null)
            {
                Debug.Log($"<color=green>SUCCESS:</color> Found '{itemName}' in allPossibleItemsInGame. Adding to inventory.");
                AddItem(item);
            }
            else
            {
                Debug.LogWarning($"<color=red>FAILURE:</color> Could not find an item asset named '{itemName}' in the 'allPossibleItemsInGame' list!");
            }
        }
    }
}