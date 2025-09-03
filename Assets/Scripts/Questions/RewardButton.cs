using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    private ItemData currentReward;
    private QuizManager quizManager;

    public void Setup(ItemData reward, QuizManager manager)
    {
        currentReward = reward;
        quizManager = manager;
        icon.sprite = reward.itemSprite;
        nameText.text = reward.itemName;
    }

    public void OnClick()
    {
        Debug.Log("Item Selected");
        if (currentReward != null && quizManager != null)
        {
            quizManager.RewardSelected(currentReward);
        }
    }
}
