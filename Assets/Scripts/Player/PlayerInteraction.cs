using UnityEngine;
using TMPro; // Required for TextMeshPro UI elements

public class PlayerInteraction : MonoBehaviour
{
    [Header("Required Components")]
    [Tooltip("The source of the interaction ray (usually the main camera).")]
    [SerializeField] private Transform interactorSource;
    [Tooltip("The UI Text element that will display the interaction prompt.")]
    [SerializeField] private TextMeshProUGUI interactionPromptText;

    [Header("Interaction Settings")]
    [Tooltip("How far the player can interact from.")]
    [SerializeField] private float interactRange = 3f;
    [Tooltip("Which layers the interaction ray can hit.")]
    [SerializeField] private LayerMask interactableMask;

    // This will store the interactable object the player is currently looking at.
    private IInteractable currentInteractable;

    void Start()
    {
        // Ensure required components are assigned in the Inspector.
        if (interactorSource == null)
        {
            Debug.LogError("PlayerInteraction: Interactor Source is not assigned! Please assign your camera.");
        }
        if (interactionPromptText == null)
        {
            Debug.LogError("PlayerInteraction: Interaction Prompt Text is not assigned! Please assign a TextMeshProUGUI element.");
        }
        else
        {
            // Make sure the prompt is hidden when the game starts.
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // --- 1. Detection Phase ---
        IInteractable interactable = null; // Assume we are looking at nothing interactable for this frame.
        Ray ray = new Ray(interactorSource.position, interactorSource.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange, interactableMask))
        {
            if (hitInfo.collider.TryGetComponent(out IInteractable interactObj))
            {
                // --- THIS IS THE NEW LOGIC ---
                // Before we do anything, check if this object is a used QuizMachine.
                QuizMachine machine = interactObj as QuizMachine;

                // If it IS a QuizMachine AND it has already been used, we will ignore it.
                if (machine != null && machine.hasBeenUsed)
                {
                    // By not assigning 'interactable', we treat it as if we saw nothing.
                }
                else
                {
                    // Otherwise, it's a valid interactable object.
                    interactable = interactObj;
                }
            }
        }

        // --- 2. State and UI Phase ---

        // If our view has changed (looking at something new, or looking away)
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

        // --- 3. Interaction Phase ---

        // If we are looking at a valid interactable object AND the player presses 'E'.
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    private void ShowPrompt()
    {
        if (interactionPromptText == null) return;

        // Generate a specific prompt based on the type of interactable object.
        string promptMessage = "Press [E] to Interact"; // Default message

        if (currentInteractable is ItemPickup itemPickup && itemPickup.itemData != null)
        {
            promptMessage = $"Press [E] to pick up {itemPickup.itemData.itemName}";
        }
        else if (currentInteractable is QuizMachine)
        {
            promptMessage = "Press [E] to use Quiz Machine";
        }
        else if (currentInteractable is ExitDoor)
        {
            promptMessage = "Press [E] to attempt lock";
        }

        interactionPromptText.text = promptMessage;
        interactionPromptText.gameObject.SetActive(true);
    }

    private void HidePrompt()
    {
        // Clear the reference when we look away.
        currentInteractable = null;
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }
}