using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // Required for using the 'Action' delegate

public class QuizManager : MonoBehaviour
{
    [Header("Quiz UI")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons = new Button[4];
    public TextMeshProUGUI feedbackText;

    [Header("Reward UI")]
    public GameObject rewardPanel;
    public RewardButton[] rewardButtons;

    [Header("Game Logic")]
    public PlayerHealthShield playerHealth;
    public InventoryManager inventoryManager;
    public int penaltyDamage = 10;
    public ItemData[] allPossibleRewards;
    public GameObject playerHudCanvas;

    private Question currentQuestion;
    // This delegate will store the action to perform on a correct answer (e.g., show rewards OR load scene).
    private Action onQuizSuccess;

    void Start()
    {
        quizPanel.SetActive(false);
        rewardPanel.SetActive(false);
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
    }

    /// <summary>
    /// Starts a new quiz session.
    /// </summary>
    /// <param name="questions">The array of possible questions for this quiz.</param>
    /// <param name="successCallback">The method to call when the player answers correctly.</param>
    /// <param name="requiresKey">Set to true if this quiz costs a key to attempt.</param>
    public void StartQuiz(Question[] questions, Action successCallback, bool requiresKey)
    {
        Time.timeScale = 0f; // This freezes all gameplay, animations, and physics
        if (playerHudCanvas != null)
        {
            playerHudCanvas.SetActive(false);
        }

        // Only consume a key if the quiz requires one.
        if (requiresKey)
        {
            if (inventoryManager != null)
            {
                inventoryManager.ConsumeKey();
            }
        }
       

        // Store the action we need to run if the answer is correct.
        onQuizSuccess = successCallback;

        // Lock inventory and show cursor.
        if (inventoryManager != null)
        {
            inventoryManager.inventoryLocked = true;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Display the quiz UI with a new question.
        DisplayNewQuestion(questions);
        quizPanel.SetActive(true);
        feedbackText.text = "";
    }

    // Inside QuizManager.cs

    private void AnswerSelected(int index)
    {
        // Disable all answer buttons to prevent multiple clicks.
        foreach (var btn in answerButtons) { btn.interactable = false; }

        if (index == currentQuestion.correctAnswerIndex)
        {
            feedbackText.text = "Correct!";
            // We now start a coroutine that can wait even when time is paused.
            StartCoroutine(CorrectAnswerRoutine());
        }
        else
        {
            feedbackText.text = "Incorrect!";
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(penaltyDamage);
            }
            // Start the other coroutine for an incorrect answer.
            StartCoroutine(IncorrectAnswerRoutine());
        }
    }

    private void ExecuteSuccessAction()
    {
        onQuizSuccess.Invoke();
    }

    private void EndQuiz()
    {
        Time.timeScale = 1f; // This unfreezes the game
        if (playerHudCanvas != null)
        {
            playerHudCanvas.SetActive(true);
            Debug.Log("The game unfreezes");
        }

        if (inventoryManager != null)
        {
            inventoryManager.inventoryLocked = false;
        }

        quizPanel.SetActive(false);
        rewardPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

       

    }

    private void DisplayNewQuestion(Question[] questions)
    {
        if (questions == null || questions.Length == 0)
        {
            Debug.LogError("No questions provided for this quiz!");
            EndQuiz();
            return;
        }

        // Get a random question from the provided array.
        currentQuestion = questions[UnityEngine.Random.Range(0, questions.Length)];

        questionText.text = currentQuestion.questionText;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i];
            int buttonIndex = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => AnswerSelected(buttonIndex));
            answerButtons[i].interactable = true;
        }
    }

    /// <summary>
    /// This method is now public. It's used by QuizMachine as its success action.
    /// </summary>
    public void ShowRandomRewards()
    {
        quizPanel.SetActive(false);
        rewardPanel.SetActive(true);
        List<ItemData> randomRewards = allPossibleRewards.OrderBy(x => UnityEngine.Random.value).Take(3).ToList();

        for (int i = 0; i < rewardButtons.Length; i++)
        {
            if (i < randomRewards.Count)
            {
                rewardButtons[i].Setup(randomRewards[i], this);
                rewardButtons[i].gameObject.SetActive(true);
            }
            else
            {
                rewardButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Called by a RewardButton after a reward has been selected from the UI.
    /// </summary>
    public void RewardSelected(ItemData reward)
    {
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(reward);
        }
        EndQuiz();
    }

    // Add these two new methods to your QuizManager.cs script

    private System.Collections.IEnumerator CorrectAnswerRoutine()
    {
        // This waits for 1 second of REAL time, ignoring Time.timeScale.
        yield return new WaitForSecondsRealtime(1f);

        // After waiting, it executes the success action.
        ExecuteSuccessAction();
    }

    private System.Collections.IEnumerator IncorrectAnswerRoutine()
    {
        // This waits for 2 seconds of REAL time.
        yield return new WaitForSecondsRealtime(2f);

        // After waiting, it ends the quiz and unfreezes the game.
        EndQuiz();
    }
}