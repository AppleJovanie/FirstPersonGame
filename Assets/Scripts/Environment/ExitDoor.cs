using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour, IInteractable
{
    [Header("Quiz Logic")]
    public Question[] questions;
    public string sceneToLoad;

    [Header("Progress Tracking")]
    public DoorType doorTypeToComplete;

    private QuizManager quizManager;
    private InventoryManager inventoryManager;

    void Start()
    {
        quizManager = FindObjectOfType<QuizManager>();
        inventoryManager = FindObjectOfType<InventoryManager>();

        if (quizManager == null)
            Debug.LogError("ExitDoor cannot find a QuizManager!");
        if (inventoryManager == null)
            Debug.LogError("ExitDoor cannot find an InventoryManager!");
    }

    public void Interact()
    {
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

    private void LoadNextScene()
    {
        GameProgressManager.CompleteDoor(doorTypeToComplete);

        Debug.Log("Quiz passed! Returning to " + sceneToLoad);

        SceneManager.LoadScene(sceneToLoad);
    }
}
