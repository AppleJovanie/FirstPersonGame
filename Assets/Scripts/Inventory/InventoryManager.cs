using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    private List<ItemData> items = new List<ItemData>();
    public List<InventorySLot> itemSlots = new List<InventorySLot>();
    public ItemData selectedItem { get; private set; }
    public GameObject InventoryMenu;
    private bool menuActivated;

    [Header("Description Panel")]
    public GameObject descriptionPanel;
    public Image descriptionItemImage;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI equipPromptText;
    public Transform handTransform;
    private GameObject equippedItemObject;

    private void Awake()
    {
        InventoryManager[] managers = FindObjectsOfType<InventoryManager>();
        if (managers.Length > 1) { Destroy(gameObject); }
        else { DontDestroyOnLoad(gameObject); }
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    // --- THIS IS THE MODIFIED SECTION ---
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject playerHandObject = GameObject.FindGameObjectWithTag("PlayerHand");
        if (playerHandObject != null)
            handTransform = playerHandObject.transform;
        else
            handTransform = null;

        GameObject inventoryUIObject = GameObject.FindGameObjectWithTag("InventoryUI");
        if (inventoryUIObject != null)
        {
            // Use the already assigned InventoryMenu (don’t search for it again)
            // If you want, you can still fetch it just in case:
            if (InventoryMenu == null)
                InventoryMenu = inventoryUIObject.transform.Find("InventoryMenu").gameObject;

            // ✅ Fix: look inside InventoryMenu instead of directly under InventoryUI
            Transform descriptionPanelTransform = InventoryMenu.transform.Find("InventoryDescriptionPanel");
            if (descriptionPanelTransform != null)
            {
                descriptionPanel = descriptionPanelTransform.gameObject;
                descriptionItemImage = descriptionPanelTransform.Find("ItemImage").GetComponent<Image>();

                Transform itemDescriptionContainer = descriptionPanelTransform.Find("ItemDescription");
                itemDescriptionText = itemDescriptionContainer.Find("ItemDescriptionText").GetComponent<TMPro.TextMeshProUGUI>();
                equipPromptText = itemDescriptionContainer.Find("EquipPromptText").GetComponent<TMPro.TextMeshProUGUI>();
            }

            // ✅ Fix: get slots directly under InventorySlots
            itemSlots.Clear();
            Transform slotsContainer = InventoryMenu.transform.Find("InventorySlots");
            if (slotsContainer != null)
                itemSlots.AddRange(slotsContainer.GetComponentsInChildren<InventorySLot>());

            foreach (InventorySLot slot in itemSlots)
                slot.Setup(this);

            RepopulateUI();
            Debug.Log("InventoryManager has re-connected and repopulated UI in the new scene.");
        }
    }

    void RepopulateUI()
    {
        foreach (InventorySLot slot in itemSlots) { slot.ClearSlot(); }
        for (int i = 0; i < items.Count; i++)
        {
            if (i < itemSlots.Count) { itemSlots[i].DisplayItem(items[i]); }
        }
    }

    public void AddItem(ItemData itemToAdd)
    {
        items.Add(itemToAdd);
        RepopulateUI();
    }

    public bool HasItem(ItemData itemToCheck)
    {
        return items.Contains(itemToCheck);
    }

    public void ClearInventory()
    {
        items.Clear();
        if (InventoryMenu != null) { RepopulateUI(); }
        Debug.Log("Inventory data and UI has been cleared.");
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory")) { ToggleInventory(); }
        if (menuActivated && selectedItem != null && Input.GetKeyDown(KeyCode.E)) { EquipItem(selectedItem); }
    }

    void ToggleInventory()
    {
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

    void EquipItem(ItemData itemToEquip)
    {
        if (equippedItemObject != null) { Destroy(equippedItemObject); }
        if (itemToEquip.itemPrefab != null) { equippedItemObject = Instantiate(itemToEquip.itemPrefab, handTransform); }
        ToggleInventory();
    }

    public void SelectItem(ItemData itemToSelect)
    {
        selectedItem = itemToSelect;
        descriptionPanel.SetActive(true);
        equipPromptText.gameObject.SetActive(true);
        descriptionItemImage.sprite = selectedItem.itemSprite;
        itemDescriptionText.text = selectedItem.itemDescription;
    }

    void ClearDescriptionPanel()
    {
        selectedItem = null;
        if (descriptionPanel != null) descriptionPanel.SetActive(false);
        if (equipPromptText != null) equipPromptText.gameObject.SetActive(false);
    }
}