using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySLot : MonoBehaviour
{
    public Image iconImage; // Drag the Image component of this slot here
    public ItemData currentItem { get; private set; }
    private InventoryManager inventoryManager;

    public void Setup(InventoryManager manager)
    {
        inventoryManager = manager;
    }

    // Called by InventoryManager to place an item in this slot
    public void DisplayItem(ItemData item)
    {
        currentItem = item;
        iconImage.sprite = item.itemSprite;
        iconImage.enabled = true;

        // Make the slot clickable
        GetComponent<Button>().interactable = true;
    }

    // Clears the slot
    public void ClearSlot()
    {
        currentItem = null;
        iconImage.sprite = null;
        iconImage.enabled = false;

        // Make the slot non-clickable
        GetComponent<Button>().interactable = false;
    }

    // This method will be called when the button is clicked
    public void OnSlotClicked()
    {
        if (currentItem != null && inventoryManager != null)
        {
            // Tell the InventoryManager that this item was selected
            inventoryManager.SelectItem(currentItem);
            Debug.Log("Thhe Item is selected");
        }
    }

}
