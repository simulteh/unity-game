using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelChecker : MonoBehaviour
{
    // Класс для хранения условий уровня
    [System.Serializable]
    public class LevelCondition
    {
        public string description;
        public string functionName; // Имя функции из GlobalEvents
        //public bool isRequired; // Обязательна ли эта функция
        public bool isCompleted;
    }

    public List<LevelCondition> levelConditions = new List<LevelCondition>();

    public float maxTimeForThreeStars = 300.0f;
    private float startTime;

    public int stars = 5;

    [SerializeField] TextMeshProUGUI textStars;
    [SerializeField] TextMeshProUGUI textTime;
    [SerializeField] TextMeshProUGUI textComments;

    void Start()
    {
        startTime = Time.time;
    }

    // Автоматическая проверка выполнения функций в GlobalEvents
    private void CheckGlobalEventsFunctions()
    {
        if (GlobalEvents.Instance == null) return;

        foreach (var condition in levelConditions)
        {
            // Если условие связано с функцией и еще не выполнено
            if (!string.IsNullOrEmpty(condition.functionName) && !condition.isCompleted)
            {
                if (GlobalEvents.Instance.WasFunctionExecuted(condition.functionName))
                {
                    condition.isCompleted = true;
                    Debug.Log($"Автоматически выполнено: {condition.description}");
                }
            }
        }
    }

    // Ручная установка выполнения условия (для не-GlobalEvents условий)
    public void CompleteCondition(int index)
    {
        if (index >= 0 && index < levelConditions.Count)
        {
            levelConditions[index].isCompleted = true;
            Debug.Log($"Условие '{levelConditions[index].description}' выполнено.");
        }
        else
        {
            Debug.LogWarning("Некорректный индекс условия.");
        }
    }

    // Ручная установка выполнения условия по описанию
    public void CompleteCondition(string description)
    {
        var condition = levelConditions.Find(c => c.description == description);
        if (condition != null)
        {
            condition.isCompleted = true;
            Debug.Log($"Условие '{condition.description}' выполнено.");
        }
        else
        {
            Debug.LogWarning($"Условие с описанием '{description}' не найдено.");
        }
    }

    // Подсчет количества выполненных условий
    public int GetCompletedConditionsCount()
    {
        int count = 0;
        foreach (var condition in levelConditions)
        {
            if (condition.isCompleted) count++;
        }
        return count;
    }

    // Проверка выполнены ли все обязательные условия
    public bool AreAllRequiredConditionsMet()
    {
        foreach (var condition in levelConditions)
        {
            //if (condition.isRequired && !condition.isCompleted)
            if (!condition.isCompleted)
            {
                Debug.Log($"Не выполнено обязательное условие: {condition.description}");
                return false;
            }
        }
        Debug.Log("Все обязательные условия выполнены!");
        return true;
    }

    // Получение отчета о невыполненных условиях
    public string GetFailedConditionsReport()
    {
        string report = "";
        int counter = 1;

        foreach (var condition in levelConditions)
        {
            //if (condition.isRequired && !condition.isCompleted)
            if (!condition.isCompleted)
            {
                report += $"{counter}. {condition.description}\n";
                counter++;
            }
        }

        return report;
    }

    public void CalculateStars()
    {
        CheckGlobalEventsFunctions();
        bool allRequiredMet = AreAllRequiredConditionsMet();
        int completed = GetCompletedConditionsCount();
        int total = levelConditions.Count;

        if (allRequiredMet)
        {
            stars = 5;
            textComments.text = "Вы проделали отличную работу! Все основные задачи выполнены.";

            // Бонус за выполнение всех условий (включая опциональные)
            //if (completed == total)
            //{
            //    textComments.text += "\nВыполнены даже дополнительные задания!";
            //}
        }
        else
        {
            stars = 2;
            textComments.text = "Вы не выполнили следующие важные пункты:\n";
            textComments.text += GetFailedConditionsReport();
        }

        float timeTaken = Time.time - startTime;

        // Корректировка звезд на основе времени
        if (timeTaken > maxTimeForThreeStars)
        {
            stars--;
            if (timeTaken > maxTimeForThreeStars * 1.5f)
            {
                stars--;
            }
        }

        stars = Mathf.Max(1, stars);
        textStars.text = $"Оценка {stars}/5";
        textTime.text = $"Время: {timeTaken:F2} сек.";
        textComments.text += $"\nВыполнено: {completed}/{total} заданий";
    }

    // Сброс всех условий
    public void ResetAllConditions()
    {
        foreach (var condition in levelConditions)
        {
            condition.isCompleted = false;
        }

        if (GlobalEvents.Instance != null)
        {
            GlobalEvents.Instance.ResetAllExecutions();
        }

        startTime = Time.time;
        Debug.Log("Все условия сброшены");
    }
}