using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class ExitDoor : MonoBehaviour, IInteractable
{
    [Header("Quiz Logic")]
    [Tooltip("The questions for this specific door's quiz.")]
    public Question[] questions;
    [Tooltip("The name of the scene to load upon a correct answer (e.g., MainScene).")]
    public string sceneToLoad;

    // --- NEW SECTION ---
    [Header("Progress Tracking")]
    [Tooltip("The color of the path this door completes.")]
    public DoorType doorTypeToComplete; // This tells the manager which path is finished.
    // --- END NEW SECTION ---

    private QuizManager quizManager;
    private InventoryManager inventoryManager;

    void Start()
    {
        // Find the managers in the scene
        quizManager = FindObjectOfType<QuizManager>();
        inventoryManager = FindObjectOfType<InventoryManager>();

        if (quizManager == null)
            Debug.LogError("ExitDoor cannot find a QuizManager!");
        if (inventoryManager == null)
            Debug.LogError("ExitDoor cannot find an InventoryManager!");
    }

    public void Interact()
    {
        // This logic remains the same: it checks for a key and starts the quiz.
        if (inventoryManager != null && inventoryManager.HasKey())
        {
            Debug.Log("Player has a key. Starting door quiz.");
            quizManager.StartQuiz(questions, LoadNextScene, true);
        }
        else
        {
            Debug.Log("You need a key to attempt this lock!");
        }
    }

    // This method is passed to the QuizManager to be called only on a correct answer.
    private void LoadNextScene()
    {
        // --- THIS IS THE NEW LINE OF CODE ---
        // Before loading the next scene, we tell the GameProgressManager that this path is complete.
        GameProgressManager.CompleteDoor(doorTypeToComplete);
        // --- END NEW LINE ---

        Debug.Log("Quiz passed! Returning to " + sceneToLoad);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
    }
}
