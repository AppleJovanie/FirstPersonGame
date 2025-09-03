using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public GameObject playerObject;
    public PlayerHealthShield playerHealth;
    public InventoryManager inventoryManager; // Make sure this is assigned in the Inspector
    public int penaltyDamage = 10;
    public ItemData[] allPossibleRewards;

    private Question currentQuestion;
    private QuizMachine activeQuizMachine;
    public bool IsQuizActive { get; private set; } = false;

    void Start()
    {
        quizPanel.SetActive(false);
        rewardPanel.SetActive(false);
        // Find the inventory manager if it's not assigned
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
    }

    public void StartQuiz(QuizMachine machine)
    {
        activeQuizMachine = machine;
        IsQuizActive = true;

        // --- LOCK THE INVENTORY ---
        if (inventoryManager != null)
        {
            inventoryManager.inventoryLocked = true;
        }

        // Disable player movement scripts here
        // Example: if(playerObject != null) playerObject.GetComponent<PlayerMovement>().enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DisplayNewQuestion();
        quizPanel.SetActive(true);
        feedbackText.text = "";
    }

    void EndQuiz()
    {
        IsQuizActive = false;

        // --- UNLOCK THE INVENTORY ---
        if (inventoryManager != null)
        {
            inventoryManager.inventoryLocked = false;
        }

        quizPanel.SetActive(false);
        rewardPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Re-enable player movement scripts here
        // Example: if(playerObject != null) playerObject.GetComponent<PlayerMovement>().enabled = true;
    }

    public void RewardSelected(ItemData reward)
    {
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(reward);
        }
        EndQuiz();
    }

    void DisplayNewQuestion()
    {
        currentQuestion = activeQuizMachine.GetRandomQuestion();
        if (currentQuestion == null) return;

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

    void AnswerSelected(int index)
    {
        if (index == currentQuestion.correctAnswerIndex)
        {
            feedbackText.text = "You answered correctly!";
            Invoke("ShowRandomRewards", 1f);
        }
        else
        {
            feedbackText.text = "Incorrect! You take damage.";
            if (playerHealth != null) playerHealth.TakeDamage(penaltyDamage);
            Invoke("EndQuiz", 2f);
        }
        foreach (var btn in answerButtons) { btn.interactable = false; }
    }

    void ShowRandomRewards()
    {
        quizPanel.SetActive(false);
        rewardPanel.SetActive(true);
        List<ItemData> randomRewards = allPossibleRewards.OrderBy(x => Random.value).Take(3).ToList();
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
}

