using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Transform interactorSource;
    [SerializeField] private TextMeshProUGUI interactionPromptText;

    [Header("Audio")]
    [Tooltip("The sound to play when successfully interacting with an object.")]
    [SerializeField] private AudioClip interactSound;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableMask;

    private IInteractable currentInteractable;
    private InventoryManager inventoryManager;

    void Start()
    {
        // It's more reliable to use the singleton instance
        inventoryManager = InventoryManager.Instance;

        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        IInteractable interactable = null;
        Ray ray = new Ray(interactorSource.position, interactorSource.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange, interactableMask))
        {
            if (hitInfo.collider.TryGetComponent(out IInteractable interactObj))
            {
                interactable = interactObj;
            }
        }

        if (interactable != currentInteractable)
        {
            currentInteractable = interactable;
            if (currentInteractable != null)
            {
                ShowPrompt();
            }
            else
            {
                HidePrompt();
            }
        }

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            if (InteractionSoundPlayer.Instance != null)
            {
                InteractionSoundPlayer.Instance.PlaySound(interactSound);
            }
            currentInteractable.Interact();
        }
    }

    // --- THIS METHOD IS THE ONLY ONE THAT IS MODIFIED ---
    private void ShowPrompt()
    {
        if (interactionPromptText == null) return;
        string promptMessage = "Press [E] to Interact";
        bool canInteract = true;

        // Check if the object we're looking at is a DoorController
        if (currentInteractable is DoorController door)
        {
            // Check 1: Is the path permanently completed?
            if (door.isCompleted)
            {
                promptMessage = "Path Completed";
                canInteract = false;
            }
            // --- ADD THIS NEW CHECK ---
            // Check 2: Is this door the one activated by the button?
            else if (GameFlowManager.Instance == null || GameFlowManager.Instance.GetActiveDoorTriggerArea() != door.myTriggerArea)
            {
                promptMessage = "Door Inactive"; // Display this message if the wrong button was pressed
                canInteract = false;
            }
            // --- END OF NEW CHECK ---
            // Check 3: Does the player have the required item?
            else if (inventoryManager != null && !inventoryManager.HasItem(door.requiredItem))
            {
                promptMessage = $"Requires {door.requiredItem.itemName}";
                canInteract = false;
            }
            // If all checks pass, show the "Enter" prompt
            else { promptMessage = "Press [E] to Enter"; }
        }
        else if (currentInteractable is QuizMachine machine && machine.hasBeenUsed)
        {
            promptMessage = "Machine Used";
            canInteract = false;
        }
        else if (currentInteractable is ItemPickup itemPickup && itemPickup.itemData != null)
        {
            promptMessage = $"Press [E] to pick up {itemPickup.itemData.itemName}";
        }
        else if (currentInteractable is ExitDoor)
        {
            promptMessage = "Press [E] to attempt lock";
        }

        interactionPromptText.text = promptMessage;
        interactionPromptText.gameObject.SetActive(true);

        // If any check failed, we nullify the interactable so the player can't press 'E'
        if (!canInteract)
        {
            currentInteractable = null;
        }
    }

    private void HidePrompt()
    {
        currentInteractable = null;
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }
}