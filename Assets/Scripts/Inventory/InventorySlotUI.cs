using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemQuantityText; // Use Text if not using TextMeshPro

    // The ItemData currently in this slot.
    private ItemData currentItemData;
    // The quantity of the current item.
    private int currentQuantity;

    // Method to update the UI of the slot.
    public void UpdateSlotUI(ItemData newItemData, int newQuantity)
    {
        currentItemData = newItemData;
        currentQuantity = newQuantity;

        if (currentItemData != null)
        {
            // If there's an item, display its icon and quantity.
            itemIcon.sprite = currentItemData.itemIcon;
            itemIcon.enabled = true; // Make sure the icon is visible

            // Only show quantity if the item is stackable and quantity > 1
            if (currentItemData.isStackable && currentQuantity > 1)
            {
                itemQuantityText.text = currentQuantity.ToString();
                itemQuantityText.enabled = true;
            }
            else
            {
                itemQuantityText.enabled = false; // Hide quantity for non-stackable or single items
            }
        }
        else
        {
            // If the slot is empty, hide the icon and quantity text.
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            itemQuantityText.text = "";
            itemQuantityText.enabled = false;
        }
    }
}
