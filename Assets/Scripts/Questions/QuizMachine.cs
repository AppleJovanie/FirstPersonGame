using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizMachine : MonoBehaviour, IInteractable
{
    // A list of all possible questions for this machine
    public Question[] questions;

    // Add this new variable to track the machine's state
    private bool hasBeenUsed = false;

    private QuizManager quizManager;

    void Start()
    {
        // Find the main Quiz Manager in the scene
        quizManager = FindObjectOfType<QuizManager>();
        if (quizManager == null)
        {
            Debug.LogError("QuizMachine cannot find a QuizManager in the scene!");
        }
    }

    // This is called by your PlayerInteraction script when you press 'E'
    public void Interact()
    {
        // Only interact if the machine has not been used yet
        if (!hasBeenUsed)
        {
            Debug.Log("Interacting with the Quiz Machine.");
            if (quizManager != null)
            {
                // Tell the QuizManager to start a new quiz
                quizManager.StartQuiz(this);

                // Set the flag to true so it can't be used again
                hasBeenUsed = true;
            }
        }
        else
        {
            // Optional: Log a message that the machine is unavailable
            Debug.Log("This quiz machine has already been used.");
        }
    }

    public Question GetRandomQuestion()
    {
        if (questions.Length == 0) return null;
        int randomIndex = Random.Range(0, questions.Length);
        return questions[randomIndex];
    }
}
