using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class ExitDoor : MonoBehaviour, IInteractable
{
    [Header("Quiz Logic")]
    [Tooltip("The questions for this specific door's quiz.")]
    public Question[] questions;
    [Tooltip("The name of the scene to load upon a correct answer.")]
    public string sceneToLoad;

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
        // Check if the player has a key to attempt opening the door
        if (inventoryManager != null && inventoryManager.HasKey())
        {
            Debug.Log("Player has a key. Starting door quiz.");
            // Start the quiz and tell the QuizManager to run our "LoadNextScene" method on success.
            quizManager.StartQuiz(questions, LoadNextScene, true);
        }
        else
        {
            Debug.Log("You need a key to attempt this lock!");
            // You could show a UI message here telling the player they need a key.
        }
    }

    // This method is passed to the QuizManager to be called only on a correct answer.
    private void LoadNextScene()
    {
        Debug.Log("Quiz passed! Loading scene: " + sceneToLoad);
        // It's good practice to reset the time scale before loading a new scene.
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
    }
}