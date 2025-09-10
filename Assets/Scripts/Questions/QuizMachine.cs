using UnityEngine;

public class QuizMachine : MonoBehaviour, IInteractable
{
    [Header("Save/Load Settings")]
    [Tooltip("A unique ID for this specific machine (e.g., 'RedDoor_Quiz1'). MUST BE UNIQUE.")]
    public string uniqueId;

    [Header("Quiz Logic")]
    public Question[] questions;

    // This can be read by other scripts, but only changed from within this one.
    public bool hasBeenUsed { get; private set; } = false;

    // Reference to the main quiz manager in the scene.
    private QuizManager quizManager;

    void Start()
    {
        // Find the QuizManager in the scene.
        quizManager = FindObjectOfType<QuizManager>();

        // Warn the developer if they forgot to set a unique ID.
        if (string.IsNullOrEmpty(uniqueId))
        {
            Debug.LogError($"QuizMachine '{gameObject.name}' is missing a unique ID! The save system will not work for this machine.");
        }

        if (quizManager == null)
        {
            Debug.LogError("QuizMachine cannot find a QuizManager in the scene!");
        }
    }

    public void Interact()
    {
        // First, check if the machine has already been used.
        if (hasBeenUsed)
        {
            Debug.Log($"This quiz machine ({uniqueId}) has already been completed.");
            return; // Stop the interaction.
        }

        // If not used, start the quiz.
        Debug.Log($"Player is using quiz machine: {uniqueId}");

        // Pass the questions, the success callback, and 'false' because it doesn't require a key.
        quizManager.StartQuiz(questions, HandleMachineSuccess, false);
    }

    // This is the callback function that runs when the QuizManager reports a correct answer.
    private void HandleMachineSuccess()
    {
        MarkAsCompleted();
        quizManager.ShowRandomRewards();
    }

    // This public method allows the save/load system (or the quiz itself) to mark the machine as used.
    public void MarkAsCompleted()
    {
        hasBeenUsed = true;
    }
}