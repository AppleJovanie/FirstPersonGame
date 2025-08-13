using UnityEngine;

// This attribute allows you to create new ItemData assets directly from the Unity Editor.
// Right-click in Project window -> Create -> Inventory -> Item Data
[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject // CORRECTED: Changed from MonoBehaviour to ScriptableObject
{
    // Public fields to define item properties.
    // [Header] helps organize properties in the Inspector.
    [Header("Item Information")]
    public string itemName = "New Item";              // The name of the item.
    public Sprite itemIcon;                           // The icon displayed in the inventory.
    [TextArea(3, 5)]                                  // Makes the string field a multi-line text area in Inspector.
    public string itemDescription = "A new item.";    // Detailed description of the item.

    [Header("Stacking Properties")]
    public bool isStackable = true;                   // Can this item stack in inventory?
    public int maxStackSize = 99;                     // Maximum quantity if stackable.

    // Properties specific to weapons
    [Header("Weapon Properties")]
    public bool isWeapon = false;                     // Is this item a weapon?
    public int maxAmmo = 10;                          // Maximum ammo for this weapon type (if it's a weapon)
}
