using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    // You can add more data here later, like a description, a prefab to equip, etc.

    [TextArea(3, 10)] // Makes the text box in the Inspector bigger (3 to 10 lines)
    public string itemDescription;

    public GameObject itemPrefab;
}
