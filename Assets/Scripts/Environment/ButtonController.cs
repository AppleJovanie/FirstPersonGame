using UnityEngine;

public class ButtonController : MonoBehaviour, IInteractable
{
    [Header("Configuration")]
    public DoorType doorType;
    public Light doorHighlightLight;
    public DoorTriggerArea associatedDoorTriggerArea;

    private bool isCompleted = false;

    void Start()
    {
        isCompleted = GameProgressManager.IsDoorCompleted(doorType);

        if (doorHighlightLight != null)
        {
            // The light should always be off or disabled at the start.
            doorHighlightLight.enabled = false;
        }
    }

    public void Interact()
    {
        // Check if the path is permanently complete.
        if (isCompleted)
        {
            Debug.Log($"The {doorType} path is already complete. This button is disabled.");
            return;
        }

        Debug.Log($"Button for {doorType} door pressed!");

        // Tell the GameFlowManager which door trigger is now active.
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.SetActiveDoorTriggerArea(associatedDoorTriggerArea);
        }

        // Visually update the lights.
        TurnOnMyLightAndOffOthers();
    }

    private void TurnOnMyLightAndOffOthers()
    {
        // Find all other buttons in the scene.
        ButtonController[] allButtons = FindObjectsOfType<ButtonController>();

        foreach (ButtonController button in allButtons)
        {
            if (button.doorHighlightLight != null)
            {
                // Turn my light on, turn all others off.
                button.doorHighlightLight.enabled = (button == this);
            }
        }
    }
}