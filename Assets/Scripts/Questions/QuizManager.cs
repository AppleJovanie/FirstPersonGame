using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public static bool IsQuizActive { get; private set; }

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
    private Action onQuizSuccess;

    public void StartQuiz(Question[] questions, Action successCallback, bool requiresKey)
    {
        EnsureInventoryManager();

        Time.timeScale = 0f;
        if (playerHudCanvas != null)
        {
            playerHudCanvas.SetActive(false);
        }

        if (requiresKey && inventoryManager != null)
        {
            inventoryManager.ConsumeKey();
        }

        onQuizSuccess = successCallback;

        if (inventoryManager != null)
        {
            inventoryManager.inventoryLocked = true;
        }

        IsQuizActive = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DisplayNewQuestion(questions);
        quizPanel.SetActive(true);
        feedbackText.text = "";
    }

    private void EndQuiz()
    {
        IsQuizActive = false;
        Time.timeScale = 1f;

        if (playerHudCanvas != null)
        {
            playerHudCanvas.SetActive(true);
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

    private void AnswerSelected(int index)
    {
        foreach (var btn in answerButtons) { btn.interactable = false; }

        if (index == currentQuestion.correctAnswerIndex)
        {
            feedbackText.text = "Correct!";
            StartCoroutine(CorrectAnswerRoutine());
        }
        else
        {
            feedbackText.text = "Incorrect!";
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(penaltyDamage);
            }
            StartCoroutine(IncorrectAnswerRoutine());
        }
    }

    private IEnumerator CorrectAnswerRoutine()
    {
        yield return new WaitForSecondsRealtime(1f);

        // ✅ FIX: properly end the quiz first
        EndQuiz();

        // then call the success action (loads next scene in ExitDoor)
        onQuizSuccess?.Invoke();
    }

    private IEnumerator IncorrectAnswerRoutine()
    {
        yield return new WaitForSecondsRealtime(2f);
        EndQuiz();
    }

    public void ShowRandomRewards()
    {
        List<ItemData> availableRewards = new List<ItemData>();
        foreach (ItemData reward in allPossibleRewards)
        {
            if (reward.requiredItem != null)
            {
                if (inventoryManager.HasItem(reward.requiredItem))
                {
                    availableRewards.Add(reward);
                }
            }
            else
            {
                availableRewards.Add(reward);
            }
        }

        quizPanel.SetActive(false);
        rewardPanel.SetActive(true);

        List<ItemData> finalRewards = availableRewards.OrderBy(x => UnityEngine.Random.value).Take(3).ToList();

        for (int i = 0; i < rewardButtons.Length; i++)
        {
            if (i < finalRewards.Count)
            {
                rewardButtons[i].Setup(finalRewards[i], this);
                rewardButtons[i].gameObject.SetActive(true);
            }
            else
            {
                rewardButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void DisplayNewQuestion(Question[] questions)
    {
        if (questions == null || questions.Length == 0)
        {
            EndQuiz();
            return;
        }

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

    public void RewardSelected(ItemData reward)
    {
        EnsureInventoryManager();

        if (inventoryManager != null)
        {
            inventoryManager.AddItem(reward);
            Debug.Log($"Added reward {reward.name} to inventory.");
        }
        else
        {
            Debug.LogError("InventoryManager not found when trying to add reward!");
        }

        EndQuiz();
    }

    private void EnsureInventoryManager()
    {
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
    }
}
