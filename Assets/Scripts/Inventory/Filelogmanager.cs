using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Filelogmanager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject fileLogPanel;
    public TextMeshProUGUI clueText;

    private bool isPanelOpen = false;
    private InventoryManager inventoryManagerInstance; // To re-enable inventory UI

    void Update()
    {
        // Listen for the escape key to close the panel
        if (isPanelOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseLog();
        }
    }

    // This is called by the InventoryManager
    public void ShowLog(ItemData logData, InventoryManager invManager)
    {
        inventoryManagerInstance = invManager;
        clueText.text = logData.clueText;

        // Hide the main inventory UI and show this one
        inventoryManagerInstance.InventoryMenu.SetActive(false);
        fileLogPanel.SetActive(true);
        isPanelOpen = true;
    }

    private void CloseLog()
    {
        // Hide this panel and re-show the inventory UI
        fileLogPanel.SetActive(false);
        inventoryManagerInstance.InventoryMenu.SetActive(true);
        isPanelOpen = false;
    }
}
