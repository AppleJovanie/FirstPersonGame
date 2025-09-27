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
        // Keep track of progress, but don't lock out the button
        isCompleted = GameProgressManager.IsDoorCompleted(doorType);

        if (doorHighlightLight != null)
        {
            // The light should always be off or disabled at the start.
            doorHighlightLight.enabled = false;
        }
    }

    public void Interact()
    {
        // 🚫 Removed "disabled if completed" check
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
        ButtonController[] allButtons = FindObjectsOfType<ButtonController>();

        foreach (ButtonController button in allButtons)
        {
            if (button.doorHighlightLight != null)
            {
                button.doorHighlightLight.enabled = (button == this);
            }
        }
    }
}
