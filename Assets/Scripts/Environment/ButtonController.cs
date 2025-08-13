using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour, IInteractable
{
    // Reference to the Light component that highlights the door this button controls.
    public Light doorHighlightLight;
    // Reference to the DoorTriggerArea associated with this button's door.
    // CORRECTED: Changed type from 'DoorTrigger' to 'DoorTriggerArea'
    public DoorTriggerArea associatedDoorTriggerArea;

    void Start()
    {
        if (doorHighlightLight == null)
        {
            Debug.LogError($"ButtonController on {gameObject.name}: Door Highlight Light is not assigned!");
        }
        if (associatedDoorTriggerArea == null)
        {
            Debug.LogError($"ButtonController on {gameObject.name}: Associated Door Trigger Area is not assigned!");
        }

        // Ensure the door highlight light is off initially
        if (doorHighlightLight != null)
        {
            doorHighlightLight.enabled = false;
        }
    }

    public void Interact()
    {
        Debug.Log($"Button {gameObject.name} pressed!");

        // Tell the GameFlowManager to set this button's associated trigger area as active.
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.SetActiveDoorTriggerArea(associatedDoorTriggerArea);
        }
        else
        {
            Debug.LogError("GameFlowManager.Instance is null! Make sure GameFlowManager is in the scene.");
        }

        // Turn on this door's highlight light and turn off others.
        TurnOnMyLightAndOffOthers();
    }

    private void TurnOnMyLightAndOffOthers()
    {
        // Find all ButtonControllers in the scene.
        ButtonController[] allButtons = FindObjectsOfType<ButtonController>();

        foreach (ButtonController button in allButtons)
        {
            if (button.doorHighlightLight != null)
            {
                // If it's this button, turn its light ON.
                if (button == this)
                {
                    button.doorHighlightLight.enabled = true;
                }
                // Otherwise, turn other lights OFF.
                else
                {
                    button.doorHighlightLight.enabled = false;
                }
            }
        }
    }
}
