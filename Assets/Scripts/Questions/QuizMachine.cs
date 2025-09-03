using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizMachine : MonoBehaviour, IInteractable
{
    public Question[] questions;
    // This can now be read by other scripts, but only changed by this one.
    public bool hasBeenUsed { get; private set; } = false;

    private QuizManager quizManager;
    private InventoryManager inventoryManager; // <-- NEW: Add reference to inventory

    void Start()
    {
        quizManager = FindObjectOfType<QuizManager>();
        inventoryManager = FindObjectOfType<InventoryManager>(); // <-- NEW: Find the inventory manager

        if (quizManager == null)
            Debug.LogError("QuizMachine cannot find a QuizManager!");
        if (inventoryManager == null)
            Debug.LogError("QuizMachine cannot find an InventoryManager!");
    }

    // --- THIS METHOD IS COMPLETELY REWRITTEN ---
    public void Interact()
    {
        if (hasBeenUsed)
        {
            Debug.Log("This quiz machine has already been completed.");
            return;
        }

        // The key check has been removed. The player can always interact if it hasn't been used.
        Debug.Log("Player is using a free quiz machine.");

        // We now pass 'false' to indicate this quiz does NOT require a key.
        quizManager.StartQuiz(questions, HandleMachineSuccess, false);

    }

    private void HandleMachineSuccess()
    {
        MarkAsCompleted();
        quizManager.ShowRandomRewards();
    }

    // This method can now be called by the QuizManager after a correct answer.
    public void MarkAsCompleted()
    {
        hasBeenUsed = true;
    }

    public Question GetRandomQuestion()
    {
        if (questions.Length == 0) return null;
        int randomIndex = Random.Range(0, questions.Length);
        return questions[randomIndex];
    }
}