using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/Question")]

public class Question : ScriptableObject
{
    [TextArea(3, 10)]
    public string questionText;
    public string[] answers = new string[4];
    public int correctAnswerIndex; // 0=A, 1=B, 2=C, 3=D
}
