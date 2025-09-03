using UnityEngine;

// This defines the different types of items we can have.
public enum ItemType { Equippable, Consumable, Readable }

[CreateAssetMenu(fileName = "New ItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public Sprite itemSprite;
    [TextArea(3, 10)]
    public string itemDescription;
    public ItemType type; // Choose the type from a dropdown in the Inspector

    [Header("Equippable")]
    // This will only be used if the type is 'Equippable'.
    public GameObject itemPrefab;

    [Header("Consumable Effects")]
    // These will only be used if the type is 'Consumable'.
    public int healAmount = 0;
    public int shieldAmount = 0;
    public int ammoAmount = 0; // For ammo packs

    [Header("Readable/Clue Settings")]

    [TextArea(15, 20)] // This makes the text box bigger in the Inspector
    public string clueText;

}