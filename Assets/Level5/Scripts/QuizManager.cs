using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("¬опросы (по 3 тогла в каждом, прив€жите в инспекторе)")]
    public Toggle[] q1Options;
    public Toggle[] q2Options;
    public Toggle[] q3Options;

    public Button submitBtn;
    public GameObject successMsg;

    // »ндексы правильных ответов: 1-а(0), 2-б(1), 3-в(2)
    private int[] correctIndices = { 0, 1, 2 };

    private void Start()
    {
        submitBtn.onClick.AddListener(CheckAnswers);
        successMsg.SetActive(false);
    }

    private void CheckAnswers()
    {
        bool isCorrect = IsGroupCorrect(q1Options, correctIndices[0]) &&
                         IsGroupCorrect(q2Options, correctIndices[1]) &&
                         IsGroupCorrect(q3Options, correctIndices[2]);

        successMsg.SetActive(isCorrect);
        if (isCorrect) Level5Manager.Instance.SetStep("Quiz_Passed");
    }

    private bool IsGroupCorrect(Toggle[] group, int correctIdx)
    {
        for (int i = 0; i < group.Length; i++)
        {
            if (group[i].isOn && i == correctIdx) return true;
            if (group[i].isOn && i != correctIdx) return false;
        }
        return false;
    }
}