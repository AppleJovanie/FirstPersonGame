using UnityEngine;

public class QuizMachine : MonoBehaviour, IInteractable
{
    [Header("Save/Load Settings")]
    [Tooltip("A unique ID for this specific machine (e.g., 'RedDoor_Quiz1'). MUST BE UNIQUE.")]
    public string uniqueId;

    [Header("Quiz Logic")]
    public Question[] questions;

    public bool hasBeenUsed { get; private set; } = false;

    private QuizManager quizManager;

    void Start()
    {
        quizManager = FindObjectOfType<QuizManager>();

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
        if (hasBeenUsed)
        {
            Debug.Log($"This quiz machine ({uniqueId}) has already been completed.");
            return;
        }

        Debug.Log($"Player is using quiz machine: {uniqueId}");
        quizManager.StartQuiz(questions, HandleMachineSuccess, false);
    }

    private void HandleMachineSuccess()
    {
        MarkAsCompleted();
        quizManager.ShowRandomRewards();
    }

    public void MarkAsCompleted()
    {
        hasBeenUsed = true;

        // Tell save manager this machine is used
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.usedQuizMachineIds.Add(uniqueId);
        }
    }

    // 🔄 NEW: Reset state when starting a new game
    public void ResetMachine()
    {
        hasBeenUsed = false;
        Debug.Log($"QuizMachine '{uniqueId}' reset for New Game.");
    }
}
